Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex("Texture",2D) = "white" {}

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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                 uint vertexId : SV_VertexID;
                float4 vertex : POSITION;
                float4 color : COLOR;
  
            };

            struct v2f
            {
                float4 color : COLOR;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
              
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
      
            v2f vert (appdata v, uint vertexID : SV_VertexID)
            {
                float4 colors[8];
                colors[0] = float4(1, 0, 0, 1);
                colors[1] = float4(0, 1, 0, 1);
                colors[2] = float4(0, 0, 1, 1);
                colors[3] = float4(1, 0, 1, 1);
                colors[4] = float4(1, 1, 0, 1);
                colors[5] = float4(0, 1, 0, 1);
                colors[6] = float4(0, 1, 1, 1);
                colors[7] = float4(1, 1, 1, 1);

            
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = colors[vertexID];
        
       
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                return i.color;
            }
            ENDCG
        }
    }
}
