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
		Tags{ "Queue" = "Transparent" } // ��ʹ������ᵼ�³��ֱ���͸�����ֲ���ʾ�����

		LOD 100

		GrabPass{} // ���Ƶ�ǰ����Ļͼ�� _GrabTexture ��

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
		half4 screenuv : TEXCOORD2; // ��ǰ��Ļͼ���UV����
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;
	float _ceshiyongde;
	float4 _Color;
	sampler2D _GrabTexture; // ��ǰ��Ļͼ��


	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		UNITY_TRANSFER_FOG(o,o.vertex);
		o.screenuv = ComputeGrabScreenPos(o.vertex); // ��ȡ��ǰ��Ļͼ���UV����
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{


		fixed4 colour = tex2D(_GrabTexture, float2(i.screenuv.x, i.screenuv.y)); // ��ȡ��ǰ��Ļͼ�����ɫ

																				 // sample the texture
	fixed4 col = tex2D(_MainTex, i.uv);

	fixed4 endd = col  *  _Color; // ADD ���ģʽ

								// apply fog
	UNITY_APPLY_FOG(i.fogCoord, endd);
	return endd;
	}
		ENDCG
	}
	}
}