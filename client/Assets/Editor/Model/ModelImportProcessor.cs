using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class ModelImportProcessor : AssetPostprocessor
{
    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) 
    {
        string path = "Arts/Objects/Act/Role";
        string pathNPC = "Arts/Objects/Act/NPC";
        foreach (string str in importedAssets)
        {
            if (str.Contains(path) || str.Contains(pathNPC))
            {
                
                string ext = Path.GetExtension(str).ToLower();

                if (ext != ".fbx")
                    continue;

                var modelImporter = AssetImporter.GetAtPath(str);
                if (modelImporter == null)
                    continue;

                string assetName = Path.GetFileNameWithoutExtension(str);
                
                if (assetName.Contains("@"))
                {
                    assetName = assetName.Replace("@", "/");
                    modelImporter.assetBundleName = string.Format("animations/{0}.bundle", assetName);

                }
                else
                {
                    modelImporter.assetBundleName = string.Format("models/{0}.bundle", assetName);
                }

            }
        }
    }

}
