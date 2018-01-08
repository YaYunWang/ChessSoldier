using UnityEngine;
using System.Collections;
using UnityEditor;

public class MEPlayEffect : StoryEditorBase
{
    private int m_effect_id = -1;
    private bool m_level_destory = true;
    public override void DrawInspectorSpace()
    {
        base.DrawInspectorSpace();

        m_effect_id = EditorGUILayout.IntField("特效ID:", m_effect_id);
        m_level_destory = EditorGUILayout.Toggle("特效是否跟随剧情消亡:", m_level_destory);
    }

    public override void SaveData(System.Xml.XmlElement element)
    {
        base.SaveData(element);
        element.SetAttribute("EffectID", m_effect_id.ToString());
        element.SetAttribute("Desotry", m_level_destory.ToString());
    }

    public override void LoadData(System.Xml.XmlElement element)
    {
        base.LoadData(element);
        m_effect_id = GameConvert.IntConvert(element.GetAttribute("EffectID"));
        m_level_destory = GameConvert.BoolConvert(element.GetAttribute("Desotry"));
    }
}
