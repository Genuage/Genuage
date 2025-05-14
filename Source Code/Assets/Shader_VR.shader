

Shader "Genuage/VolumeRenderingSurface"
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
            #define offset 0.01
            #define num_steps 500

            sampler3D _VolumeTex;
            sampler3D _GradientTex;

            float _Threshold;
            float step_size = 1.732 / num_steps;
           

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

            v2f vert (appdata v)
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
                prod= dot(direction, normal);
                if ((prod<0))
                    prod = 0;
                float4 diff = float4(prod, prod, prod, 1) * (1, 0, 0, 1);
                return diff;
            }
         
            fixed4 raymarch(float3 position, float3 direction)
            {
                
                
                float4 finalValue = float4(0, 0, 0, 0);
                float3 pos2 = position + direction * 512 * step_size;
                for (int i = 0; i < num_steps; i++)
                {
                    position = position + offset * (direction); //added the offset
                    float4 color = tex3Dlod(_VolumeTex, float4(pos2.x, pos2.y, pos2.z, 0));
                    float4 color2 = tex3Dlod(_GradientTex, float4(pos2.x, pos2.y, pos2.z, 0));

                    if (pos2.x < 0.0f || pos2.x > 1.0f || pos2.y < 0.0f || pos2.y > 1.0f || pos2.z < 0.0f || pos2.z > 1.0f)
                        continue;

                    if (color.x > _Threshold) 
                    {
                        float4 prod = diff(direction, color2); //diff
                        float3 ReflectDir = reflect(direction, color2); //specular
                        ReflectDir = dot(ReflectDir,direction); //specular
                        finalValue = prod+float4(ReflectDir.x, ReflectDir.y, ReflectDir.z, 1);
                        break;
                    }                        
                    pos2 += direction * step_size;
                 
                    
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
