Shader "Unlit/CloudShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_otherTex1("Texture1",2D) = "white" {}
		_otherTex2("Texture2",2D) = "white" {}
		_otherTex3("Texture3",2D) = "white" {}
		_alpha("TextureAlpha",float) = 1
		_Speed("Speed", float) = 1
	    _Scale("Scale ",float) = 1
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100
		Blend SrcAlpha  OneMinusSrcAlpha
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
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _otherTex1;
			float4 _otherTex1_ST;
			sampler2D _otherTex2;
			float4 _otherTex2_ST;
			sampler2D _otherTex3;
			float4 _otherTex3_ST;
			float _Speed;
			float _Scale;
			float _alpha;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half2 uv = i.uv;
				half r = sqrt(uv.x*uv.x + uv.y*uv.y);
				half z = cos(_Scale*r + _Time[1] * _Speed) / 1;
				half x = sin(_Scale*r + _Time[1] * _Speed) / 1;
				fixed4 col = tex2D(_MainTex, i.uv+float2(x/3,z/5))*_alpha;
				//col.a *= saturate(sin((i.uv.y + 0.2)*3.14));
				return col;
			}
			ENDCG
		}
	}
}
