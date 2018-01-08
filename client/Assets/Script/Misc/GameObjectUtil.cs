using UnityEngine;
using System.Collections;

public static class GameObjectUtil {

    public static T SafeAddComponent<T>(this GameObject go) where T : Component
    {
        var c = go.GetComponent<T>();
        if (c == null)
        {
            c = go.AddComponent<T>();
        }

        return c;
    }
}
