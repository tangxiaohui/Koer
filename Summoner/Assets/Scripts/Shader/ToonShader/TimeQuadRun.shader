Shader "Unlit/TimeQuadRun"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MaskTex("MaskTexture",2D) = "white"{}
		_RimTex("RimTexture",2D) = "white" {}
		_RotateSpeed("RotateSpeed",float) = 1
		_Color("MainColor",Color)=(1,1,1,1)

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15
	}
	SubShader
	{
		Tags { "Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True" }
			ZWrite Off
			ZTest[unity_GUIZTestMode]
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask[_ColorMask]
		Stencil
		{ 
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;			
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float2 alphaUv :TEXCOORD1;
				float2 rimUv :TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _MaskTex;
			float4 _MaskTex_ST;
			sampler2D _RimTex;
			float4 _Rim_ST;
			float _RotateSpeed;
			fixed4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.alphaUv = TRANSFORM_TEX(v.uv, _MaskTex);
				o.uv.xy = float2(o.uv.x - 0.5, o.uv.y - 0.5);
				float s = sin(_RotateSpeed * _Time.y);
				float c = cos(_RotateSpeed * _Time.y);
				float2x2 rotationMatrix = float2x2(c, s, -s, c);
				rotationMatrix *= 0.5;
				rotationMatrix += 0.5;
				rotationMatrix = rotationMatrix * 2 - 1;
				o.uv = mul(o.uv, rotationMatrix);
				float s1 = sin(_RotateSpeed*5 * _Time.y);
				float c1 = cos(_RotateSpeed*5 * _Time.y);
				float2x2 rotationMatrix1 = float2x2(c1, s1, -s1, c1);
				rotationMatrix1 *= 0.5;
				rotationMatrix1 += 0.5;
				rotationMatrix1 = rotationMatrix1 * 2 - 1;
				o.rimUv = mul(o.uv, rotationMatrix1);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float timeZ = _Time.z;
				fixed4 final =fixed4(0,0,0,0);
				fixed4 mainTex = tex2D(_MainTex, i.uv);// *clamp((abs(sin(timeZ))), 0, 1);
				mainTex.a = tex2D(_MaskTex, i.alphaUv).r;
			/*	fixed4 rimTex = tex2D(_RimTex, i.rimUv);
				fixed2 rimUv = fixed2(i.alphaUv.x, i.alphaUv.y);
				rimTex.a = tex2D(_MaskTex, rimUv).r;*/
				final = mainTex;// +rimTex;
				return final*_Color;
			}
			ENDCG
		}
	}
}
