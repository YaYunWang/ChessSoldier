using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TransformUtil {
    public static void ResetPRS(this Transform trans)
    {
        trans.localPosition = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = Vector3.one;
    }

    public static Transform Search(this Transform trans, string name)
    {
        if (trans.name == name)
            return trans;

        int count = trans.childCount;

        for (int i = 0; i < count; i++)
        {
            Transform child = trans.GetChild(i);

            Transform found = Search(child, name);
            if (found != null)
                return found;
        }

        return null;
    }

    public static void SetChildActive(this Transform trans, int startIndex, int length)
    {
        int count = trans.childCount;
        int endIndex = startIndex + length;
        if (endIndex > count)
            endIndex = count;

        for (int i = 0; i < count; i++)
        {
            Transform child = trans.GetChild(i);
            if (child != null)
            {
                if (i >= startIndex && i < endIndex)
                    child.gameObject.SetActive(true);
                else
                    child.gameObject.SetActive(false);
            }
        }
    }

    public static void SetChildActive(this Transform trans, int startIndex)
    {
        int count = trans.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform child = trans.GetChild(i);
            if (child != null)
            {
                if (i >= startIndex)
                    child.gameObject.SetActive(false);
                else
                    child.gameObject.SetActive(true);
            }
        }
    }

    public static string GetTransformPath(Transform root, Transform node)
    {
        List<string> nameList = new List<string>();
        GetTransformPath(nameList, root, node);
        nameList.Reverse();
        return string.Join("/", nameList.ToArray());
    }

    private static void GetTransformPath(List<string> nameList, Transform root, Transform node)
    {
        if (node.parent == null || node == root)
            return;

        nameList.Add(node.name);

        GetTransformPath(nameList, root, node.parent);
    }
}
