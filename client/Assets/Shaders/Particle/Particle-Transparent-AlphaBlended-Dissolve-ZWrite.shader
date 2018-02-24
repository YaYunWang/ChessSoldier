// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Demo03/Particle/Transparent/Alpha Blended (Dissolve + ZWrite)" {
    Properties {
        _Color ("Tint Color", Color) = (0.5, 0.5, 0.5, 0.5)
	    _MainTex ("Particle Texture", 2D) = "white" {}
		_DissolveTex("Dissolve Texture", 2D) = "grey" {}
		_DissolveValue("Dissolve Value", Range(0, 1)) = 0.5
		_DissolveStride("Dissolve Stride", Range(0, 1)) = 0.5
		_DissolveColor("Dissolve Color", Color) = (1,1,1,1)

		_BillboardScale("BillboardScale", Vector) = (1,1,1,1)
    }
    SubShader {
	    Tags { "Queue"="Transparent+1" "IgnoreProjector"="True" "RenderType"="Transparent" }
	    LOD 50

	    Pass {
	        Blend SrcAlpha OneMinusSrcAlpha
	        Cull Off 
	        ZWrite On 
			Fog { Mode Off }  
	    
	        CGPROGRAM
	        #pragma vertex vert
	        #pragma fragment frag
			#pragma multi_compile __ BILLBOARD_ON
			#pragma multi_compile_instancing

            #include "UnityCG.cginc"
			#include "ParticleInc.cginc"
    
		    struct v2f {
		        float4 pos : SV_POSITION;
		        half2 uv : TEXCOORD0;
				half2 uv2 : TEXCOORD1;
				fixed4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
		    };
		    

		    sampler2D _MainTex;
			sampler2D _DissolveTex;
			
			UNITY_INSTANCING_CBUFFER_START(ParticlePropBlock)
			UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
		    UNITY_DEFINE_INSTANCED_PROP(half4, _MainTex_ST)
			UNITY_DEFINE_INSTANCED_PROP(half4, _DissolveTex_ST)
			UNITY_DEFINE_INSTANCED_PROP(fixed, _DissolveValue)
			UNITY_DEFINE_INSTANCED_PROP(fixed, _DissolveStride)
			UNITY_DEFINE_INSTANCED_PROP(fixed4, _DissolveColor)

#ifdef BILLBOARD_ON
			UNITY_DEFINE_INSTANCED_PROP(float3, _BillboardScale)
#endif
			UNITY_INSTANCING_CBUFFER_END


		    v2f vert(appdata_particle v) {
		        v2f o = (v2f)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

#ifdef BILLBOARD_ON
				o.pos = BillboardToClipPos(v.vertex, UNITY_ACCESS_INSTANCED_PROP(_BillboardScale));
#else
				o.pos = UnityObjectToClipPos(v.vertex);
#endif
		        o.uv = TRANSFORM_INSTANCED_TEX(v.texcoord, _MainTex );
				o.uv2 = TRANSFORM_INSTANCED_TEX(v.texcoord, _DissolveTex);
				o.color = v.color;
		        return o;
		    }
		    
		    fixed4 frag(v2f i) : COLOR {
				UNITY_SETUP_INSTANCE_ID(i);

				fixed4 dissolve = tex2D(_DissolveTex, i.uv2);
				fixed clipAmount = dissolve - UNITY_ACCESS_INSTANCED_PROP(_DissolveValue) - UNITY_ACCESS_INSTANCED_PROP(_DissolveStride);
				clip(clipAmount);
				fixed lerpVal = saturate(clipAmount / UNITY_ACCESS_INSTANCED_PROP(_DissolveStride) + (1- UNITY_ACCESS_INSTANCED_PROP(_DissolveColor).a));
				fixed4 edgeColor = UNITY_ACCESS_INSTANCED_PROP(_DissolveColor);
				fixed4 texColor = tex2D(_MainTex, i.uv);
				edgeColor.a = texColor.a * i.color.a;
				fixed4 col = lerp(edgeColor, 2 * texColor * _Color * i.color, lerpVal);

		        return col;
		    }
	        ENDCG
	    }
    }
	CustomEditor "BillboardMaterialEditor"
    FallBack Off
}