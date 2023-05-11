Shader "GBPalette"
{
	Properties
	{
		_RenderTexture("RenderTexture", 2D) = "white" {}
		_Palette("Palette", 2D) = "white" {}
		_Fade("Fade", Range( 0 , 5)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Palette;
		uniform sampler2D _RenderTexture;
		uniform float4 _RenderTexture_ST;
		uniform float _Fade;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_RenderTexture = i.uv_texcoord * _RenderTexture_ST.xy + _RenderTexture_ST.zw;
			float lerpResult4 = lerp( tex2D( _RenderTexture, uv_RenderTexture ).r , 0.0 , ( 1.0 - _Fade ));
			float2 appendResult3 = (float2(lerpResult4 , lerpResult4));
			o.Emission = tex2D( _Palette, appendResult3 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}
