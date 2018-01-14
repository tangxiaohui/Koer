Shader "Unlit/Waterfall"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_ScrollXSpeed("X Scroll Speed",float) = 0
		_ScrollY("Y Speed",float) = 0
		_Brightness("Brightness",float) = 1
		_Color("Main Color",color) = (1,1,1,1)
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent" }

		LOD 100
		ZWrite Off
		ZTest Off
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
	float _ScrollXSpeed;
	fixed _Brightness;
	float _TimeLine;
	float _ScrollY;
	fixed4 _Color;


	v2f vert(appdata v)
	{
		v2f o;
		float xScrollValue = _ScrollXSpeed*_Time.y;
		float ySpeed =sin( _ScrollY*_Time.y);
		o.vertex = UnityObjectToClipPos(v.vertex);
		fixed2 scrolledUV = v.uv;
		scrolledUV += float2(ySpeed, xScrollValue);
		o.uv = TRANSFORM_TEX(scrolledUV, _MainTex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 col = tex2D(_MainTex, i.uv);
		return col*_Brightness*_Color;
	}

		ENDCG
	}
	}
}
