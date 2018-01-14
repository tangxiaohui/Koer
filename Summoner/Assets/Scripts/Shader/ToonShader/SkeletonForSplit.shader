Shader "Unlit/SkeletonForSplit"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
	}
		SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100
		Blend SrcAlpha  OneMinusSrcAlpha
		Zwrite off
		ZTest off
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
	fixed _Cutoff;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 alpha;	fixed4 color;
	color = tex2D(_MainTex, float2(i.uv.x, i.uv.y / 2));	alpha = tex2D(_MainTex, float2(i.uv.x, 0.5 + i.uv.y / 2));
	color.a = alpha.r;	clip(color.a - _Cutoff);	return color;
	}
		ENDCG
	}
	}
}
