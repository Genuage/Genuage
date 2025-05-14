
Shader "Genuage/VolumeRenderingSurface4"
{
	Properties
	{
		_VolumeTex("VolumeTex", 3D) = "" {}
		_GradientTex("GradientTex", 3D) = "" {}
		_ShaderTex("ShaderTex", 2D) = "" {}


		_Threshold("Threshold", Range(0.0, 1.0)) = 0.0
		_Intensity("Intensity", Range(0.0, 1.0)) = 0.0
		_Transparency("Transparency", Range(0.0, 1.0)) = 0.0



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



				float _Threshold, _Intensity,_Transparency;


		
				sampler3D _VolumeTex;
				sampler3D _GradientTex;
				sampler2D _ShaderTex;



				float _MinVal;
				float _MaxVal;



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



				float3 calculateLighting(float3 col, float3 normal, float3 lightDir, float3 eyeDir, float specularIntensity)
				{
					float ndotl = max(lerp(0.0f, 1.5f, dot(normal, lightDir)), 0.5f); // modified, to avoid volume becoming too dark
					float3 diffuse = ndotl * col;
					float3 v = eyeDir;
					float3 r = normalize(reflect(-lightDir, normal));
					float rdotv = max(dot(r, v), 0.0);
					float3 specular = pow(rdotv, 32.0f) * float3(1.0f, 1.0f, 1.0f) * specularIntensity;
					return diffuse + specular;
				}




				
				fixed4 raymarch(float3 position, float3 raydirection, float3 lightdirection)
				{
				
					float num_steps = 512;
					float step_size = 1.732f / num_steps;
					float3 pos2 = position + (raydirection) * num_steps * step_size;
					float4 finalValue = float4(0, 0, 0, 0);

					
					for (int i = 0; i < num_steps; i++)
					{
						float t = i * step_size;
						float3 currPos = pos2 - raydirection * t;
						float4 color = tex3Dlod(_VolumeTex, float4(currPos.x, currPos.y, currPos.z, 0));
						
						float4 gradient = tex3Dlod(_GradientTex, float4(currPos.x, currPos.y, currPos.z, 0));
						
						if (currPos.x < 0.0f || currPos.x > 1.0f || currPos.y < 0.0f || currPos.y > 1.0f || currPos.z < 0.0f || currPos.z > 1.0f)
							continue;
						
						float4 color_transfer = tex2Dlod(_ShaderTex, float4(color.x, 0, 0, 0));
						finalValue.rgb = calculateLighting(float3(1,1,1), gradient, lightdirection, raydirection, 0.3f);
							

						finalValue.a = color_transfer.r + (1 - color_transfer.r)  * finalValue.a;
						finalValue.a = finalValue.a * _Transparency;
						if (finalValue.a > 1)
						{
							break;
						}
							



					}
					return finalValue;
				}



				fixed4 frag(v2f i) : SV_Target
				{



					float3 worldPosition = i.vertexLocal + float3(0.5f, 0.5f, 0.5f);

					float3 lightDirection = normalize(ObjSpaceViewDir(float4(float3(0.0f, 0.0f, 0.0f), 0.0f)));

					float3 rayDirection = normalize(ObjSpaceViewDir(float4(i.vertexLocal, 0.0f)));

					
					return raymarch(worldPosition, rayDirection, lightDirection);



				}
				ENDCG
			}
		}
}