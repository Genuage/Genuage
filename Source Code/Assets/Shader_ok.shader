Shader "Genuage/Shade_ok"
{
    Properties
    {
        _VolumeTex("VolumeTex", 3D) = "" {}
        _GradientTex("GradientTex", 3D) = "" {}

        _Threshold("Threshold", Range(0.0, 1.0)) = 0.0


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

                #define STEPS 100
                #define STEP_SIZE 0.01
                #define MIN_DISTANCE 0.01

                sampler3D _VolumeTex;
                sampler3D _GradientTex;

                float _Threshold;


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
                    if (prod < 0)
                        prod = 0;
                    float4 diff = float4(prod,prod,prod,1) * (1, 1, 1, 1);
                    return diff;
                }
                fixed4 raymarch(float3 position, float3 direction)
                {

                    float4 finalValue = float4(0, 0, 0, 0);
                    for (int i = 0; i < 500; i++)
                    {
                        float4 color = tex3Dlod(_VolumeTex, float4(position.x, position.y, position.z, 0));
                        float4 color2 = tex3Dlod(_GradientTex, float4(position.x, position.y, position.z, 0));
                        if (position.x < 0.0f || position.x > 1.0f || position.y < 0.0f || position.y > 1.0f || position.z < 0.0f || position.z > 1.0f) // TODO: avoid branch?
                            continue;
                        if (color.x > _Threshold)
                        {
                            float4 prod = diff(-direction, color2); //diff
                            float3 ReflectDir = reflect(-direction, color2); //specular
                            ReflectDir = dot(ReflectDir,-direction); //specular
                            finalValue = prod;//+float4(ReflectDir.x, ReflectDir.y, ReflectDir.z, 1);
                            break;
                        }
                        position += direction * STEP_SIZE;
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
