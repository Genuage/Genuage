Shader "Genuage/VolumeRenderingTFShader"
{
    Properties
    {
        _Radius("Radius", float) = 1
        _VolumeTex("VolumeTex", 3D) = "" {}
        _GradientTex("GradientTex", 3D) = "" {}
        _NoiseTex("NoiseTex", 2D) = "white" {}
        _TransferFunctionTex("TFTex", 2D) = "white" {}
        _MinVal("MinVal", Range(0.0, 1.0)) = 0.0
        _MaxVal("MaxVal", Range(0.0, 1.0)) = 1.0
        _Opacity("Opacity", Range(0.0, 1.0)) = 1.0
        _NumSteps("NumSteps", Range(0, 1024)) = 512
    }
        SubShader
    {
            Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
            LOD 100
            Cull Front

            ZTest LEqual
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha

            Pass
        {
            CGPROGRAM
            #pragma multi_compile MODE_TF MODE_SURF
            #pragma multi_compile LIGHTING_ON
            #pragma vertex vert
            #pragma fragment frag


            #include "UnityCG.cginc"

            #define STEPS 100
            #define STEP_SIZE 0.01
            #define MIN_DISTANCE 0.01

            float _Radius;
            float3 _Center = float3(1.0,1.0,1.0);
            sampler3D _VolumeTex;
            sampler3D _GradientTex;
            sampler2D _NoiseTex;
            sampler2D _TransferFunctionTex;

            float _MinVal;
            float _MaxVal;
            float _Opacity;
            int _NumSteps;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
                float2 uv : TEXCOORD0;

            };

            struct v2f
            {
                float4 vertex : SV_POSITION; //Clip Space
                float3 vertexLocal : TEXCOORD1; //World Position
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertexLocal = v.vertex;
                o.uv = v.uv;
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            float getDensity(float3 pos)
            {
                return tex3Dlod(_VolumeTex, float4(pos.x, pos.y, pos.z, 0.0f));
            }

            float3 getGradient(float3 pos)
            {
                return tex3Dlod(_GradientTex, float4(pos.x, pos.y, pos.z, 0.0f)).rgb;
            }

            float4 getTF1DColor(float density)
            {
                return tex2Dlod(_TransferFunctionTex, float4(density, 0.0f, 0.0f, 0.0f));
            }

            // Find ray intersection points with axis aligned bounding box
            float2 intersectAABB(float3 rayOrigin, float3 rayDir, float3 boxMin, float3 boxMax)
            {
                float3 tMin = (boxMin - rayOrigin) / rayDir;
                float3 tMax = (boxMax - rayOrigin) / rayDir;
                float3 t1 = min(tMin, tMax);
                float3 t2 = max(tMin, tMax);
                float tNear = max(max(t1.x, t1.y), t1.z);
                float tFar = min(min(t2.x, t2.y), t2.z);
                return float2(tNear, tFar);
            };


            struct RayInfo
            {
                float3 startPos;
                float3 endPos;
                float3 direction;
                float2 aabbInters;
            };

            struct RaymarchInfo
            {
                RayInfo ray;
                int numSteps;
                float numStepsRecip;
                float stepSize;
            };

            float3 getViewRayDir(float3 vertexLocal)
            {
                if (unity_OrthoParams.w == 0)
                {
                    // Perspective
                    return normalize(ObjSpaceViewDir(float4(vertexLocal, 0.0f)));
                }
                else
                {
                    // Orthographic
                    float3 camfwd = mul((float3x3)unity_CameraToWorld, float3(0, 0, -1));
                    float4 camfwdobjspace = mul(unity_WorldToObject, camfwd);
                    return normalize(camfwdobjspace);
                }
            }


            // Get a ray for the specified fragment (back-to-front)
            RayInfo getRayBack2Front(float3 vertexLocal)
            {
                RayInfo ray;
                ray.direction = getViewRayDir(vertexLocal);
                ray.startPos = vertexLocal + float3(0.5f, 0.5f, 0.5f);
                // Find intersections with axis aligned boundinng box (the volume)
                ray.aabbInters = intersectAABB(ray.startPos, ray.direction, float3(0.0, 0.0, 0.0), float3(1.0f, 1.0f, 1.0));

                // Check if camera is inside AABB
                const float3 farPos = ray.startPos + ray.direction * ray.aabbInters.y - float3(0.5f, 0.5f, 0.5f);
                float4 clipPos = UnityObjectToClipPos(float4(farPos, 1.0f));
                ray.aabbInters += min(clipPos.w, 0.0);

                ray.endPos = ray.startPos + ray.direction * ray.aabbInters.y;
                return ray;
            }

            // Get a ray for the specified fragment (front-to-back)
            RayInfo getRayFront2Back(float3 vertexLocal)
            {
                RayInfo ray = getRayBack2Front(vertexLocal);
                ray.direction = -ray.direction;
                float3 tmp = ray.startPos;
                ray.startPos = ray.endPos;
                ray.endPos = tmp;
                return ray;
            }


            RaymarchInfo initRaymarch(RayInfo ray, int maxNumSteps)
            {
                #define GREATEST_BOX_DISTANCE 1.732f //Greatest possible distance from one point in the box to another

                RaymarchInfo raymarchInfo;
                raymarchInfo.stepSize = GREATEST_BOX_DISTANCE / maxNumSteps;
                raymarchInfo.numSteps = (int)clamp(abs(ray.aabbInters.x - ray.aabbInters.y) / raymarchInfo.stepSize, 1, maxNumSteps);
                raymarchInfo.numStepsRecip = 1.0 / raymarchInfo.numSteps;
                return raymarchInfo;
            }

            // Performs lighting calculations, and returns a modified colour.
            float3 calculateLighting(float3 col, float3 normal, float3 lightDir, float3 eyeDir, float specularIntensity)
            {
                float ndotl = max(lerp(0.0f, 1.5f, dot(normal, lightDir)), 0.5f); // modified, to avoid volume becoming too dark
                float3 diffuse = ndotl * col;
                float3 v = eyeDir;
                float3 r = normalize(reflect(-lightDir, normal));
                float rdotv = max(dot(r, v), 0.0);
                float3 specular = pow(rdotv, 32.0f) * float3(1.0f, 1.0f, 1.0f) * specularIntensity;
                return diffuse + specular;
            }


            //Transfer Function Rendering
            fixed4 frag_TF(v2f i)
            {
                #define NUM_STEPS 512

                

                #define OPACITY_THRESHOLD 1.0f

                RayInfo ray = getRayBack2Front(i.vertexLocal);
                RaymarchInfo raymarchInfo = initRaymarch(ray, _NumSteps);

                float3 lightDir = normalize(ObjSpaceViewDir(float4(float3(0.0f, 0.0f, 0.0f), 0.0f)));

                // Create a small random offset in order to remove artifacts
                ray.startPos += (2.0f * ray.direction * raymarchInfo.stepSize) * tex2D(_NoiseTex, float2(i.uv.x, i.uv.y)).r;

                float4 col = float4(0.0f, 0.0f, 0.0f, 0.0f);
                //float tDepth = 0.0f;

                for (int iStep = 0; iStep < raymarchInfo.numSteps; iStep++)
                {

                    const float t = iStep * raymarchInfo.numStepsRecip;
                    const float3 currPos = lerp(ray.startPos, ray.endPos, t);
                    // Get the dansity/sample value of the current position
                    const float density = getDensity(currPos);

                    //if (currPos.x < 0.0f || currPos.x >= 1.0f || currPos.y < 0.0f || currPos.y > 1.0f || currPos.z < 0.0f || currPos.z > 1.0f) // TODO: avoid branch?
                        //break;

                    float3 gradient = getGradient(currPos);

                    float4 colorsource = getTF1DColor(density);

                    #if defined (LIGHTING_ON)
                        //Warning : If we change to forward rays we'll need to invert the ray direction
                        colorsource.rgb = calculateLighting(colorsource.rgb, normalize(gradient), lightDir, ray.direction, 0.3f);
                    #endif

                    col.rgb = colorsource.a * colorsource.rgb + (1.0f - colorsource.a) * col.rgb;
                    col.a = (colorsource.a + (1.0f - colorsource.a) * col.a) * _Opacity;
                    //tDepth = max(tDepth, t * step(0.15, src.a));
                    //TODO : Add Lighting to the color
                    if (col.a > OPACITY_THRESHOLD)
                        break;

                    //col.a = col.a * 0.8f;
                }
                return col;
            }


            //Isosurface Rendering
            fixed4 frag_SURF(v2f i)
            {
                #define MAX_NUM_STEPS 1024
                #define GREATEST_BOX_DISTANCE 1.732f //Greatest possible distance from one point in the box to another
                const float stepSize = GREATEST_BOX_DISTANCE / MAX_NUM_STEPS;

                RayInfo ray = getRayFront2Back(i.vertexLocal);
                RaymarchInfo raymarchInfo = initRaymarch(ray, MAX_NUM_STEPS);

                // Create a small random offset in order to remove artifacts
                ray.startPos = ray.startPos + (2.0f * ray.direction * raymarchInfo.stepSize) * tex2D(_NoiseTex, float2(i.uv.x, i.uv.y)).r;


                float4 col = float4(0, 0, 0, 0);
                for (uint iStep = 0; iStep < raymarchInfo.numSteps; iStep++)
                {
                    const float t = iStep * raymarchInfo.numStepsRecip;
                    const float3 currPos = lerp(ray.startPos, ray.endPos, t);

                    const float density = getDensity(currPos);
                    if (density > _MinVal && density < _MaxVal)
                    {
                        float3 normal = normalize(getGradient(currPos));
                        col = getTF1DColor(density);
                        col.rgb = calculateLighting(col.rgb, normal, -ray.direction, -ray.direction, 0.15);
                        col.a = 1.0f;
                        break;
                    }
                }
                return col;
            }


            fixed4 frag(v2f i) : SV_Target
            {

                #if MODE_TF
                    return frag_TF(i);
                #elif MODE_SURF
                    return frag_SURF(i);
                #endif

            }
                ENDCG
        }
    }
}