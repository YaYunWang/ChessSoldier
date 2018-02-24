Shader "Demo03/Particle/Transparent/Additive (SR2)" {
	Properties{
		_MainTex("Base layer (RGB)", 2D) = "white" {}
		_Scroll("Base layer scroll speed (Vector2) X Y", Vector) = (1,0,0,0)
		_Rotation("Base layer rotation (CenterX, CenterY, Start, Speed)", Vector) = (0.5, 0.5, 0, 1)

		_DetailTex("2nd layer (RGB)", 2D) = "white" {}
		_Scroll2("2nd layer scroll speed (Vector2) X Y", Vector) = (1,0,0,0)
		_Rotation2("2nd layer rotation (CenterX, CenterY, Start, Speed)", Vector) = (0.5, 0.5, 0, 1)

		_Color("Color", Color) = (1,1,1,1)
		_MMultiplier("Multiplier", Float) = 2.0

		_BillboardScale("BillboardScale", Vector) = (1,1,1,1)
	}

	SubShader{
		Tags{ "Queue" = "Transparent+1" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Pass{
		Blend SrcAlpha One
		Cull Off Lighting Off ZWrite Off
		Fog{ Mode Off }
		LOD 50

		CGPROGRAM
		#include "UnityCG.cginc"
		#include "ParticleInc.cginc"

		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile __ BILLBOARD_ON	
		#pragma multi_compile_instancing


		sampler2D _MainTex;
		sampler2D _DetailTex;

		UNITY_INSTANCING_CBUFFER_START(ParticlePropBlock)

		UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
		UNITY_DEFINE_INSTANCED_PROP(float2, _Scroll)
		UNITY_DEFINE_INSTANCED_PROP(float4, _Rotation)

		UNITY_DEFINE_INSTANCED_PROP(float4, _DetailTex_ST)
		UNITY_DEFINE_INSTANCED_PROP(float2, _Scroll2)
		UNITY_DEFINE_INSTANCED_PROP(float4, _Rotation2)

		UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
		UNITY_DEFINE_INSTANCED_PROP(float, _MMultiplier)

#ifdef BILLBOARD_ON
		UNITY_DEFINE_INSTANCED_PROP(float3, _BillboardScale)
#endif

		UNITY_INSTANCING_CBUFFER_END

		struct v2f {
			float4 pos : SV_POSITION;
			float4 uv : TEXCOORD0;
			fixed4 color : TEXCOORD1;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		float2 CalcUV(float2 uv, float tx, float ty, float rstart, float rx, float ry, float r)
		{
			float s = sin(r*_Time + rstart);
			float c = cos(r*_Time + rstart);

			float2x2 m = { c, -s, s, c };

			uv -= float2(rx, ry);
			uv = mul(uv, m);
			uv += float2(rx, ry);

			uv += frac(float2(tx, ty) * _Time);

			return uv;
		}

		v2f vert(appdata_particle v)
		{
			v2f o;

			UNITY_SETUP_INSTANCE_ID(v);
			UNITY_TRANSFER_INSTANCE_ID(v, o);

	#ifdef BILLBOARD_ON
			o.pos = BillboardToClipPos(v.vertex, UNITY_ACCESS_INSTANCED_PROP(_BillboardScale));
	#else
			o.pos = UnityObjectToClipPos(v.vertex);
	#endif

			// base layer
			o.uv.xy = TRANSFORM_INSTANCED_TEX(v.texcoord.xy,_MainTex);
			float2 scroll = UNITY_ACCESS_INSTANCED_PROP(_Scroll);
			float4 rotation = UNITY_ACCESS_INSTANCED_PROP(_Rotation);
			o.uv.xy = CalcUV(o.uv.xy,
				scroll.x,
				scroll.y,
				rotation.z,
				rotation.x,
				rotation.y,
				rotation.w);

			// 2nd layer
			o.uv.zw = TRANSFORM_INSTANCED_TEX(v.texcoord.xy,_DetailTex);
			scroll = UNITY_ACCESS_INSTANCED_PROP(_Scroll2);
			rotation = UNITY_ACCESS_INSTANCED_PROP(_Rotation2);
			o.uv.zw = CalcUV(o.uv.zw,
				scroll.x,
				scroll.y,
				rotation.z,
				rotation.x,
				rotation.y,
				rotation.w);

			o.color = UNITY_ACCESS_INSTANCED_PROP(_MMultiplier) * UNITY_ACCESS_INSTANCED_PROP(_Color) * v.color;

			return o;
		}

		fixed4 frag(v2f i) : COLOR
		{
			return tex2D(_MainTex, i.uv.xy) * tex2D(_DetailTex, i.uv.zw) * i.color;
		}
		ENDCG
	}
}
CustomEditor "BillboardMaterialEditor"
}

