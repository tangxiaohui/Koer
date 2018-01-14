Shader "Unlit/BackGroundShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_ScrollXSpeed("X Scroll Speed",float) = 0
		_Brightness("Brightness",float) = 1
	}

		SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent" }

		LOD 100
		//Cull back
		//不写入深度缓冲,为了不遮挡住其他物体
		ZWrite Off
		ZTest Off
		//选取Alpha混合方式
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma shader_feature ISSTOP
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
	fixed _ScrollXSpeed;
	fixed _Brightness;
	float _TimeLine;


	v2f vert(appdata v)
	{
		v2f o;
		fixed xScrollValue = _ScrollXSpeed;
		o.vertex = UnityObjectToClipPos(v.vertex);
		fixed2 scrolledUV = v.uv;
		scrolledUV += fixed2(_ScrollXSpeed, 0);
		o.uv = TRANSFORM_TEX(scrolledUV, _MainTex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 col = tex2D(_MainTex, i.uv);
		return col*_Brightness;
	}

		ENDCG
	}
	}
}
