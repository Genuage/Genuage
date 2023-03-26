// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Genuage/ThickLineShader"
{
    Properties
    {
        _UpperTimeLimit("UpperTimeLimit", Float) = 10.0
        _LowerTimeLimit("LowerTimeLimit", Float) = 0.0
        _ColorTex("Texture", 2D) = "white" {}

        _Color("Main Color", Color) = (1,1,1,1)
        _Thickness("Thickness",Range(0, 0.2)) = 0.05
    }
        SubShader
    {
        Pass
        {
            Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
            //ZWrite Off
            Cull Off
            Blend OneMinusDstColor One

            LOD 200

            CGPROGRAM
            #pragma target 5.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geo
            #pragma multi_compile _ CLIPPING_PLANE
            #pragma multi_compile _ POINT_HIDDEN
            //#pragma multi_compile _ TRAJECTORYID
            #pragma multi_compile _ COLOR_OVERRIDE

            #include "UnityCG.cginc"

            struct appdata
            //Mesh data, vertex position, vertex normal, UVs, tangents, vertex colors...
            //Structure to define what we want to grab from the Mesh
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 colors : COLOR;
                float2 uv1 : TEXCOORD1; // colorindex, pointID
                float2 uv2 : TEXCOORD2; //isSelected, isHidden, 0 for false
                float2 uv3 : TEXCOORD3; // trajectoryID, frame of apparition

                //float4 normal : NORMAL;
            };

            struct v2g
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                //float4 colors : COLOR;
                float2 uv1 : TEXCOORD1; // colorindex, pointID
                float2 uv2 : TEXCOORD2; //isSelected, isHidden, 0 for false
                float2 uv3 : TEXCOORD3; // trajectoryID, frame of apparition

            };

            struct g2f
            {
                float4 vertex: POSITION;
                //float4 vertex : SV_POSITION; //will be read as clip position on the camera
                float3 normal : NORMAL;
                float2 uv1 : TEXCOORD1; // colorindex, pointID
                float2 uv2 : TEXCOORD2; //isSelected, isHidden, 0 for false
                float2 uv3 : TEXCOORD3; // trajectoryID, frame of apparition

            };

            fixed4 _Color;
            float _Thickness;

            uniform sampler _ColorTex;

            uniform float4x4 _ControllerWorldToLocalMatrix;
            uniform float4x4 _BoxWorldToLocalMatrix;
            uniform float3 _BoxLocalScale;
            uniform float4 _ControllerWorldPosition;
            uniform float4 _ControllerPlaneNormal;


            uniform float _UpperTimeLimit;
            uniform float _LowerTimeLimit;


            v2g vert(appdata v)
            {
                v2g o;
                //o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex = v.vertex;
                o.normal = v.normal;
                o.uv1 = v.uv1;
                o.uv2 = v.uv2;
                o.uv3 = v.uv3;

                return o;
            }

            [maxvertexcount(4)]
            void geo(line v2g v[2], inout TriangleStream<g2f> ts)
            {
                
                float4 p1 = v[0].vertex;
                float4 p2 = v[1].vertex;
                //p1 = UnityObjectToClipPos(v[0].vertex);
                //p2 = UnityObjectToClipPos(v[1].vertex);

                float3 linedirection = normalize(p2-p1);
                float3 cameradirection = normalize(_WorldSpaceCameraPos - p1);
                float3 orthogonalvector = cross(linedirection, cameradirection);


                float4 offset = float4((_Thickness/100) * orthogonalvector, 1);
                //offset.x *= _ScreenParams.y / _ScreenParams.x;

                //offset1.x *= _ScreenParams.y / _ScreenParams.x;
                //offset2.x *= _ScreenParams.y / _ScreenParams.x;

                
                float4 o1 = p1 + offset;
                float4 o2 = p1 - offset;
                float4 o3 = p2 + offset;
                float4 o4 = p2 - offset;


                g2f g[4];

                g[0].vertex = UnityObjectToClipPos(o1);
                g[0].normal = v[0].normal;
                g[0].uv1 = v[0].uv1;
                g[0].uv2 = v[0].uv2;
                g[0].uv3 = v[0].uv3;

                g[1].vertex = UnityObjectToClipPos(o2);
                g[1].normal = v[0].normal;
                g[1].uv1 = v[0].uv1;
                g[1].uv2 = v[0].uv2;
                g[1].uv3 = v[0].uv3;

                g[2].vertex = UnityObjectToClipPos(o3);
                g[2].normal = v[1].normal;
                g[2].uv1 = v[1].uv1;
                g[2].uv2 = v[1].uv2;
                g[2].uv3 = v[1].uv3;

                g[3].vertex = UnityObjectToClipPos(o4);
                g[3].normal = v[1].normal;
                g[3].uv1 = v[1].uv1;
                g[3].uv2 = v[1].uv2;
                g[3].uv3 = v[1].uv3;

                ts.Append(g[0]);
                ts.Append(g[1]);
                ts.Append(g[2]);
                ts.Append(g[3]);
                
                /*
                float4 p1 = UnityObjectToClipPos(v[0].vertex);
                float4 p2 = UnityObjectToClipPos(v[1].vertex);

                //float2 dir = normalize(p2.xy - p1.xy);

                float2 direction = normalize((p2.xy / p2.z) - (p1.xy / p1.z));
                float2 normal = float2(-direction.y, direction.x);

                float4 offset1 = float4(normal * p1.z * _Thickness/2 , 0, 0);
                float4 offset2 = float4(normal * p2.z * _Thickness/2 , 0, 0);
                offset1.x *= _ScreenParams.y / _ScreenParams.x;
                offset2.x *= _ScreenParams.y / _ScreenParams.x;


                float4 o1 = p1 + offset1;
                float4 o2 = p1 - offset1;
                float4 o3 = p2 + offset2;
                float4 o4 = p2 - offset2;


                g2f g[4];

                g[0].vertex = o1;
                g[0].normal = v[0].vertex;
                g[0].uv1 = v[0].uv1;
                g[0].uv2 = v[0].uv2;
                g[0].uv3 = v[0].uv3;

                g[1].vertex = o2;
                g[1].normal = v[0].vertex;
                g[1].uv1 = v[0].uv1;
                g[1].uv2 = v[0].uv2;
                g[1].uv3 = v[0].uv3;

                g[2].vertex = o3;
                g[2].normal = v[1].vertex;
                g[2].uv1 = v[1].uv1;
                g[2].uv2 = v[1].uv2;
                g[2].uv3 = v[1].uv3;

                g[3].vertex = o4;
                g[3].normal = v[1].vertex;
                g[3].uv1 = v[1].uv1;
                g[3].uv2 = v[1].uv2;
                g[3].uv3 = v[1].uv3;

                ts.Append(g[0]);
                ts.Append(g[1]);
                ts.Append(g[2]);
                ts.Append(g[3]);
                */
            }

            fixed4 frag(g2f v) : COLOR
            {
                
                if (v.uv3.y > _UpperTimeLimit || v.uv3.y < _LowerTimeLimit || v.uv2.y > 0) {
                    discard; 

                }

                #if defined(CLIPPING_PLANE)
                float dis = dot(_ControllerPlaneNormal, mul(unity_ObjectToWorld, v.normal) - _ControllerWorldPosition);
                if (dis < 0) {
                    discard;
                }

                #endif
                




                float4 color;

                color = tex2Dlod(_ColorTex, float4(v.uv1.x, 0.5, 0, 0));


                if (v.uv2.x > 0) {
                    color = float4(1, 1, 1, 1); 
                }

                #if defined(COLOR_OVERRIDE)
                color = tex2Dlod(_ColorTex, float4(v.uv1.x, 0.5, 0, 0));

                #endif
                color.w = 0.5;
                return color;
            }
            ENDCG
        }
    }
}
