Shader "Demo03/Opaque/Alternative/See Through/Lambert Emission" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		[NoScaleOffset]
		_EmissionTex ("Emission (RGB)", 2D) = "black" {}
		_EmissionPower ("Emission Power", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#include "GameCG.cginc"
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert addshadow noforwardadd

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _EmissionTex;
		float _EmissionPower;

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			fixed3 e = tex2D(_EmissionTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Emission = e * _EmissionPower;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
