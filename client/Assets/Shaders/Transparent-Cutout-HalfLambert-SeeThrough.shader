Shader "Demo03/Transparent (Cutout)/Alternative/See Through/Half Lambert" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
		_Indirect("Indirect Intensity", Range(0,5)) = 1
	}
	SubShader {
		Tags { "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType"="TransparentCutout" }
		LOD 100
		CULL Back
		
		CGPROGRAM
		#include "GameCG.cginc"

		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf HalfLambert alpha:fade addshadow noforwardadd

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;
		fixed _Indirect;
		fixed _Cutoff;

		void surf (Input IN, inout SurfaceOutput_HalfLambert o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * _Color.rgb;
			o.Alpha = _Color.a;
			o.IndirectIntensity = _Indirect; 
			clip(c.a - _Cutoff);
		}
		ENDCG
		// Extracts information for lightmapping, GI (emission, albedo, ...)
		// This pass it not used during regular rendering.
		Pass
		{
			CULL Off

			Name "META"
			Tags { "LightMode" = "Meta" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#include "UnityCG.cginc"
			#include "UnityMetaPass.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uvMain : TEXCOORD0;
			};

			float4 _MainTex_ST;

			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
				o.uvMain = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			sampler2D _MainTex;
			fixed4 _Color;
			fixed _Emission;

			half4 frag (v2f i) : SV_Target
			{
				UnityMetaInput metaIN;
				UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);

				fixed4 tex = tex2D(_MainTex, i.uvMain);
				fixed4 c = tex * _Color;
				metaIN.Albedo = c.rgb * 100;
				metaIN.Emission = half3(0,0,0);
	#if defined (UNITY_PASS_META)
				o.Emission *= _Emission.rrr;
	#endif
				return UnityMetaFragment(metaIN);
			}
			ENDCG
		}
	}

	Fallback "Transparent/Cutout/Diffuse"
}