using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CharacterLightSetup : MonoBehaviour
{
    [ColorUsage(false)]
    public Color color = new Color(1,1,1);

    void OnEnable()
    {
        RefreshLight();
    }

    void RefreshLight()
    {
        Shader.SetGlobalVector("_CharacterLightDir", -transform.forward);
        Shader.SetGlobalColor("_CharacterLightColor", color);
    }

#if UNITY_EDITOR
    void Update ()
    {
        RefreshLight();
    }
#endif
}
