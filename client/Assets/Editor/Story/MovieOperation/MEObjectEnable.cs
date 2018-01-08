using UnityEngine;
using System.Collections;
using UnityEditor;

public class MEObjectEnable : StoryEditorBase
{
    private string m_object_url = "";//空是主摄像机
    private bool m_object_value = false;//隐藏，显示
    private bool m_is_return = true;//是否返回
	public override void DrawWorkSpace()
	{
		base.DrawWorkSpace();
	}

    public override void DrawInspectorSpace()
    {
        base.DrawInspectorSpace();

        m_object_url = EditorGUILayout.TextField("物件路径（空是主摄像机）:", m_object_url);
        m_object_value = EditorGUILayout.Toggle("隐藏显示:", m_object_value);
        m_is_return = EditorGUILayout.Toggle("是否还原:", m_is_return);
    }

    public override void SaveData(System.Xml.XmlElement element)
    {
        base.SaveData(element);
        element.SetAttribute("ObjectURL", m_object_url);
        element.SetAttribute("ObjectValue", m_object_value.ToString());
        element.SetAttribute("IsReturn", m_is_return.ToString());
    }

    public override void LoadData(System.Xml.XmlElement element)
    {
        base.LoadData(element);
        m_object_url = element.GetAttribute("ObjectURL");
        m_object_value = GameConvert.BoolConvert(element.GetAttribute("ObjectValue"));
        m_is_return = GameConvert.BoolConvert(element.GetAttribute("IsReturn"));
    }
}
