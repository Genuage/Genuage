Shader "Genuage/VolumeRenderingSurface2"
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

                #define offset 0.001
                #define num_steps 1024

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
                fixed4 diff(float3 direction, float4 normal)
                {
                    float prod;
                    prod = dot(direction, normal);
                    if (prod < 0)
                        prod = 0;
                    fixed4 diff = fixed4(prod, prod, prod, 1) * (1, 0, 0, 1);
                    return diff;
                }
                fixed4 specular(float3 direction, float4 normal)
                {
                    float3 ReflectDir = reflect(-direction, normal); //specular
                    float ReflectDir1 = dot(ReflectDir, direction); //specular
                    if (ReflectDir1 < 0)
                        ReflectDir1 = 0;
                    fixed4 rez = fixed4(ReflectDir1, ReflectDir1, ReflectDir1, 1) * _Intensity;
                    return rez;
                }
                fixed4 raymarch(float3 position, float3 direction1)
                {
                    float3 direction = -direction1; //change dir
                    float3 pos2 = position + (-direction) * num_steps * step_size;
                    fixed4 finalValue = fixed4(0, 0, 0, 0);
                    for (int i = 0; i < num_steps; i++)
                    {
                        float t = i * step_size;
                        float3 currPos = pos2 + direction * t;
                        float4 color = tex3Dlod(_VolumeTex, float4(currPos.x, currPos.y, currPos.z, 0));
                        float4 color2 = tex3Dlod(_GradientTex, float4(currPos.x, currPos.y, currPos.z, 0));
                        if (currPos.x < 0.0f || currPos.x > 1.0f || currPos.y < 0.0f || currPos.y > 1.0f || currPos.z < 0.0f || currPos.z > 1.0f)
                            continue;
                        if (color.x > _Threshold)
                        {
                            fixed4 prod = diff(-direction, color2); //diff
                            fixed4 prod2 = specular(-direction, color2);//specular
                            finalValue = prod + prod2;
                            break;
                        }

                    }
                    return finalValue;



                }

                fixed4 frag(v2f i) : SV_Target
                {

                    float3 worldPosition = i.vertexLocal + float3(0.5f, 0.5f, 0.5f);
                    float3 viewDirection = normalize(ObjSpaceViewDir(float4(i.vertexLocal, 0.0f)));

                    return raymarch(worldPosition, viewDirection);

                }


                    ENDCG
                }
        }
}
