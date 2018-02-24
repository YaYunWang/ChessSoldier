Shader "Demo03/Opaque/BlinnPhong (Bump Map)" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "white" {}
		_BumpScale("Scale", Float) = 3.0
		_SpecularColor("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess("Shininess", Range (0.03, 1)) = 0.078125
		_Gloss("Gloss", Range(0.1, 10)) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 400
		
		CGPROGRAM
		#include "GameCG.cginc"
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf CustomBlinnPhong fullforwardshadows noforwardadd

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;
		half _BumpScale;
		half _Shininess;
		fixed4 _SpecularColor;
		half _Gloss;

		struct Input {
			float2 uv_MainTex;
//			float2 uv_BumpMap;
		};

		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutput_CustomBlinnPhong o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed3 n = CustomUnpackScaleNormal(tex2D(_BumpMap, IN.uv_MainTex), _BumpScale);
			o.Albedo = c.rgb;
			o.Normal = n;
			o.Specular = _Shininess;
			o.Gloss = _Gloss;
			o.Alpha = c.a;
			o.SpecColor = _SpecularColor.rgb;
		}
		ENDCG
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#include "GameCG.cginc"
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert fullforwardshadows noforwardadd

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}

	FallBack "Diffuse"
}
