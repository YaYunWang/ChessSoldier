Shader "Demo03/Particle/Transparent/Alpha Blended (Distortion)" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_Distortion ("Distortion", Range(0, 100)) = 1

		_BillboardScale("BillboardScale", Vector) = (1,1,1,1)
	}
	SubShader {
	    Tags { "Queue"="Transparent+2" "IgnoreProjector"="True" "RenderType"="Transparent" }
	    LOD 50
		
		GrabPass {
	    	"_GrabTexture"
	    }
	    Pass {
	        Blend SrcAlpha OneMinusSrcAlpha
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
		        half2 uv1 : TEXCOORD1;
		        half4 grabPos : TEXCOORD2;
				fixed4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
		    };
			
			sampler2D _MainTex;
		    sampler2D _BumpMap;
			sampler2D _GrabTexture;
			float4 _GrabTexture_TexelSize;
			
			UNITY_INSTANCING_CBUFFER_START(ParticlePropBlock)
		    UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
		    UNITY_DEFINE_INSTANCED_PROP(half4, _MainTex_ST)
		    UNITY_DEFINE_INSTANCED_PROP(half4, _BumpMap_ST)
		    UNITY_DEFINE_INSTANCED_PROP(half, _Distortion)

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
		        o.grabPos = ComputeGrabScreenPos(o.pos);
		        o.uv = TRANSFORM_INSTANCED_TEX(v.texcoord, _MainTex);
		        o.uv1 = TRANSFORM_INSTANCED_TEX(v.texcoord, _BumpMap);
				o.color = v.color;
		        return o;
		    }
		    
		    fixed4 frag( v2f i ) : COLOR {
				UNITY_SETUP_INSTANCE_ID(i);

		        fixed4 col = 2 * tex2D( _MainTex, i.uv ) * UNITY_ACCESS_INSTANCED_PROP(_Color) * i.color;
				half2 bump = UnpackNormal(tex2D(_BumpMap, i.uv1)).rg * UNITY_ACCESS_INSTANCED_PROP(_Distortion) *_GrabTexture_TexelSize.xy;
				i.grabPos.xy += bump.xy * i.grabPos.w;
		        half4 bgcolor = tex2Dproj(_GrabTexture, i.grabPos);
		        return col * bgcolor;
		    }
	        ENDCG
	    }
    }
	SubShader {
		Blend DstColor Zero
		Pass {
			Name "BASE"
			SetTexture [_MainTex] {	combine texture }
		}
	}
	CustomEditor "BillboardMaterialEditor"
}
