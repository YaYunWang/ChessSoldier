using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class GameAssetImportProcessor : AssetPostprocessor
{
    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        const string UI_PATH = "Assets/Arts/UI/Panel/";
        const string ICON_PATH = "Assets/Arts/Sprites/";
        const string AUDIO_PATH = "Assets/Arts/Audio/";
        const string EFFECT_PATH = "Assets/Arts/Effects/";
        const string CONTROLLER_PATH = "Assets/Prefabs/Controllers/";

		foreach (string str in importedAssets)
        {
            string ext = Path.GetExtension(str).ToLower();
            var importer = AssetImporter.GetAtPath(str);
            if (importer == null)
                continue;

            if (str.Contains(UI_PATH) && ext == ".prefab")
            {
                importer.assetBundleName = string.Format("ui/{0}.bundle", Path.GetFileNameWithoutExtension(str));
            }
            else if (str.Contains(ICON_PATH) && ext == ".png")
            {
                importer.assetBundleName = string.Format("sprites/{0}.bundle", Path.GetFileNameWithoutExtension(str));
            }
            else if (str.Contains(AUDIO_PATH))
            {
                importer.assetBundleName = string.Format("audio/{0}.bundle", Path.GetFileNameWithoutExtension(str));
            }
            else if (str.Contains(EFFECT_PATH) && ext == ".prefab")
            {
                importer.assetBundleName = string.Format("effects/{0}.bundle", Path.GetFileNameWithoutExtension(str));
            }
            else if (str.Contains(CONTROLLER_PATH) && ext == ".controller")
            {
                importer.assetBundleName = string.Format("controllers/{0}.bundle", Path.GetFileNameWithoutExtension(str));
            }
        }
    }
}
