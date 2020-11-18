/**
Copyright (c) 2020, 	Institut Curie, Institut Pasteur and CNRS
			Thomas BLanc, Mohamed El Beheiry, Jean Baptiste Masson and Bassam Hajj
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. All advertising materials mentioning features or use of this software
   must display the following acknowledgement:
   This product includes software developed by the Institut Curie, Insitut Pasteur and CNRS.
4. Neither the name of the Institut Curie, Insitut Pasteur and CNRS nor the
   names of its contributors may be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDER ''AS IS'' AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE
USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
**/

Shader "Genuage/UnlitLineShader"
{
    Properties
    {

		_UpperTimeLimit("UpperTimeLimit", Float) = 10.0
		_LowerTimeLimit("LowerTimeLimit", Float) = 0.0
        _ColorTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        //LOD 100

        Pass
        {
            CGPROGRAM

			//What's the name of vertex and fragment shaders'
			#pragma vertex vert
            #pragma fragment frag
			#pragma target 5.0
			#pragma multi_compile _ CLIPPING_PLANE
			#pragma multi_compile _ POINT_HIDDEN
			//#pragma multi_compile _ TRAJECTORYID
			#pragma multi_compile _ COLOR_OVERRIDE

			//Uses unity specific function(copy paste directly here)
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

            struct v2f
            //output of the vertex shader that goes into the fragment shader
			{

                float4 vertex : SV_POSITION; //will be read as clip position on the camera
				float3 normal : NORMAL;
				float2 uv1 : TEXCOORD1; // colorindex, pointID
				float2 uv2 : TEXCOORD2; //isSelected, isHidden, 0 for false
				float2 uv3 : TEXCOORD3; // trajectoryID, frame of apparition
            };

		
			
			uniform sampler _ColorTex;

			uniform float4x4 _ControllerWorldToLocalMatrix;
			uniform float4x4 _BoxWorldToLocalMatrix;
			uniform float3 _BoxLocalScale;
			uniform float4 _ControllerWorldPosition;
			uniform float4 _ControllerPlaneNormal;


			uniform float _UpperTimeLimit;
			uniform float _LowerTimeLimit;
			

            v2f vert (appdata_full v )
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); // transform input positions into clip positions
				o.normal = v.vertex;
				o.uv1 = v.texcoord1;
				o.uv2 = v.texcoord2;
				o.uv3 = v.texcoord3;

                return o;
            }

            float4 frag (v2f v) : COLOR
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

				return color;

				
            }
            ENDCG
        }
    }
}
