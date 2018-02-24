Shader "Demo03/Weapon" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		[Normal]
		_BumpMap("Normalmap", 2D) = "bump" {}
		_BumpScale("Scale", Float) = 3.0
		_SpecularMap("Specular Map", 2D) = "white" {}
		_Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
		_Shininess("Shininess", Range(0.03, 2)) = 0.08
		_Gloss("Gloss", Range(0.1, 10)) = 1
		_RimLightIntensity("RimLightIntensity", Range(0, 3)) = 1
		_GlowTex("Glow Texure", 2D) = "black" {}
		_GlowColor("Glow Color", Color) = (1,1,1,1)
		[HideInInspector]
		_FlashColor("Flash Color", Color) = (1,1,1,1)
		[HideInInspector]
		_FlashIntensity("Flash Intensity", Range(0,1)) = 0
		[HideInInspector]
		_CharacterLightDir2("CharacterLightDir", Vector) = (0,0,0,0)
		[HideInInspector]
		_CharacterLightColor2("CharacterLightColor", Color) = (0,0,0,0)
	}
	SubShader {
		Tags { "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" "AlphaFix" = "Cutout" }
		LOD 500
		
		CGPROGRAM
		#include "GameCG.cginc"
		
		#pragma target 3.0
		#pragma surface surf CharacterVeryHighQuality alphatest:_Cutoff addshadow noforwardadd noambient noshadow 
		#pragma multi_compile __ PER_OBJECT_LIGHTING	
		#pragma shader_feature DEBUG_SKIN_COLOR

		sampler2D _MainTex;
		sampler2D _GlowTex;
		sampler2D _BumpMap;
		sampler2D _SpecularMap;
		half _BumpScale;
		half _Shininess;
		half _Gloss;
		fixed4 _Color;
		fixed3 _FlashColor;
		fixed _FlashIntensity;
		float _RimLightIntensity;
		fixed4 _GlowColor;
		fixed3 _SkinColor;
		float3 _CharacterLightDir;
		fixed3 _CharacterLightColor;

#ifdef PER_OBJECT_LIGHTING
		float3 _CharacterLightDir2;
		fixed3 _CharacterLightColor2;
#endif

		struct Input {
			float2 uv_MainTex;
			float2 uv_GlowTex;
			INTERNAL_DATA
		};

		void surf(Input IN, inout SurfaceOutput_CharacterVeryHighQuality o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			fixed4 s = tex2D(_SpecularMap, IN.uv_MainTex);
			fixed3 n = CustomUnpackScaleNormal(tex2D(_BumpMap, IN.uv_MainTex), _BumpScale);
			float2 guv = IN.uv_GlowTex + float2(_Time.y*0.25 + sin(_Time.y)*0.1, _Time.y*0.3 + sin(_Time.y)*0.05);
			fixed3 g = _GlowColor * tex2D(_GlowTex, guv) * 10 * (sin(_Time.y) * 0.5 + 1);
			o.Albedo = c.rgb;
			o.Normal = n;
			o.Specular = _Shininess;
			o.Gloss = _Gloss;
			o.Alpha = c.a;
			o.SpecColor = s.rgb;
			o.Rim = _RimLightIntensity;
			o.Glow = g;
			o.SkinColor = _SkinColor;

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
	SubShader {
		Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" "AlphaFix"="Cutout" "ModelShow" = "Player" }
		LOD 400
		
		CGPROGRAM
		#include "GameCG.cginc"
		#pragma target 3.0
		#pragma surface surf CharacterHighQuality alphatest:_Cutoff addshadow noforwardadd noambient noshadow 
		#pragma multi_compile __ PER_OBJECT_LIGHTING

		sampler2D _MainTex;
		sampler2D _GlowTex;
		sampler2D _BumpMap;
		sampler2D _SpecularMap;
		half _BumpScale;
		half _Shininess;
		half _Gloss;
		fixed4 _Color;
		fixed3 _FlashColor;
		fixed _FlashIntensity;
		float _RimLightIntensity;
		fixed4 _GlowColor;
		float3 _CharacterLightDir;
		fixed3 _CharacterLightColor;

#ifdef PER_OBJECT_LIGHTING
		float3 _CharacterLightDir2;
		fixed3 _CharacterLightColor2;
#endif

		struct Input {
			float2 uv_MainTex;
			float2 uv_GlowTex;
//			float2 uv_BumpMap;
//			float2 uv_SpecularMap;
			INTERNAL_DATA
		};

		void surf (Input IN, inout SurfaceOutput_CharacterHighQuality o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 s = tex2D(_SpecularMap, IN.uv_MainTex);
			fixed3 n = CustomUnpackScaleNormal(tex2D (_BumpMap, IN.uv_MainTex), _BumpScale);
			float2 guv = IN.uv_GlowTex + float2(_Time.y*0.25+sin(_Time.y)*0.1, _Time.y*0.3 + sin(_Time.y)*0.05);
			fixed3 g = _GlowColor * tex2D(_GlowTex, guv) * 10 * (sin(_Time.y) * 0.5 + 1);
			o.Albedo = c.rgb;
			o.Normal = n;
			o.Specular = _Shininess;
			o.Gloss = _Gloss;
			o.Alpha = c.a;
			o.SpecColor = s.rgb;
			o.Glow = g;
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

	SubShader{
		Tags{ "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" "AlphaFix"="Cutout" "ModelShow" = "Player" }
		LOD 300

		CGPROGRAM
		#include "GameCG.cginc"
		#pragma target 3.0
		#pragma surface surf CharacterMediumQuality alphatest:_Cutoff addshadow noforwardadd noambient noshadow
		#pragma multi_compile __ PER_OBJECT_LIGHTING

		sampler2D _MainTex;
		sampler2D _SpecularMap;
		fixed4 _Color;
		half _Shininess;
		half _Gloss;
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
			o.SpecColor = s.rgb * 1.3;
			o.Specular = _Shininess;
			o.Gloss = _Gloss;
			o.Albedo = c.rgb;
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

	SubShader{
		Tags{ "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" "AlphaFix"="Cutout" "ModelShow" = "Player" }
		LOD 200

		CGPROGRAM
		#include "GameCG.cginc"
		#pragma target 3.0
		#pragma surface surf CharacterLowQuality alphatest:_Cutoff addshadow noforwardadd noambient noshadow
		#pragma multi_compile __ PER_OBJECT_LIGHTING

		sampler2D _MainTex;
		sampler2D _SpecularMap;
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

		void surf(Input IN, inout SurfaceOutput_CharacterLowQuality o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
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
