// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Demo03/Skill/Rect Alert"{
	Properties{
		_Color("Tint Color", Color) = (0.8, 0, 0, 0.9)
		_Scale("Scale", Range(0, 2)) = 1
		_BColor("Background Color", Range(0,1)) = 0.3
		//_MaxScale("Max Scale", Float) = 1
		_Edge("Edge", Range(0, 1)) = 0.3
    }
    SubShader {
	    Tags { "Queue"="Transparent+1" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 50
        
	    Pass {
	        Blend SrcAlpha One
	        Cull Back 
	        ZWrite Off 
			Fog { Mode Off }
	    
	        CGPROGRAM
	        #pragma vertex vert
	        #pragma fragment frag
	        #pragma fragmentoption ARB_precision_hint_fastest

            #include "UnityCG.cginc"
    
		    struct v2f {
		        float4 pos : SV_POSITION;
				float3 objPos : TEXCOORD0;
		    };
		    
		    fixed4 _Color;
			float _Scale;
			float _Edge;
			float _BColor;

		    v2f vert(appdata_base v ) {
				v2f o;
		        o.pos = UnityObjectToClipPos( v.vertex );
				o.objPos = v.vertex.xyz;
		        return o;
		    }
		    
		    fixed4 frag( v2f i ) : COLOR {
				const float _MaxScale = 0.5;

				fixed4 col = (fixed4)0;// = 2 * tex2D(_MainTex, i.uv) * _Color * i.color;
				float l = (i.objPos.y + _MaxScale) / (2 * _MaxScale);
				if (l < min(_Scale, 1))
				{
					float fadeoff = max(0, _Scale - 1) * 3;
					col.rgb = max(smoothstep(_Scale - _Edge, _Scale, l), lerp(_BColor, 0, fadeoff));
					col.rgb *= _Color;
				}
				else
				{
					col.rgb = 0;
				}

				col.a = 1;
		        return col;
		    }
	        ENDCG
	    }
    }
    
    FallBack Off
}