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

Shader "ViSP/Sprite Shader"
{
	Properties
	{
		_UpperTimeLimit("UpperTimeLimit", Float) = 10.0
		_LowerTimeLimit("LowerTimeLimit", Float) = 0.0

		_SpriteTex("Texture", 2D) = "white" {}
		_ColorTex("Texture", 2D) = "white" {}

		_Size("Size", Range(0, 0.05)) = 0.01
		//_Color("Color", Color) = (.34, .85, .92, 1)
	}

	SubShader
	{
		Pass {
			Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
			ZWrite Off
			Cull Off
			Blend OneMinusDstColor One

			LOD 200

			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vertex_shader
			#pragma fragment fragment_shader
			#pragma geometry geometry_shader
			#pragma multi_compile _ CLIPPING_PLANE
			#pragma multi_compile _ FIXED_CLIPPING_PLANE_1
						#pragma multi_compile _ FIXED_CLIPPING_PLANE_2

						#pragma multi_compile _ FIXED_CLIPPING_PLANE_3

						#pragma multi_compile _ FIXED_CLIPPING_PLANE_4

						#pragma multi_compile _ FIXED_CLIPPING_PLANE_5

						#pragma multi_compile _ FIXED_CLIPPING_PLANE_6

						#pragma multi_compile _ FIXED_CLIPPING_PLANE_7

						#pragma multi_compile _ FIXED_CLIPPING_PLANE_8

						#pragma multi_compile _ FIXED_CLIPPING_PLANE_9

						#pragma multi_compile _ FIXED_CLIPPING_PLANE_10

			#pragma multi_compile _ COLORMAP_REVERSED
			#pragma multi_compile _ FREE_SELECTION
			#pragma multi_compile _ POINTSIZE

			#include "UnityCG.cginc" 

			struct geometry_input
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1; // colorindex, pointID
				float2 uv2 : TEXCOORD2; //isSelected, isHidden, 0 for false
				float2 uv3 : TEXCOORD3; // trajectoryID, frame of apparition, Size, unused

				float4 color : COLOR;

			};

			struct fragment_input
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1; // colorindex, pointID
				float2 uv2 : TEXCOORD2; //isSelected, isHidden, 0 for false
				float2 uv3 : TEXCOORD3; // trajectoryID, frame of apparition, Size, unused

				float4 color : COLOR0;
			};


			float _Size;
			Texture2D _SpriteTex;
			SamplerState sampler_SpriteTex;

			uniform sampler _ColorTex;

			float4 _SpherePosition;
			float _SphereRadius;

			float4x4 _ControllerWorldToLocalMatrix;
			float4x4 _BoxWorldToLocalMatrix;
			float3 _BoxLocalScale;

			float4 _ControllerFixedLocalPosition1;
			float4 _ControllerFixedPlaneNormal1;
			float4 _ControllerFixedLocalPosition2;
			float4 _ControllerFixedPlaneNormal2;
			float4 _ControllerFixedLocalPosition3;
			float4 _ControllerFixedPlaneNormal3;
			float4 _ControllerFixedLocalPosition4;
			float4 _ControllerFixedPlaneNormal4;
			float4 _ControllerFixedLocalPosition5;
			float4 _ControllerFixedPlaneNormal5;
			float4 _ControllerFixedLocalPosition6;
			float4 _ControllerFixedPlaneNormal6;
			float4 _ControllerFixedLocalPosition7;
			float4 _ControllerFixedPlaneNormal7;
			float4 _ControllerFixedLocalPosition8;
			float4 _ControllerFixedPlaneNormal8;
			float4 _ControllerFixedLocalPosition9;
			float4 _ControllerFixedPlaneNormal9;
			float4 _ControllerFixedLocalPosition10;
			float4 _ControllerFixedPlaneNormal10;


			float _UpperTimeLimit;
			float _LowerTimeLimit;
			float4 _ControllerWorldPosition;
			float4 _ControllerPlaneNormal;

			// Vertex Shader
			geometry_input vertex_shader(appdata_full v)
			{
				geometry_input output = (geometry_input)0;

				output.vertex = v.vertex;
				output.normal = v.normal;
				output.uv = v.texcoord.xy;
				output.uv1 = v.texcoord1;
				output.color = v.color;
				output.uv2 = v.texcoord2;
				output.uv3 = v.texcoord3;

				return output;
			}
	
			// Geometry Shader
			[maxvertexcount(4)]
			void geometry_shader(point geometry_input p[1], inout TriangleStream<fragment_input> stream)
			{
				float3 up = UNITY_MATRIX_IT_MV[1].xyz;
				float3 look = _WorldSpaceCameraPos - p[0].vertex;
				look.y = 0;
				look = normalize(look);
				float3 right = UNITY_MATRIX_IT_MV[0].xyz;

				float3 forward = UNITY_MATRIX_IT_MV[2].xyz;


				// float half_size = 0.5f * _Size * p[0].uv1.x;
				float half_size_x = 0.5f * _Size;
				float half_size_y = 0.5f * _Size;
				float half_size_z = 0.5f * _Size;

				#if defined(POINTSIZE)
				half_size_x = 0.5f * _Size * p[0].uv.x;// *p[0].uv1.x;
				half_size_y = 0.5f * _Size * 0;
				half_size_z = 0.5f * _Size * p[0].uv.x;

				#endif


				float4 vertices[4];
				vertices[0] = float4(p[0].vertex + (half_size_x * right) - (half_size_z * up), 1.0f);
				vertices[1] = float4(p[0].vertex + (half_size_x * right) + (half_size_z * up), 1.0f);
				vertices[2] = float4(p[0].vertex - (half_size_x * right) - (half_size_z * up), 1.0f);
				vertices[3] = float4(p[0].vertex - (half_size_x * right) + (half_size_z * up), 1.0f);




				fragment_input output = (fragment_input)0;

				output.vertex = UnityObjectToClipPos(vertices[0]);
				output.normal = p[0].vertex;
				output.uv = float2(1.0f, 0.0f);
				output.color = p[0].color;
				output.uv1 = p[0].uv1;
				output.uv2 = p[0].uv2;
				output.uv3 = p[0].uv3;

				stream.Append(output);

				output.vertex = UnityObjectToClipPos(vertices[1]);
				output.normal = p[0].vertex;
				output.uv = float2(1.0f, 1.0f);
				output.color = p[0].color;
				output.uv1 = p[0].uv1;
				output.uv2 = p[0].uv2;
				output.uv3 = p[0].uv3;
				stream.Append(output);

				output.vertex = UnityObjectToClipPos(vertices[2]);
				output.normal = p[0].vertex;
				output.uv = float2(0.0f, 0.0f);
				output.color = p[0].color;
				output.uv1 = p[0].uv1;
				output.uv2 = p[0].uv2;
				output.uv3 = p[0].uv3;
				stream.Append(output);

				output.vertex = UnityObjectToClipPos(vertices[3]);
				output.normal = p[0].vertex;
				output.uv = float2(0.0f, 1.0f);
				output.color = p[0].color;
				output.uv1 = p[0].uv1;
				output.uv2 = p[0].uv2;
				output.uv3 = p[0].uv3;
				stream.Append(output);



			}

			// Fragment Shader
			float4 fragment_shader(fragment_input input) : COLOR
			{
				if (input.uv3.y > _UpperTimeLimit || input.uv3.y < _LowerTimeLimit || input.uv2.y > 0) {
					discard;
				}



				#if defined(CLIPPING_PLANE)
				//NEW TRY WITH DOT CALCULUS AND PLANE	
				float dis = dot(_ControllerPlaneNormal, mul(unity_ObjectToWorld, input.normal) - _ControllerWorldPosition);
				if (dis < 0) {
					discard;
				}
				#endif

				#if defined(FIXED_CLIPPING_PLANE_1)
				float dis1 = dot(_ControllerFixedPlaneNormal1, mul(unity_ObjectToWorld, input.normal) - _ControllerFixedLocalPosition1);
				if (dis1 < 0) {
					discard;
				}
				#endif

				
				#if defined(FIXED_CLIPPING_PLANE_2)
				float dis2 = dot(_ControllerFixedPlaneNormal2, mul(unity_ObjectToWorld, input.normal) - _ControllerFixedLocalPosition2);
				if (dis2 < 0) {
					discard;
				}
				#endif


				#if defined(FIXED_CLIPPING_PLANE_3)
				float dis3 = dot(_ControllerFixedPlaneNormal3, mul(unity_ObjectToWorld, input.normal) - _ControllerFixedLocalPosition3);
				if (dis3 < 0) {
					discard;
				}
				#endif


				#if defined(FIXED_CLIPPING_PLANE_4)
				float dis4 = dot(_ControllerFixedPlaneNormal4, mul(unity_ObjectToWorld, input.normal) - _ControllerFixedLocalPosition4);
				if (dis4 < 0) {
					discard;
				}
				#endif


				#if defined(FIXED_CLIPPING_PLANE_5)
				float dis5 = dot(_ControllerFixedPlaneNormal5, mul(unity_ObjectToWorld, input.normal) - _ControllerFixedLocalPosition5);
				if (dis5 < 0) {
					discard;
				}
				#endif


				#if defined(FIXED_CLIPPING_PLANE_6)
				float dis6 = dot(_ControllerFixedPlaneNormal6, mul(unity_ObjectToWorld, input.normal) - _ControllerFixedLocalPosition6);
				if (dis6 < 0) {
					discard;
				}
				#endif


				#if defined(FIXED_CLIPPING_PLANE_7)
				float dis7 = dot(_ControllerFixedPlaneNormal7, mul(unity_ObjectToWorld, input.normal) - _ControllerFixedLocalPosition7);
				if (dis7 < 0) {
					discard;
				}
				#endif


				#if defined(FIXED_CLIPPING_PLANE_8)
				float dis8 = dot(_ControllerFixedPlaneNormal8, mul(unity_ObjectToWorld, input.normal) - _ControllerFixedLocalPosition8);
				if (dis8 < 0) {
					discard;
				}
				#endif


				#if defined(FIXED_CLIPPING_PLANE_9)
				float dis9 = dot(_ControllerFixedPlaneNormal9, mul(unity_ObjectToWorld, input.normal) - _ControllerFixedLocalPosition9);
				if (dis9 < 0) {
					discard;
				}
				#endif


				#if defined(FIXED_CLIPPING_PLANE_10)
				float dis10 = dot(_ControllerFixedPlaneNormal10, mul(unity_ObjectToWorld, input.normal) - _ControllerFixedLocalPosition10);
				if (dis10 < 0) {
					discard;
				}
				#endif

				float index = input.uv1.x;

				#if defined(COLORMAP_REVERSED)
				index = 1 - index;
				#endif

				float4 color;



				color = tex2Dlod(_ColorTex, float4(index, 0.5, 0, 0));




				if (input.uv2.x > 0) {
					color = float4(0, 0.5, 0, 1);
				}

				#if defined(FREE_SELECTION)
				float dist = distance(mul(unity_ObjectToWorld, input.normal).xyz, _SpherePosition.xyz);
				if (dist < _SphereRadius / 2) {
					color = float4(1, 1, 1, 1);
				}
				#endif


				//#if defined(CLIPPING_PLANE)
				//return _SpriteTex.Sample(sampler_SpriteTex, input.uv) * color * dis;

				//#else
				return _SpriteTex.Sample(sampler_SpriteTex, input.uv) * color;
				//#endif
				//return _SpriteTex.Sample(sampler_SpriteTex, input.uv) * input.color;
			}

			ENDCG
		}
	}
}
