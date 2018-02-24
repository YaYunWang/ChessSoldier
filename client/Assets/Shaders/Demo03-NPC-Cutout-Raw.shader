Shader "Demo03/NPC (Cutout) (RAW)" {
	Properties {
		//[HideInInspector]
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
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
		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" "AlphaFix" = "Cutout" "ModelShow" = "NPC-Cutout" }
		LOD 200

		CGPROGRAM
		#include "GameCG.cginc"
		#pragma surface surf CharacterMediumQuality alphatest:_Cutoff addshadow noforwardadd noambient noshadow
		#pragma multi_compile __ PER_OBJECT_LIGHTING

		sampler2D _MainTex;
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
			o.SpecColor = c.rgb;//Luminance(c.rgb);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.FlashColor = _FlashColor;
			o.FlashIntensity = _FlashIntensity;
			o.Gloss = 1;

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
