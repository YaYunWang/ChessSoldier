
#ifndef PARTICLE_INC_INCLUDED
#define PARTICLE_INC_INCLUDED

#define TRANSFORM_INSTANCED_TEX(tex,name) (tex.xy * UNITY_ACCESS_INSTANCED_PROP(name##_ST).xy + UNITY_ACCESS_INSTANCED_PROP(name##_ST).zw)

struct appdata_particle {
	float4 vertex : POSITION;
	//float3 normal : NORMAL;
	float4 texcoord : TEXCOORD0;
	fixed4 color : COLOR;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};



inline float4 BillboardToClipPos(float3 vertexPos, float3 scale)
{
	const half DEG2RAD = 3.1415926 / 180;

	half3 origin = UnityObjectToViewPos(float3(0, 0, 0));
	half2 localPos = half2(-vertexPos.x * scale.x, vertexPos.y * scale.y);
	half cosx = cos(scale.z * DEG2RAD);
	half sinx = sin(scale.z * DEG2RAD);
	half2x2 mat = { cosx, sinx, -sinx, cosx };
	origin.xy += mul(localPos, mat);
	return mul(UNITY_MATRIX_P, float4(origin, 1));
}

#endif // PARTICLE_INC_INCLUDED