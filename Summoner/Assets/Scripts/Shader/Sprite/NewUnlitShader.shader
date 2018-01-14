Shader "Unlit/NewUnlitShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_ceshiyongde("ceshiyongde",float) = 1
		_Color("Color", Color) = (0.5,0.5,0.5,0.5)
	}

		SubShader
	{
		//Tags { "RenderType"="Opaque" }
		Blend SrcAlpha  OneMinusSrcAlpha
		Tags{ "Queue" = "Transparent" } // 不使用这个会导致出现背景透明部分不显示的情况

		LOD 100

		GrabPass{} // 绘制当前的屏幕图像到 _GrabTexture 里

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
		// make fog work
#pragma multi_compile_fog

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		UNITY_FOG_COORDS(1)
			float4 vertex : SV_POSITION;
		half4 screenuv : TEXCOORD2; // 当前屏幕图像的UV坐标
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;
	float _ceshiyongde;
	float4 _Color;
	sampler2D _GrabTexture; // 当前屏幕图像


	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		UNITY_TRANSFER_FOG(o,o.vertex);
		o.screenuv = ComputeGrabScreenPos(o.vertex); // 获取当前屏幕图像的UV坐标
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{


		fixed4 colour = tex2D(_GrabTexture, float2(i.screenuv.x, i.screenuv.y)); // 获取当前屏幕图像的颜色

																				 // sample the texture
	fixed4 col = tex2D(_MainTex, i.uv);

	fixed4 endd = col  *  _Color; // ADD 混合模式

								// apply fog
	UNITY_APPLY_FOG(i.fogCoord, endd);
	return endd;
	}
		ENDCG
	}
	}
}