Shader "Hidden/Demo03/ModelShow (RAW)" {

	SubShader {
		Tags { "Queue" = "AlphaTest" "ModelShow" = "Player" }
		LOD 100
		
		CGPROGRAM
		#include "../GameCG.cginc"
		
		#pragma target 3.0
		#pragma surface surf CharacterHighQuality alphatest:_Cutoff addshadow noforwardadd noambient noshadow 
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
			//			float2 uv_BumpMap;
			//			float2 uv_SpecularMap;
			INTERNAL_DATA
		};
		
		void surf(Input IN, inout SurfaceOutput_CharacterHighQuality o) {
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
	FallBack "Diffuse"
}
