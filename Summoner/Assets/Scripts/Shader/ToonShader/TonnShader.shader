// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/TonnShader"
{
	Properties{
		_MainTex("Main Tex",2D) = "white" {}
		[Toggle(USEBUMP)] _Normaluse("Use NormalMap?", float) = 0
		_NormalMap("Bump",2D) = "white"{}
		_NormalFactor("NormalFactor",Range(1,10))=1
		_RimColor("_Rim Color",color) = (1,1,1,1)//边缘色
		_Outline("Thick of Outline",range(0,0.1)) = 0.02//粗细
		_Factor("Factor",range(0,1)) = 0.5
	    _Brightness("Brightness",range(0,3))=1//没灯光的情况下调节亮度
		//_Displacement("Displacement", Range(0, 1.0)) = 0 //曲面细分程度
	    [Toggle(USELIGHT)] _LightUse("Use Light?", float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Tags{ "LightMode" = "Always" }
			Cull Front
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 normal :TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Factor;
			float _Outline;
			float4 _RimColor;

			v2f vert(appdata_full v) {
				v2f o;
				float3 dir = normalize(v.vertex.xyz);
				float3 dir2 = v.normal;
				float D = dot(dir, dir2);
				dir = dir*sign(D);
				dir = dir*_Factor + dir2*(1 - _Factor);
				v.vertex.xyz += dir*_Outline;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal =float4( v.normal.x, v.normal.y, v.normal.z,0.0);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			 
			fixed4 frag (v2f i) : SV_Target 
			{ 
				float3 dir = normalize(i.vertex.xyz);
				float3 dir2 = i.normal;
				float D = dot(dir, dir2);
				dir = dir*sign(D);
				dir = dir*_Factor + dir2*(1 - _Factor);
				i.vertex.xyz += dir*_Outline;
				float4 c = _RimColor;
				return c;
			}
			ENDCG 
		}

		Pass
		{
				Tags{ "LightMode" = "ForwardBase" }
				Cull Back
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				#pragma shader_feature USELIGHT
				#pragma shader_feature USEBUMP
				float4 _LightColor0;
				float _Steps;
				//float _ToonEffect;
				sampler2D _MainTex;
				sampler2D _NormalMap;
				float4 _MainTex_ST;
				float4 _NormalMap_ST;
				float _NormalFactor;
				float _LightUse;
				float _Brightness;
				float _Displacement;

				struct v2f
				{
					float4 pos:SV_POSITION;
					float3 lightDir:TEXCOORD0;
					float3 viewDir:TEXCOORD1;
					float3 normal:TEXCOORD2;
					float2 uv :TEXCOORD3;
					float2 uv2 : TEXCOORD4;//法线uv
				};

				v2f vert(appdata_full v)
				{
					v2f o;
					//float d = tex2Dlod(_MainTex, float4(v.texcoord.xy, 0, 0)).r * _Displacement;
					o.normal = v.normal;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.lightDir = ObjSpaceLightDir(v.vertex);
					o.viewDir = ObjSpaceViewDir(v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.uv2 = TRANSFORM_TEX(v.texcoord, _NormalMap);
					return o;
				}
				float4 frag(v2f i) :COLOR
				{
					float3 N = normalize(i.normal);
					float3 normalTex = UnpackNormal(tex2D(_NormalMap, i.uv2))*_NormalFactor;
					float3 viewDir = normalize(i.viewDir);
					float3 lightDir = normalize(i.lightDir);
					//#if USEBUMP
						float diff = max(0, dot(normalTex, i.lightDir));
					//#else
						float diff1 = max(0, dot(N, i.lightDir));
					//#endif
						diff = diff + diff1;
					diff = (diff + 1) / 2;
					diff = smoothstep(0,1,diff);
					fixed4 col;
					float toon = floor(diff*1) / _Steps;//把颜色做离散化处理，把diffuse颜色限制在_Steps种（_Steps阶颜色），简化颜色，这样的处理使色阶间能平滑的显示
					diff = lerp(diff, toon, 1);//调节卡通与现实的比重
					#if USELIGHT
						 col = tex2D(_MainTex, i.uv)*(diff)*_LightColor0;
					#else
						 col = tex2D(_MainTex, i.uv);
					#endif
					return col*_Brightness*1.5;
				}
				ENDCG
		  }
		 Pass
		 {
				Tags{ "LightMode" = "Always" }
				Cull Front
				ZWrite On
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"
				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
					float4 normal :TEXCOORD1;
				};
				sampler2D _MainTex;
				float4 _MainTex_ST;

				v2f vert(appdata_full v) {
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.normal = float4(v.normal.x, v.normal.y, v.normal.z,0.0);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					float4 c = tex2D(_MainTex, i.uv);
					return c;
				}
				ENDCG
			}
	}
}
