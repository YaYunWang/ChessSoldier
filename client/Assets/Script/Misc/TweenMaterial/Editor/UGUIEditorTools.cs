using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using Entry = PropertyReferenceDrawer.Entry;


public static class UGUIEditorTools
{

    static public void SetLabelWidth(float width)
    {
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
		EditorGUIUtility.LookLikeControls(width);
#else
        EditorGUIUtility.labelWidth = width;
#endif
    }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text) { return DrawHeader(text, text, false, UGUIEditorTools.minimalisticLook); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text, string key) { return DrawHeader(text, key, false, UGUIEditorTools.minimalisticLook); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text, bool detailed) { return DrawHeader(text, text, detailed, !detailed); }

    static public bool DrawHeader(string text, string key, bool forceOn, bool minimalistic)
    {
        bool state = EditorPrefs.GetBool(key, true);

        if (!minimalistic) GUILayout.Space(3f);
        if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
        GUILayout.BeginHorizontal();
        GUI.changed = false;

        if (minimalistic)
        {
            if (state) text = "\u25BC" + (char)0x200a + text;
            else text = "\u25BA" + (char)0x200a + text;

            GUILayout.BeginHorizontal();
            GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);
            if (!GUILayout.Toggle(true, text, "PreToolbar2", GUILayout.MinWidth(20f))) state = !state;
            GUI.contentColor = Color.white;
            GUILayout.EndHorizontal();
        }
        else
        {
            text = "<b><size=11>" + text + "</size></b>";
            if (state) text = "\u25BC " + text;
            else text = "\u25BA " + text;
            if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
        }

        if (GUI.changed) EditorPrefs.SetBool(key, state);

        if (!minimalistic) GUILayout.Space(2f);
        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
        if (!forceOn && !state) GUILayout.Space(3f);
        return state;
    }


    static public void BeginContents() { BeginContents(UGUIEditorTools.minimalisticLook); }

    static bool mEndHorizontal = false;

    /// <summary>
    /// Begin drawing the content area.
    /// </summary>

    static public void BeginContents(bool minimalistic)
    {
        if (!minimalistic)
        {
            mEndHorizontal = true;
            GUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));
        }
        else
        {
            mEndHorizontal = false;
            EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(10f));
            GUILayout.Space(10f);
        }
        GUILayout.BeginVertical();
        GUILayout.Space(2f);
    }

    /// <summary>
    /// End drawing the content area.
    /// </summary>

    static public void EndContents()
    {
        GUILayout.Space(3f);
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        if (mEndHorizontal)
        {
            GUILayout.Space(3f);
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(3f);
    }

    /// <summary>
    /// Draw a list of fields for the specified list of delegates.
    /// </summary>

    static public void DrawEvents(string text, Object undoObject, List<EventDelegate> list)
    {
        DrawEvents(text, undoObject, list, null, null, false);
    }

    /// <summary>
    /// Draw a list of fields for the specified list of delegates.
    /// </summary>

    static public void DrawEvents(string text, Object undoObject, List<EventDelegate> list, bool minimalistic)
    {
        DrawEvents(text, undoObject, list, null, null, minimalistic);
    }

    /// <summary>
    /// Draw a list of fields for the specified list of delegates.
    /// </summary>

    static public void DrawEvents(string text, Object undoObject, List<EventDelegate> list, string noTarget, string notValid, bool minimalistic)
    {
        if (!UGUIEditorTools.DrawHeader(text, text, false, minimalistic)) return;

        if (!minimalistic)
        {
            UGUIEditorTools.BeginContents(minimalistic);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            Field(undoObject, list, notValid, notValid, minimalistic);

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            UGUIEditorTools.EndContents();
        }
        else Field(undoObject, list, notValid, notValid, minimalistic);
    }



    /// <summary>
    /// Create an undo point for the specified objects.
    /// </summary>

    static public void RegisterUndo(string name, params Object[] objects)
    {
        if (objects != null && objects.Length > 0)
        {
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			UnityEditor.Undo.RegisterUndo(objects, name);
#else
            UnityEditor.Undo.RecordObjects(objects, name);
#endif
            foreach (Object obj in objects)
            {
                if (obj == null) continue;
                EditorUtility.SetDirty(obj);
            }
        }
    }

    /// <summary>
    /// Draw 18 pixel padding on the right-hand side. Used to align fields.
    /// </summary>

    static public void DrawPadding()
    {
        if (!UGUIEditorTools.minimalisticLook)
            GUILayout.Space(18f);
    }


    /// <summary>
    /// Draw an editor field for the Unity Delegate.
    /// </summary>

    static public bool Field(Object undoObject, EventDelegate del)
    {
        return Field(undoObject, del, true, UGUIEditorTools.minimalisticLook);
    }

    /// <summary>
    /// Draw an editor field for the Unity Delegate.
    /// </summary>

    static public bool Field(Object undoObject, EventDelegate del, bool removeButton, bool minimalistic)
    {
        if (del == null) return false;
        bool prev = GUI.changed;
        GUI.changed = false;
        bool retVal = false;
        MonoBehaviour target = del.target;
        bool remove = false;

        if (removeButton && (del.target != null || del.isValid))
        {
            if (!minimalistic) UGUIEditorTools.SetLabelWidth(82f);

            if (del.target == null && del.isValid)
            {
                EditorGUILayout.LabelField("Notify", del.ToString());
            }
            else
            {
                target = EditorGUILayout.ObjectField("Notify", del.target, typeof(MonoBehaviour), true) as MonoBehaviour;
            }

            GUILayout.Space(-18f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(70f);

            if (GUILayout.Button("", "ToggleMixed", GUILayout.Width(20f), GUILayout.Height(16f)))
            {
                target = null;
                remove = true;
            }
            GUILayout.EndHorizontal();
        }
        else target = EditorGUILayout.ObjectField("Notify", del.target, typeof(MonoBehaviour), true) as MonoBehaviour;

        if (remove)
        {
            UGUIEditorTools.RegisterUndo("Delegate Selection", undoObject);
            del.Clear();
            EditorUtility.SetDirty(undoObject);
        }
        else if (del.target != target)
        {
            UGUIEditorTools.RegisterUndo("Delegate Selection", undoObject);
            del.target = target;
            EditorUtility.SetDirty(undoObject);
        }

        if (del.target != null && del.target.gameObject != null)
        {
            GameObject go = del.target.gameObject;
            List<Entry> list = GetMethods(go);

            int index = 0;
            string[] names = PropertyReferenceDrawer.GetNames(list, del.ToString(), out index);
            int choice = 0;

            GUILayout.BeginHorizontal();
            choice = EditorGUILayout.Popup("Method", index, names);
            UGUIEditorTools.DrawPadding();
            GUILayout.EndHorizontal();

            if (choice > 0 && choice != index)
            {
                Entry entry = list[choice - 1];
                UGUIEditorTools.RegisterUndo("Delegate Selection", undoObject);
                del.target = entry.target as MonoBehaviour;
                del.methodName = entry.name;
                EditorUtility.SetDirty(undoObject);
                retVal = true;
            }

            GUI.changed = false;
            EventDelegate.Parameter[] ps = del.parameters;

            if (ps != null)
            {
                for (int i = 0; i < ps.Length; ++i)
                {
                    EventDelegate.Parameter param = ps[i];
                    Object obj = EditorGUILayout.ObjectField("   Arg " + i, param.obj, typeof(Object), true);

                    if (GUI.changed)
                    {
                        GUI.changed = false;
                        param.obj = obj;
                        EditorUtility.SetDirty(undoObject);
                    }

                    if (obj == null) continue;

                    GameObject selGO = null;
                    System.Type type = obj.GetType();
                    if (type == typeof(GameObject)) selGO = obj as GameObject;
                    else if (type.IsSubclassOf(typeof(Component))) selGO = (obj as Component).gameObject;

                    if (selGO != null)
                    {
                        // Parameters must be exact -- they can't be converted like property bindings
                        PropertyReferenceDrawer.filter = param.expectedType;
                        PropertyReferenceDrawer.canConvert = false;
                        List<PropertyReferenceDrawer.Entry> ents = PropertyReferenceDrawer.GetProperties(selGO, true, false);

                        int selection;
                        string[] props = GetNames(ents, GetFuncName(param.obj, param.field), out selection);

                        GUILayout.BeginHorizontal();
                        int newSel = EditorGUILayout.Popup(" ", selection, props);
                        UGUIEditorTools.DrawPadding();
                        GUILayout.EndHorizontal();

                        if (GUI.changed)
                        {
                            GUI.changed = false;

                            if (newSel == 0)
                            {
                                param.obj = selGO;
                                param.field = null;
                            }
                            else
                            {
                                param.obj = ents[newSel - 1].target;
                                param.field = ents[newSel - 1].name;
                            }
                            EditorUtility.SetDirty(undoObject);
                        }
                    }
                    else if (!string.IsNullOrEmpty(param.field))
                    {
                        param.field = null;
                        EditorUtility.SetDirty(undoObject);
                    }

                    PropertyReferenceDrawer.filter = typeof(void);
                    PropertyReferenceDrawer.canConvert = true;
                }
            }
        }
        else retVal = GUI.changed;
        GUI.changed = prev;
        return retVal;
    }

    /// <summary>
    /// Draw a list of fields for the specified list of delegates.
    /// </summary>

    static public void Field(Object undoObject, List<EventDelegate> list)
    {
        Field(undoObject, list, null, null, UGUIEditorTools.minimalisticLook);
    }

    /// <summary>
    /// Draw a list of fields for the specified list of delegates.
    /// </summary>

    static public void Field(Object undoObject, List<EventDelegate> list, bool minimalistic)
    {
        Field(undoObject, list, null, null, minimalistic);
    }

    /// <summary>
    /// Draw a list of fields for the specified list of delegates.
    /// </summary>

    static public void Field(Object undoObject, List<EventDelegate> list, string noTarget, string notValid, bool minimalistic)
    {
        bool targetPresent = false;
        bool isValid = false;

        // Draw existing delegates
        for (int i = 0; i < list.Count;)
        {
            EventDelegate del = list[i];

            if (del == null || (del.target == null && !del.isValid))
            {
                list.RemoveAt(i);
                continue;
            }

            Field(undoObject, del, true, minimalistic);
            EditorGUILayout.Space();

            if (del.target == null && !del.isValid)
            {
                list.RemoveAt(i);
                continue;
            }
            else
            {
                if (del.target != null) targetPresent = true;
                isValid = true;
            }
            ++i;
        }

        // Draw a new delegate
        EventDelegate newDel = new EventDelegate();
        Field(undoObject, newDel, true, minimalistic);

        if (newDel.target != null)
        {
            targetPresent = true;
            list.Add(newDel);
        }

        if (!targetPresent)
        {
            if (!string.IsNullOrEmpty(noTarget))
            {
                GUILayout.Space(6f);
                EditorGUILayout.HelpBox(noTarget, MessageType.Info, true);
                GUILayout.Space(6f);
            }
        }
        else if (!isValid)
        {
            if (!string.IsNullOrEmpty(notValid))
            {
                GUILayout.Space(6f);
                EditorGUILayout.HelpBox(notValid, MessageType.Warning, true);
                GUILayout.Space(6f);
            }
        }
    }


    /// <summary>
    /// Collect a list of usable delegates from the specified target game object.
    /// </summary>

    static public List<Entry> GetMethods(GameObject target)
    {
        MonoBehaviour[] comps = target.GetComponents<MonoBehaviour>();

        List<Entry> list = new List<Entry>();

        for (int i = 0, imax = comps.Length; i < imax; ++i)
        {
            MonoBehaviour mb = comps[i];
            if (mb == null) continue;

            MethodInfo[] methods = mb.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);

            for (int b = 0; b < methods.Length; ++b)
            {
                MethodInfo mi = methods[b];

                if (mi.ReturnType == typeof(void))
                {
                    string name = mi.Name;
                    if (name == "Invoke") continue;
                    if (name == "InvokeRepeating") continue;
                    if (name == "CancelInvoke") continue;
                    if (name == "StopCoroutine") continue;
                    if (name == "StopAllCoroutines") continue;
                    if (name == "BroadcastMessage") continue;
                    if (name.StartsWith("SendMessage")) continue;
                    if (name.StartsWith("set_")) continue;

                    Entry ent = new Entry();
                    ent.target = mb;
                    ent.name = mi.Name;
                    list.Add(ent);
                }
            }
        }
        return list;
    }

    /// <summary>
    /// Convert the specified list of delegate entries into a string array.
    /// </summary>

    static public string[] GetNames(List<Entry> list, string choice, out int index)
    {
        index = 0;
        string[] names = new string[list.Count + 1];
        names[0] = "<GameObject>";

        for (int i = 0; i < list.Count;)
        {
            Entry ent = list[i];
            string del = GetFuncName(ent.target, ent.name);
            names[++i] = del;
            if (index == 0 && string.Equals(del, choice))
                index = i;
        }
        return names;
    }

    /// <summary>
    /// Convenience function that converts Class + Function combo into Class.Function representation.
    /// </summary>

    static public string GetFuncName(object obj, string method)
    {
        if (obj == null) return "<null>";
        string type = obj.GetType().ToString();
        int period = type.LastIndexOf('/');
        if (period > 0) type = type.Substring(period + 1);
        return string.IsNullOrEmpty(method) ? type : type + "/" + method;
    }


    /// <summary>
    /// Convenience function that marks the specified object as dirty in the Unity Editor.
    /// </summary>

    static public void SetDirty(UnityEngine.Object obj)
    {
#if UNITY_EDITOR
        if (obj)
        {
            //if (obj is Component) Debug.Log(NGUITools.GetHierarchy((obj as Component).gameObject), obj);
            //else if (obj is GameObject) Debug.Log(NGUITools.GetHierarchy(obj as GameObject), obj);
            //else Debug.Log("Hmm... " + obj.GetType(), obj);
            UnityEditor.EditorUtility.SetDirty(obj);
        }
#endif
    }



    static public bool minimalisticLook
    {
        get
        {
            return GetBool("UGUI Minimallistic", false);
        }
        set
        {
            SetBool("UGUI Minimallistic", false);
        }
    }

    static public bool GetBool(string name, bool defaultValue) { return EditorPrefs.GetBool(name, defaultValue); }

    /// <summary>
    /// Get the previously saved integer value.
    /// </summary>

    static public void SetBool(string name, bool val) { EditorPrefs.SetBool(name, val); }

}
