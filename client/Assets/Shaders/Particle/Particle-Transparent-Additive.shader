Shader "Demo03/Particle/Transparent/Additive" {
    Properties {
        _Color ("Tint Color", Color) = (0.5, 0.5, 0.5, 0.5)
	    _MainTex ("Particle Texture", 2D) = "white" {}
		_MMultiplier("Multiplier", Float) = 1.0
		_BillboardScale("BillboardScale", Vector) = (1,1,1,1)
    }
    SubShader {
	    Tags { "Queue"="Transparent+1" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 50
        
	    Pass {
			ColorMask RGB
	        Blend SrcAlpha One
	        Cull Off 
	        ZWrite Off 
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
				fixed4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
		    };

			sampler2D _MainTex;

			UNITY_INSTANCING_CBUFFER_START(ParticlePropBlockAdditive)
			UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
			UNITY_DEFINE_INSTANCED_PROP(half4, _MainTex_ST)
			UNITY_DEFINE_INSTANCED_PROP(float, _MMultiplier)
#ifdef BILLBOARD_ON
			UNITY_DEFINE_INSTANCED_PROP(float3, _BillboardScale)
#endif
			UNITY_INSTANCING_CBUFFER_END


		    v2f vert(appdata_particle v ) {
		        v2f o = (v2f)0;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

#ifdef BILLBOARD_ON
				o.pos = BillboardToClipPos(v.vertex, UNITY_ACCESS_INSTANCED_PROP(_BillboardScale));
#else
				o.pos = UnityObjectToClipPos(v.vertex);
#endif

		        o.uv = TRANSFORM_INSTANCED_TEX( v.texcoord, _MainTex);
				o.color = v.color * UNITY_ACCESS_INSTANCED_PROP(_MMultiplier);
		        return o;
		    }
		    
		    fixed4 frag( v2f i ) : COLOR {
				UNITY_SETUP_INSTANCE_ID(i);

		        fixed4 col = 2 * tex2D(_MainTex, i.uv ) *
					UNITY_ACCESS_INSTANCED_PROP(_Color) * i.color;

		        return col;
		    }
	        ENDCG
	    }
    }


	CustomEditor "BillboardMaterialEditor"
    FallBack Off
}