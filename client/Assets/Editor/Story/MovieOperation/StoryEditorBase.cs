using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Xml;

public class StoryEditorBase : Editor
{
	public float time_rule_interval_pixel = 50;
	public float item_height = 20;

	public float m_start_time = 0f;
	public float m_end_time = 0f;
	public bool m_is_select = false;
	public int m_index = 0;
    public string m_type = string.Empty;
	public virtual void DrawWorkSpace()
	{
        float x = m_start_time * time_rule_interval_pixel;
        float width = (m_end_time - m_start_time) * time_rule_interval_pixel;
        if (width < 10) width = 10;
        if (GUI.Button(new Rect(x, (m_index + 1) * item_height, width, item_height), "", EditorStyles.miniButton))
        {
        }
	}

	public virtual void DrawInspectorSpace()
	{
        EditorGUILayout.LabelField(m_type);
        m_start_time = EditorGUILayout.FloatField("开启时间:", m_start_time);
        m_end_time = EditorGUILayout.FloatField("结束时间:", m_end_time);
	}

	public virtual void OnSelect()
	{
		m_is_select = true;
	}

	public virtual void UnSelect()
	{
		m_is_select = false;
	}

    public virtual void OnInit()
    {

    }

    public virtual void OnLeave()
    {

    }

    public virtual void SaveData(XmlElement element)
    {
        element.SetAttribute("Type", m_type);
        element.SetAttribute("StartTime", m_start_time.ToString());
        element.SetAttribute("EndTime", m_end_time.ToString());
    }

    public virtual void LoadData(XmlElement element)
    {
        m_start_time = GameConvert.FloatConvert(element.GetAttribute("StartTime"));
        m_end_time = GameConvert.FloatConvert(element.GetAttribute("EndTime"));
    }
}
