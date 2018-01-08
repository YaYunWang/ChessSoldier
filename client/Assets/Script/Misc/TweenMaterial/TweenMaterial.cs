//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright 漏 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;


/// <summary>
/// Tween the object's material.
/// </summary>

//[AddComponentMenu("NGUI/Tween/Tween Material")]
public class TweenMaterial : UITweener
{
	public enum PropertyTypes
	{
		None,
		Color,
		Vector,
		Float,
		TexParam,
	}

	public enum ColorLerpTypes
	{
		Curve,
		Gradient,
	}

	public string propertyName = "";
	public PropertyTypes propertyType = PropertyTypes.None;

	public object from
	{
		get 
		{ 
			switch(propertyType)
			{
			case PropertyTypes.Color:
				return fromColor;
			case PropertyTypes.Vector:
				return fromVector;
			case PropertyTypes.Float:
				return fromFloat;
			case PropertyTypes.TexParam:
				return fromVector;
			}
			return null;
		}
		set
		{
			switch(propertyType)
			{
			case PropertyTypes.Color:
				fromColor = (Color)value;
				break;
			case PropertyTypes.Vector:
			case PropertyTypes.TexParam:
				fromVector = (Vector4)value;
				break;
			case PropertyTypes.Float:
				fromFloat = (float)value;
				break;
			}
		}
	}

	public object to
	{
		get 
		{ 
			switch(propertyType)
			{
			case PropertyTypes.Color:
				return toColor;
			case PropertyTypes.Vector:
				return toVector;
			case PropertyTypes.Float:
				return toFloat;
			case PropertyTypes.TexParam:
				return toVector;
			}
			return null;
		}
		set
		{
			switch(propertyType)
			{
			case PropertyTypes.Color:
				toColor = (Color)value;
				break;
			case PropertyTypes.Vector:
			case PropertyTypes.TexParam:
				toVector = (Vector4)value;
				break;
			case PropertyTypes.Float:
				toFloat = (float)value;
				break;
			}
		}
	}

	public ColorLerpTypes colorLerpType = ColorLerpTypes.Curve;
	public Gradient colorGradient = new Gradient();
	public Color fromColor = Color.white;
	public Color toColor = Color.white;

	public Vector4 fromVector = Vector4.zero;
	public Vector4 toVector = Vector4.zero;

	public float fromFloat = 0;
	public float toFloat = 0;

	bool mCached = false;
    private int propID = 0;
    //private float time;

    private MaterialObject mMatObj;

	public void Cache ()
	{
		if (!Application.isPlaying)
			return;

		mCached = true;
        var ren = GetComponent<Renderer>();

        if (ren != null)
        {
            mMatObj = gameObject.SafeAddComponent<MaterialObject>();
        }

        if (propertyType == PropertyTypes.TexParam)
        {
            string propName = propertyName + "_ST";
            propID = Shader.PropertyToID(propName);
        }
        else
        {
            propID = Shader.PropertyToID(propertyName);
        }
    }

	/// <summary>
	/// Tween's current value.
	/// </summary>

	public object value
	{
		get
		{
			if (!mCached) Cache();
			if (mMatObj == null) return null;

			switch(propertyType)
			{
			case PropertyTypes.Color:
				return mMatObj.GetColor(propID);
			case PropertyTypes.Vector:
				return mMatObj.GetVector(propID);
			case PropertyTypes.Float:
				return mMatObj.GetFloat(propID);
			case PropertyTypes.TexParam:
                return mMatObj.GetVector(propID);
			}
			return null;
		}
		set
		{
			if (!mCached) Cache();
			if (mMatObj == null) return;

			switch(propertyType)
			{
			case PropertyTypes.Color:
                mMatObj.SetColor(propID, (Color)value);
				break;
			case PropertyTypes.Vector:
                mMatObj.SetVector(propID, (Vector4)value);
				break;
			case PropertyTypes.Float:
                mMatObj.SetFloat(propID, (float)value);
				break;
			case PropertyTypes.TexParam:
				Vector4 param = (Vector4)value;
                mMatObj.SetVector(propID, param);
				break;
			}

            mMatObj.Flush();
        }
	}

	/// <summary>
	/// Tween the value.
	/// </summary>

	protected override void OnUpdate (float factor, bool isFinished) 
	{ 
		switch(propertyType)
		{
		case PropertyTypes.Color:
			if (colorLerpType == ColorLerpTypes.Curve)
			{
				value = Color.Lerp(fromColor, toColor, factor); 
			}
			else
			{
				value = colorGradient.Evaluate(factor);
			}
			break;
		case PropertyTypes.Vector:
		case PropertyTypes.TexParam:
			value = Vector4.Lerp(fromVector, toVector, factor);
			break;
		case PropertyTypes.Float:
			value = Mathf.Lerp(fromFloat, toFloat, factor);
			break;
		}
	}

	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue () { from = value; }

	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue () { to = value; }

	[ContextMenu("Assume value of 'From'")]
	void SetCurrentValueToStart () { value = from; }

	[ContextMenu("Assume value of 'To'")]
	void SetCurrentValueToEnd () { value = to; }
}
