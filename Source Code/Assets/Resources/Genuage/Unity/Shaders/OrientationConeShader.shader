Shader "Genuage/OrientationConeShader"{
    //show values to edit in inspector
    Properties{
        _Color("Color", Color) = (0, 0, 0, 0)
        _MainTex("Texture", 2D) = "white" {}
        _Smoothness("Smoothness", Range(0, 1)) = 0.1
        _Metallic("Metalness", Range(0, 1)) = 0
        [HDR]_Emission("Emission", color) = (0,0,0)

        [HDR]_CutoffColor("Cutoff Color", Color) = (0,0,0,0)
    }

        SubShader{
            //the material is completely non-transparent and is rendered at the same time as the other opaque geometry
            //Tags{ "RenderType" = "Transparent" "Queue" = "Geometry"}
            Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
            // render faces regardless if they point towards the camera or away from it
            //Cull Off

            CGPROGRAM
            //the shader is a surface shader, meaning that it will be extended by unity in the background
            //to have fancy lighting and other features
            //our surface shader function is called surf and we use our custom lighting model
            //fullforwardshadows makes sure unity adds the shadow passes the shader might need
            //vertex:vert makes the shader use vert as a vertex shader function
            #pragma surface surf Standard fullforwardshadows alpha
            #pragma target 5.0
            #pragma multi_compile _ CLIPPING_PLANE

            sampler2D _MainTex;
            fixed4 _Color;

            half _Smoothness;
            half _Metallic;
            half3 _Emission;

            float4 _Plane;

            float4 _CutoffColor;

            uniform float4x4 _ControllerWorldToLocalMatrix;
            uniform float4x4 _BoxWorldToLocalMatrix;
            uniform float3 _BoxLocalScale;
            uniform float4 _ControllerWorldPosition;
            uniform float4 _ControllerPlaneNormal;


            //input struct which is automatically filled by unity
            struct Input {
                float2 uv_MainTex;
                float3 worldPos;
                float facing : VFACE;
                //float2 uv1 : TEXCOORD1; // colorindex, pointID

            };

            //the surface shader function which sets parameters the lighting function then uses
            void surf(Input i, inout SurfaceOutputStandard o) {

                #if defined(CLIPPING_PLANE)
                float dis = dot(_ControllerPlaneNormal, i.worldPos - _ControllerWorldPosition);
                //distance = distance + _Plane.w;
                //discard surface above plane
                clip(dis);
                //o.Emission = facing;

                #endif



                float facing = i.facing * 0.5 + 0.5;

                //normal color stuff
                //fixed4 col = tex2D(_MainTex, i.uv1);
                fixed4 col = tex2Dlod(_MainTex, float4(i.uv_MainTex.x, 0.5, 0, 0));
                col *= _Color;
                o.Albedo = col.rgb * facing;
                o.Metallic = _Metallic * facing;
                o.Smoothness = _Smoothness * facing;
                o.Emission = lerp(_CutoffColor, _Emission, facing);
                o.Alpha = col.a;

            }
            ENDCG
        }
            //FallBack "Standard" //fallback adds a shadow pass so we get shadows on other objects
}