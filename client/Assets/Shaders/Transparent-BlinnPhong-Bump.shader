Shader "Demo03/Transparent/BlinnPhong (Bump Map + Blend Map)" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "white" {}
		_BumpScale("Normal Scale", Float) = 3.0
		_BlendMap("Blend Map (R)", 2D) = "white" {}
		_SpecularColor("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess("Shininess", Range (0.03, 1)) = 0.078125
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Geometry+1" }
		LOD 300
		
		CGPROGRAM
		#include "GameCG.cginc"
		#pragma surface surf CustomBlinnPhong decal:blend noforwardadd

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;
		half _BumpScale;
		sampler2D _BlendMap;
		half _Shininess;
		fixed4 _SpecularColor;

		struct Input {
			float2 uv_MainTex;
//			float2 uv_BumpMap;
		};

		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutput_CustomBlinnPhong o) {
			// Albedo comes from a texture tinted by color
			fixed3 c = tex2D (_MainTex, IN.uv_MainTex).rgb * _Color.rgb;
			fixed3 b = tex2D(_BlendMap, IN.uv_MainTex).r * _Color.a;
			fixed3 n = CustomUnpackScaleNormal(tex2D(_BumpMap, IN.uv_MainTex), _BumpScale);
			o.Albedo = c.rgb;
			o.Normal = n;
			o.Specular = _Shininess;
			o.Gloss = _SpecularColor.a;
			o.Alpha = b;
			o.SpecColor = _SpecularColor.rgb;
		}
		ENDCG
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Geometry+1" }
		LOD 200
		
		CGPROGRAM
		#include "GameCG.cginc"
		#pragma surface surf Lambert decal:blend noforwardadd

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BlendMap;

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutput o) {
			fixed3 c = tex2D (_MainTex, IN.uv_MainTex).rgb * _Color.rgb;
			fixed3 b = tex2D(_BlendMap, IN.uv_MainTex).r * _Color.a;
			o.Albedo = c.rgb;
			o.Alpha = b;
		}
		ENDCG
	}
	Fallback "Transparent/Diffuse"
}
