Shader "Genuage/VolumeRenderingSurface1"
{
    Properties
    {
        _VolumeTex("VolumeTex", 3D) = "" {}
        _GradientTex("GradientTex", 3D) = "" {}

        _Threshold("Threshold", Range(0.0, 1.0)) = 0.0

        _Intensity("Intensity", Range(0.0, 1.0)) = 0.0

    }
        SubShader
        {
                Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
                LOD 100
                Cull Front

                ZTest LEqual
                ZWrite Off
                Blend SrcAlpha OneMinusSrcAlpha

            Pass
            {
                CGPROGRAM
                #pragma multi_compile LIGHTING_ON
                #pragma vertex vert
                #pragma fragment frag


                #include "UnityCG.cginc"

                #define offset 0.01
                #define num_steps 500

                float step_size = 1.732 / num_steps;
                sampler3D _VolumeTex;
                sampler3D _GradientTex;

                float _Threshold,_Intensity;


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

                float4 diff(float3 direction, float4 normal)
                {

                    float prod;
                    prod = dot(direction, normal);
                    float4 diff = float4(prod,prod,prod,1) * (1, 0, 0, 1);
                    return diff;
                }
                fixed4 raymarch(float3 position, float3 direction)
                {
                    float3 direction2 = (-direction);
                    float3 pos2 = position + (direction2) * 512 * step_size;
                    float4 finalValue = float4(0, 0, 0, 0);
                    for (int i = 0; i < num_steps; i++)
                    {
                        pos2 = pos2 + offset * (-direction2);
                        float4 color = tex3Dlod(_VolumeTex, float4(pos2.x, pos2.y, pos2.z, 0));
                        float4 color2 = tex3Dlod(_GradientTex, float4(pos2.x, pos2.y, pos2.z, 0));
                        if (pos2.x < 0.0f || pos2.x > 1.0f || pos2.y < 0.0f || pos2.y > 1.0f || pos2.z < 0.0f || pos2.z > 1.0f)
                            continue;
                        if (color.x > _Threshold)
                        {
                            float4 prod = diff(-direction2, color2); //diff
                            float3 ReflectDir = reflect(-direction2, color2); //specular
                            float ReflectDir1 = dot(ReflectDir,-direction2); //specular
                            finalValue = prod+float4(ReflectDir1, ReflectDir1, ReflectDir1, 1)*_Intensity;
                            break;
                        }
                        pos2 += direction2 * step_size;
                        // Make sure we are inside the box

                    }
                    return finalValue;

                }

                fixed4 frag(v2f i) : SV_Target
                {

                    float3 worldPosition = i.vertexLocal + float3(0.5f, 0.5f, 0.5f);
                    float3 viewDirection = normalize(ObjSpaceViewDir(float4(i.vertexLocal, 0.0f)));

                    return raymarch(worldPosition, viewDirection);
                    //return frag_SURF(i);

            }









            ENDCG
        }
        }
}
