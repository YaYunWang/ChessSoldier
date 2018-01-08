//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(TweenMaterial))]
public class TweenMaterialEditor : UITweenerEditor
{
	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		//EventDelegateEditor.SetLabelWidth(120f);

		TweenMaterial tw = target as TweenMaterial;
        EditorGUI.BeginChangeCheck();

        var ren = tw.GetComponent<Renderer>();
        if (ren == null)
        {
            EditorGUILayout.HelpBox("Can't find renderer in this object", MessageType.Warning);
            return;
        }

        Material mat = ren.sharedMaterial;
		if (mat == null)
		{
			EditorGUILayout.HelpBox("Can't find material in this object", MessageType.Warning);
			return;
		}

		Shader s = mat.shader;
		if (s == null)
		{
			EditorGUILayout.HelpBox("Can't find shader in this material", MessageType.Warning);
			return;
		}

		Dictionary<string, TweenMaterial.PropertyTypes> validProperty = new Dictionary<string, TweenMaterial.PropertyTypes>();

		for (int i = 0; i < ShaderUtil.GetPropertyCount(s); i++)
		{
			if (ShaderUtil.IsShaderPropertyHidden(s, i))
			{
				continue;
			}

			string n = ShaderUtil.GetPropertyName(s, i);

			TweenMaterial.PropertyTypes t = GetPropertyType(s, i);

			if (t == TweenMaterial.PropertyTypes.None)
			{
				continue;
			}

			validProperty[n] = t;
		}

		if (validProperty.Count <= 0)
		{
			EditorGUILayout.HelpBox("Can't find valid property in this material", MessageType.Info);
			return;
		}

		if (string.IsNullOrEmpty(tw.propertyName))
		{
			tw.propertyName = validProperty.Keys.First();
			tw.propertyType = validProperty[tw.propertyName];
		}

		List<string> names = validProperty.Keys.ToList();

		int selectedIndex = names.FindIndex(delegate(string n){ return n==tw.propertyName; });

		if (selectedIndex == -1)
			selectedIndex = 0;

		selectedIndex = EditorGUILayout.Popup(selectedIndex, names.ToArray());
		tw.propertyName = names[selectedIndex];
		tw.propertyType = validProperty[tw.propertyName];

		object from = null;
		object to = null;

		switch (tw.propertyType)
		{
		case TweenMaterial.PropertyTypes.Color:
			tw.colorLerpType = (TweenMaterial.ColorLerpTypes)EditorGUILayout.EnumPopup(tw.colorLerpType);
			if (tw.colorLerpType == TweenMaterial.ColorLerpTypes.Curve)
			{
				from = EditorGUILayout.ColorField("From", (Color)tw.from);
				to = EditorGUILayout.ColorField("To", (Color)tw.to);
			}
			else
			{
				SerializedObject serializedGradient = new SerializedObject(tw);
				SerializedProperty colorGradient = serializedGradient.FindProperty("colorGradient");
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(colorGradient);
				if (EditorGUI.EndChangeCheck())
				{
					serializedGradient.ApplyModifiedProperties();
				}
			}
			break;
		case TweenMaterial.PropertyTypes.Vector:
			from = EditorGUILayout.Vector4Field("From", (Vector4)tw.from);
			to = EditorGUILayout.Vector4Field("To", (Vector4)tw.to);
			break;
		case TweenMaterial.PropertyTypes.Float:
			from = EditorGUILayout.FloatField("From", (float)tw.from);
			to = EditorGUILayout.FloatField("To", (float)tw.to);
			break;
		case TweenMaterial.PropertyTypes.TexParam:
			from = EditorGUILayout.Vector4Field("From", (Vector4)tw.from);
			to = EditorGUILayout.Vector4Field("To", (Vector4)tw.to);
			break;
		}



		if (EditorGUI.EndChangeCheck())
		{
			//EventDelegateEditor.RegisterUndo("Tween Change", tw);
			tw.from = from;
			tw.to = to;

            EditorUtility.SetDirty(tw);
			//EventDelegateEditor.SetDirty(tw);
		}

		DrawCommonProperties();
	}

	TweenMaterial.PropertyTypes GetPropertyType(Shader s, int propertyIdx)
	{
		ShaderUtil.ShaderPropertyType t = ShaderUtil.GetPropertyType(s, propertyIdx);

		switch(t)
		{
		case ShaderUtil.ShaderPropertyType.Color:
			return TweenMaterial.PropertyTypes.Color;
		case ShaderUtil.ShaderPropertyType.Float:
			return TweenMaterial.PropertyTypes.Float;
		case ShaderUtil.ShaderPropertyType.Range:
			return TweenMaterial.PropertyTypes.Float;
		case ShaderUtil.ShaderPropertyType.TexEnv:
			return TweenMaterial.PropertyTypes.TexParam;
		case ShaderUtil.ShaderPropertyType.Vector:
			return TweenMaterial.PropertyTypes.Vector;
		}

		return TweenMaterial.PropertyTypes.None;
	}



}

