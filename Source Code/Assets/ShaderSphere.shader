// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/ShaderSphere"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
         _Centre("Centre", float) = 0
         _Radius("Radius", float) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            float _Radius;
            float _Centre;
            // make fog work
            #pragma multi_compile_fog
            #define STEP_SIZE 0.01
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;    
                float3 wPos : TEXCOORD1;   
            };
       

            // Vertex function
            v2f vert(appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
            fixed4 raymarch(float3 position, float3 direction)
            {
                for (int i = 0; i < 100; i++)
                {  
                    if (distance(position, _Centre) < _Radius)
                        return fixed4(1, 0, 0, 1);

                    position += direction * STEP_SIZE;
                }
                return fixed4(0, 0, 0, 1); 
            }


            // Fragment function
            fixed4 frag(v2f i) : SV_Target
            {
                float3 worldPosition = i.wPos;
                float3 viewDirection = normalize(i.wPos - _WorldSpaceCameraPos);
                return raymarch(worldPosition, viewDirection);
            }
               
        ENDCG
        }
    }
}
