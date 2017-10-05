Shader "Sprites/SpriteShadowV02"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		[PerRendererData]_MainTex("Sprite Texture", 2D) = "white" {}
	_Cutoff("Shadow alpha cutoff", Range(0,1)) = 0.5
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
	}
		SubShader
	{
		Tags
	{
		"Queue" = "Geometry"
		"RenderType" = "TransparentCutout"
	}
		LOD 0

		Cull Off

		CGPROGRAM
		// Lambert lighting model, and enable shadows on all light types
#pragma surface surf Lambert addshadow fullforwardshadows
#pragma vertex vert
		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		sampler2D _MainTex;
	fixed4 _Color;
	fixed _Cutoff;

	struct appdata_t
	{
		float4 vertex   : POSITION;
		float4 color    : COLOR;
		float2 texcoord : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f
	{
		float4 vertex   : SV_POSITION;
		fixed4 color : COLOR;
		float2 texcoord  : TEXCOORD0;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	struct Input
	{
		float2 uv_MainTex;
	}; 
	
	v2f vert(appdata_t IN)
	{
		v2f OUT;
		UNITY_SETUP_INSTANCE_ID(IN);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
		OUT.vertex = UnityObjectToClipPos(IN.vertex);
		OUT.texcoord = IN.texcoord;
		OUT.color = IN.color * _Color;
#ifdef PIXELSNAP_ON
		OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif

		return OUT;
	}
		//void surf(Input IN, inout SurfaceOutput o)
		//{
		//	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		//	o.Albedo = c.rgb;
		//	o.Alpha = c.a;
		//	clip(o.Alpha - _Cutoff);
		//}
	ENDCG
	}
		FallBack "Diffuse"
}
