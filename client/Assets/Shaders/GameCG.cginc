#ifndef GAME_CG_INCLUDE
#define GAME_CG_INCLUDE

#include "Lighting.cginc"

half3 CustomUnpackScaleNormal(half4 packednormal, half bumpScale)
{
#if defined(UNITY_NO_DXT5nm)
	half3 normal = packednormal.xyz * 2 - 1;
#if (SHADER_TARGET >= 30)
	// SM2.0: instruction count limitation
	// SM2.0: normal scaler is not supported
	normal.xy *= bumpScale;
	normal = normalize(normal);
#endif
	return normal;

#else
	half3 normal;
	normal.xy = (packednormal.wy * 2 - 1);
#if (SHADER_TARGET >= 30)
	// SM2.0: instruction count limitation
	// SM2.0: normal scaler is not supported
	normal.xy *= bumpScale;
#endif
	normal.z = sqrt(1.0 - saturate(dot(normal.xy, normal.xy)));
	return normal;
#endif
}

inline half CalcQuadricHalfLambert(half3 normal, half3 lightDir)
{
	half diff = 1 - ((dot(normal, lightDir) * 0.5f) + 0.5f);
	diff = 1 - (diff * diff);
	return diff;
}

inline half CalcHalfLambert(half3 normal, half3 lightDir)
{
	half diff = dot(normal, lightDir);
	return diff * 0.5 + 0.5;
}

struct SurfaceOutput_HalfLambert {
	fixed3 Albedo;
	half3 Normal;
	fixed3 Emission;
	half Specular;
	fixed Gloss;
	fixed Alpha;
	half IndirectIntensity;
};

inline fixed4 HalfLambertLight (SurfaceOutput_HalfLambert s, UnityLight light)
{
	fixed diff = dot(s.Normal, light.dir) * 0.5f + 0.5f;
	
	fixed4 c;
	c.rgb = s.Albedo * light.color * diff;
	c.a = s.Alpha;

	return c;
}

inline fixed4 LightingHalfLambert (SurfaceOutput_HalfLambert s, UnityGI gi)
{
	fixed4 c;
	c = HalfLambertLight (s, gi.light);

	#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
		c.rgb += s.Albedo * gi.indirect.diffuse;
	#endif

	return c;
}

inline void LightingHalfLambert_GI (
	SurfaceOutput_HalfLambert s,
	UnityGIInput data,
	inout UnityGI gi)
{
	gi = UnityGlobalIllumination (data, 1.0, s.Normal);
	gi.indirect.diffuse = (gi.indirect.diffuse + half3(s.IndirectIntensity,s.IndirectIntensity,s.IndirectIntensity))/(1+s.IndirectIntensity);
	//gi.indirect.diffuse = (gi.indirect.diffuse + half3(1,1,1)) * 0.5f;
}

struct SurfaceOutput_CharacterLowQuality {
	fixed3 Albedo;
	fixed Alpha;
	fixed3 Emission;
	half3 Normal;
	fixed3 FlashColor;
	fixed FlashIntensity;
	half3 lightDir;
	fixed3 lightColor;
};

inline fixed4 CharacterLowQualityLight(SurfaceOutput_CharacterLowQuality s, half3 viewDir, UnityLight light)
{
	fixed diff = CalcQuadricHalfLambert(s.Normal, s.lightDir);

	fixed4 c;
	c.rgb = s.Albedo * (0.5 + diff) * s.lightColor;
	c.rgb += s.FlashColor * s.FlashIntensity * s.Albedo;
	c.a = s.Alpha;
	return c;
}

inline fixed4 LightingCharacterLowQuality(SurfaceOutput_CharacterLowQuality s, half3 viewDir, UnityGI gi)
{
	fixed4 c;
	c = CharacterLowQualityLight(s, viewDir, gi.light);
	c.a = s.Alpha;

	return c;
}

inline void LightingCharacterLowQuality_GI(
	SurfaceOutput_CharacterLowQuality s,
	UnityGIInput data,
	inout UnityGI gi)
{
	//gi = UnityGlobalIllumination(data, 1.0, s.Normal);
}

struct SurfaceOutput_CharacterMediumQuality {
	fixed3 Albedo;
	half3 Normal;
	fixed3 Emission;
	half Specular;
	fixed Gloss;
	fixed Alpha;
	fixed3 SpecColor;
	fixed3 FlashColor;
	fixed FlashIntensity;
	half3 lightDir;
	fixed3 lightColor;
};

inline fixed4 CharacterMediumQualityLight(SurfaceOutput_CharacterMediumQuality s, half3 viewDir, UnityLight light)
{
	fixed diff = CalcQuadricHalfLambert(s.Normal, s.lightDir);

	half3 h = normalize(s.lightDir + viewDir);
	half nh = max(0, dot(s.Normal, h));
	half p = s.Specular * 6;
	half spec = pow(nh, p) *s.Gloss;

	fixed4 c;
	c.rgb = (s.Albedo * (diff + 0.3) +
		s.SpecColor.rgb * spec) * s.lightColor;

	c.rgb += s.FlashColor * s.FlashIntensity * (1 - nh);
	c.a = s.Alpha;
	return c;
}

inline fixed4 LightingCharacterMediumQuality(SurfaceOutput_CharacterMediumQuality s, half3 viewDir, UnityGI gi)
{
	fixed4 c;
	c = CharacterMediumQualityLight(s, viewDir, gi.light);

	return c;
}

inline void LightingCharacterMediumQuality_GI(
	SurfaceOutput_CharacterMediumQuality s,
	UnityGIInput data,
	inout UnityGI gi)
{
	//gi = UnityGlobalIllumination(data, 1.0, s.Normal);
}

struct SurfaceOutput_CharacterHighQuality {
	fixed3 Albedo;
	half3 Normal;
	fixed3 Emission;
	half Specular;
	fixed Gloss;
	fixed Alpha;
	fixed3 SpecColor;
	fixed3 Glow;
	fixed3 FlashColor;
	fixed FlashIntensity;
	half3 lightDir;
	fixed3 lightColor;
};

inline fixed4 CharacterHighQualityLight(SurfaceOutput_CharacterHighQuality s, half3 viewDir, UnityLight light)
{
	half4 c = 0;
	s.Normal = normalize(s.Normal);

	half diff = CalcHalfLambert(s.Normal, s.lightDir);
	
	half3 albedo = pow(s.Albedo.rgb, 2.2);

	half3 h = normalize(light.dir + viewDir);
	half nh = max(0, dot(s.Normal, h));

	half3 h2 = normalize(s.lightDir + viewDir);
	half nh2 = max(0, dot(s.Normal, h2));

	half p = s.Specular * 24;
	half spec = pow(max(nh,nh2), p) * s.Gloss;

	half nv = max(0, dot(viewDir, s.Normal));

	c.rgb = albedo * (diff  + nv * nv) * s.lightColor + saturate(spec * s.SpecColor * s.lightColor);

	// Rim
	half t = (1 - nv);
	t *= t;
	c.rgb += s.Glow * t * s.SpecColor.rgb;
	c.rgb += s.FlashColor * s.FlashIntensity * t;

	c.rgb = pow(c.rgb, 1 / 2.2);
	c.a = s.Alpha;

	return c;
}

inline fixed4 LightingCharacterHighQuality(SurfaceOutput_CharacterHighQuality s, half3 viewDir, UnityGI gi)
{
	fixed4 c;
	c = CharacterHighQualityLight(s, viewDir, gi.light);
	c.a = s.Alpha;

	return c;
}

inline void LightingCharacterHighQuality_GI(
	SurfaceOutput_CharacterHighQuality s,
	UnityGIInput data,
	inout UnityGI gi)
{
	//gi = UnityGlobalIllumination(data, 1.0, s.Normal);
}


struct SurfaceOutput_CharacterVeryHighQuality {
	fixed3 Albedo;
	half3 Normal;
	fixed3 Emission;
	half Specular;
	fixed Gloss;
	fixed Alpha;
	fixed3 SpecColor;
	fixed3 RimColor;
	fixed Rim;
	fixed3 Glow;
	fixed3 SkinColor;
	half3 lightDir;
	fixed3 lightColor;
};

inline fixed4 CharacterVeryHighQualityLight(SurfaceOutput_CharacterVeryHighQuality s, half3 viewDir, UnityLight light)
{
	s.Normal = normalize(s.Normal);

	half diff = CalcHalfLambert(s.Normal, s.lightDir);

	half3 h = normalize(s.lightDir + viewDir);
	half nh = max(0, dot(s.Normal, h));

	//half r = dot(reflect(viewDir, s.Normal), viewDir);
	//r = max(0, r);

	half nv = saturate(dot(s.Normal, viewDir));

	fixed4 c;

	half p = s.Specular * 12;
	half spec = pow(nh, p) * s.Gloss;

	half spec2 = pow(nv, 20);

	// Rim
	half t = (1 - nv);
	t = t * t * t;
	half spec3 = saturate(t * s.Rim);

	half glow = t;

	half3 skyColor = half3(0.24313, 0.5608, 1);

	half3 d = abs(s.SkinColor - s.Albedo);
	half skin = max(0, 1 - dot(d, d));
	skin = saturate(pow(skin, 30));
	half skinMask = (1 - skin);

	half3 albedo = pow(s.Albedo.rgb, 2.2);

	c.rgb = albedo * s.lightColor * (diff + skin * 0.1f);
	c.rgb += s.SpecColor.rgb * (max(half3(1,1,1) * saturate(spec + spec2) * skinMask, skyColor * spec3));
	c.rgb += s.Glow * glow * s.SpecColor.rgb;

#if DEBUG_SKIN_COLOR
	c.rgb = fixed3(skinMask, skinMask, 1);
#endif
	c.rgb = pow(c.rgb, 1 / 2.2);

	c.a = s.Alpha;
	return c;
}

inline fixed4 LightingCharacterVeryHighQuality(SurfaceOutput_CharacterVeryHighQuality s, half3 viewDir, UnityGI gi)
{
	fixed4 c;
	c = CharacterVeryHighQualityLight(s, viewDir, gi.light);
	c.a = s.Alpha;

	return c;
}

inline void LightingCharacterVeryHighQuality_GI(
	SurfaceOutput_CharacterVeryHighQuality s,
	UnityGIInput data,
	inout UnityGI gi)
{
	//gi = UnityGlobalIllumination(data, 1.0, s.Normal);
}

struct SurfaceOutput_CustomBlinnPhong {
	fixed3 Albedo;
	half3 Normal;
	fixed3 Emission;
	half Specular;
	fixed Gloss;
	fixed Alpha;
	fixed3 SpecColor;
};

inline fixed4 CustomBlinnPhongLight (SurfaceOutput_CustomBlinnPhong s, half3 viewDir, UnityLight light)
{
	half3 h = normalize (light.dir + viewDir);
	
	fixed diff = max (0, dot (s.Normal, light.dir));
	
	float nh = max (0, dot (s.Normal, h));
	float spec = pow (nh, s.Specular*128.0) * s.Gloss;
	
	fixed4 c;
	c.rgb = s.Albedo * light.color * diff + light.color * s.SpecColor.rgb * saturate(spec);
	c.a = s.Alpha;

	return c;
	//return fixed4(light.dir,1);
}

inline fixed4 LightingCustomBlinnPhong (SurfaceOutput_CustomBlinnPhong s, half3 viewDir, UnityGI gi)
{
	fixed4 c;
	c = CustomBlinnPhongLight (s, viewDir, gi.light);

	#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
		c.rgb += s.Albedo * gi.indirect.diffuse;
	#endif

	return c;
}

inline void LightingCustomBlinnPhong_GI (
	SurfaceOutput_CustomBlinnPhong s,
	UnityGIInput data,
	inout UnityGI gi)
{
	gi = UnityGlobalIllumination (data, 1.0, s.Normal);
}

inline fixed4 QuadraticLambertLight(SurfaceOutput s, UnityLight light)
{
	float d = dot(s.Normal, light.dir);
	float diff = 1 + d - 0.5f * d * d;
	diff = max(0.3f, diff);
	fixed4 c;
	c.rgb = s.Albedo * light.color * diff;
	c.a = s.Alpha;
	return c;
}

inline fixed4 LightingQuadraticLambert(SurfaceOutput s, UnityGI gi)
{
	fixed4 c;
	c = QuadraticLambertLight(s, gi.light);

#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
	c.rgb += s.Albedo * gi.indirect.diffuse;
#endif

	return c;
}

inline void LightingQuadraticLambert_GI(
	SurfaceOutput s,
	UnityGIInput data,
	inout UnityGI gi)
{
	gi = UnityGlobalIllumination(data, 1.0, s.Normal);
}

struct SurfaceOutput_Water {
	fixed3 Albedo;
	half3 Normal;
	fixed3 Emission;
	float Specular;
	float Gloss;
	fixed Alpha;
	fixed3 SpecColor;
};

inline fixed4 WaterLight(SurfaceOutput_Water s, half3 viewDir, UnityLight light)
{
	float3 h = normalize(light.dir + viewDir);

	fixed diff = max(0, dot(s.Normal, light.dir));

	float nh = max(0, dot(s.Normal, h));
	float spec = pow(nh, s.Specular*128.0) * s.Gloss;

	fixed4 c;
	c.rgb = s.Albedo * light.color * diff + light.color * s.SpecColor.rgb * spec;

	half nv = max(0, dot(s.Normal, viewDir));
	nv = nv * nv * nv;

	nv = 1 - nv;
	c.a = s.Alpha * max(nv, 0.6);

	return c;
}

inline fixed4 LightingWater(SurfaceOutput_Water s, half3 viewDir, UnityGI gi)
{
	fixed4 c;
	c = WaterLight(s, viewDir, gi.light);

#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
	c.rgb += s.Albedo * gi.indirect.diffuse;
#endif
	return c;
}

inline void LightingWater_GI(
	SurfaceOutput_Water s,
	UnityGIInput data,
	inout UnityGI gi)
{
	gi = UnityGlobalIllumination(data, 1.0, s.Normal);
}

inline fixed4 WaterLowQualityLight(SurfaceOutput_Water s, half3 viewDir, UnityLight light)
{
	fixed diff = max(0, dot(s.Normal, light.dir));
	fixed nv = max(0, dot(s.Normal, viewDir));
	nv = nv * nv;
	nv = max(0.8, (1 - nv));
	fixed4 c;
	c.rgb = s.Albedo * light.color * diff;

	c.a = s.Alpha * nv;
	return c;
}

inline fixed4 LightingWaterLowQuality(SurfaceOutput_Water s, half3 viewDir, UnityGI gi)
{
	fixed4 c;
	c = WaterLowQualityLight(s, viewDir, gi.light);

#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
	c.rgb += s.Albedo * gi.indirect.diffuse;
#endif

	return c;
}

inline void LightingWaterLowQuality_GI(
	SurfaceOutput_Water s,
	UnityGIInput data,
	inout UnityGI gi)
{
	gi = UnityGlobalIllumination(data, 1.0, s.Normal);
}
#endif