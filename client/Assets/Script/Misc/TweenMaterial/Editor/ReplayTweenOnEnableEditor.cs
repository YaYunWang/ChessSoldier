using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ReplayTweenOnEnable))]
public class ReplayTweenOnEnableEditor : Editor
{
    SerializedProperty tweeners;
    bool uiTweenersDirty = false;

    public void OnEnable()
    {
        tweeners = serializedObject.FindProperty("tweens");
        CheckUITweenersDirty();
    }

    private void CheckUITweenersDirty()
    {
        uiTweenersDirty = true;

        ReplayTweenOnEnable replayScript = (ReplayTweenOnEnable)target;
        var curTweeners = replayScript.gameObject.GetComponentsInChildren<UITweener>(true);

        HashSet<UITweener> hashset0 = new HashSet<UITweener>();
        HashSet<UITweener> hashset1 = new HashSet<UITweener>();

        foreach (var s in curTweeners)
        {
            if (s.enabled)
                hashset0.Add(s);
        }

        int arraySize = tweeners.arraySize;

        for (int i = 0; i < arraySize; i++)
        {
            var tweener = tweeners.GetArrayElementAtIndex(i);

            hashset1.Add((UITweener)tweener.objectReferenceValue);
        }

        if (hashset0.IsSubsetOf(hashset1) && hashset0.Count == hashset1.Count)
        {
            uiTweenersDirty = false;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Tweeners: ");

        if (uiTweenersDirty)
        {
            Color c = GUI.contentColor;
            GUI.contentColor = Color.red;
            GUILayout.Label("Dirty");
            GUI.contentColor = c;
        }
        else
        {
            GUILayout.Label("Clean");
        }

        GUILayout.EndHorizontal();


        if (GUILayout.Button("Find All Tween"))
        {
            ReplayTweenOnEnable replayScript = (ReplayTweenOnEnable)target;
            var curTweeners = replayScript.gameObject.GetComponentsInChildren<UITweener>(true);

            List<UITweener> activeTweeners = new List<UITweener>(curTweeners.Length);
            for (int i = 0; i < curTweeners.Length; i++)
            {
                if (curTweeners[i].enabled)
                {
                    activeTweeners.Add(curTweeners[i]);
                }
            }

            tweeners.ClearArray();
            tweeners.arraySize = activeTweeners.Count;

			for (int i = 0; i < activeTweeners.Count; i++)
            {
                tweeners.GetArrayElementAtIndex(i).objectReferenceValue = activeTweeners[i];
            }

            uiTweenersDirty = false;
        }

        serializedObject.ApplyModifiedProperties();


    }
}
