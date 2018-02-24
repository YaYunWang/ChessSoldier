Shader "Demo03/Water" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		[Normal]
		_BumpMap("Normal Map", 2D) = "black" {}
		_NoiseMap ("Noise Map", 2D) = "black" {}
		_SpecularColor("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess("Shininess", Range(0.03, 30)) = 0.078125
		_Gloss("Gloss", Range(0, 5)) = 1
		_FlowVec("Flow Vector", Vector) = (0.5, 0.5, 0, 0)
	}

	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 300
		
		CGPROGRAM

		#include "GameCG.cginc"
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Water noshadow alpha noforwardadd

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _NoiseMap;
		float _Shininess;
		fixed4 _SpecularColor;
		float _Gloss;
		float4 _FlowVec;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float2 uv_NoiseMap;
			float3 viewDir;
		};

		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutput_Water o) {
			IN.uv_NoiseMap -= _Time.x * (float2)_FlowVec.zw;
			fixed4 noise = tex2D(_NoiseMap, IN.uv_NoiseMap);
			float2 offset = noise.rg;
			IN.uv_MainTex += _Time.x * (float2)_FlowVec.xy + offset;
			IN.uv_BumpMap += _Time.x * (float2)_FlowVec.xy + offset;
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			fixed3 n = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Albedo = c.rgb;
			o.Normal = n;
			o.Alpha = c.a;
			o.Specular = _Shininess;
			o.Gloss = _Gloss;
			o.SpecColor = _SpecularColor.rgb;
		}
		ENDCG
	}

	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 200
		
		CGPROGRAM

		#include "GameCG.cginc"
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf WaterLowQuality noshadow alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _NoiseMap;
		float _Shininess;
		fixed4 _SpecularColor;
		float _Gloss;
		float4 _FlowVec;

		struct Input {
			float2 uv_MainTex;
			float2 uv_NoiseMap;
			float3 viewDir;
		};

		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutput_Water o) {
			IN.uv_NoiseMap -= _Time.x * (float2)_FlowVec.zw;
			fixed4 noise = tex2D(_NoiseMap, IN.uv_NoiseMap);
			float2 offset = noise.rg;
			IN.uv_MainTex += _Time.x * (float2)_FlowVec.xy + offset;
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}

	FallBack "Transparent/Diffuse"
}
