Shader "Demo03/Npc (Bump Map + Specular Map)" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		[Normal]
		_BumpMap("Normalmap", 2D) = "bump" {}
		_BumpScale("Scale", Float) = 3.0
		_SpecularMap("Specular Map", 2D) = "white" {}
		_Shininess("Shininess", Range(0.03, 2)) = 0.08
		_Gloss("Gloss", Range(0.1, 10)) = 1
		[HideInInspector]
		_FlashColor("Flash Color", Color) = (1,1,1,1)
		[HideInInspector]
		_FlashIntensity("Flash Intensity", Range(0,1)) = 0
		[HideInInspector]
		_CharacterLightDir2("CharacterLightDir", Vector) = (0,0,0,0)
		[HideInInspector]
		_CharacterLightColor2("CharacterLightColor", Color) = (0,0,0,0)
	}

	SubShader{
		Tags{ "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" "AlphaFix"="Cutout" }
		LOD 200

		CGPROGRAM
		#include "GameCG.cginc"
		#pragma target 3.0
		#pragma surface surf CharacterMediumQuality addshadow noforwardadd noambient noshadow
		#pragma multi_compile __ PER_OBJECT_LIGHTING

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _SpecularMap;
		half _BumpScale;
		half _Shininess;
		half _Gloss;
		fixed4 _Color;
		fixed3 _FlashColor;
		fixed _FlashIntensity;
		float3 _CharacterLightDir;
		fixed3 _CharacterLightColor;

#ifdef PER_OBJECT_LIGHTING
		float3 _CharacterLightDir2;
		fixed3 _CharacterLightColor2;
#endif

		struct Input {
			float2 uv_MainTex;
			INTERNAL_DATA
		};

		void surf(Input IN, inout SurfaceOutput_CharacterMediumQuality o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			fixed4 s = tex2D(_SpecularMap, IN.uv_MainTex);
			fixed3 n = CustomUnpackScaleNormal(tex2D(_BumpMap, IN.uv_MainTex), _BumpScale);
			o.SpecColor = s.rgb;
			o.Normal = n;
			o.Albedo = c.rgb;
			o.Specular = _Shininess;
			o.Gloss = _Gloss;
			o.Alpha = c.a;
			o.FlashColor = _FlashColor;
			o.FlashIntensity = _FlashIntensity;

#ifdef PER_OBJECT_LIGHTING
			o.lightDir = normalize(_CharacterLightDir2);
			o.lightColor = _CharacterLightColor2;
#else
			o.lightDir = normalize(_CharacterLightDir);
			o.lightColor = _CharacterLightColor;
#endif
		}

		ENDCG
	}


	FallBack "Legacy Shaders/Transparent/Cutout/Diffuse"
}
