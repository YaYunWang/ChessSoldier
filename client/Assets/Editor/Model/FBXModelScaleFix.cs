using UnityEngine;
using UnityEditor;
using System.IO;

public class FBXModelScaleFix : AssetPostprocessor
{
    public void OnPreprocessModel()
    {
        string ext = Path.GetExtension(assetPath).ToLower();

        if (ext != ".fbx")
            return;

        ModelImporter modelImporter = assetImporter as ModelImporter;
        if (modelImporter == null)
            return;

        modelImporter.globalScale = 1.0f;
    }
}
