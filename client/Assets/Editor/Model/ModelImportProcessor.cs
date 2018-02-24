using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public class ModelImportProcessor : AssetPostprocessor
{
    public static string[] extraExposedTransforms = new string[]
    {
        "z_zuo",
        "attach_point",
        "effect_point",
        "navel_01",
        "weapon_L_01",
        "weapon_R_01",
        "hand_L_01",
        "hand_R_01",
        "weapon_back_01",
        "weapon_back_02",
        "head_01",
        "body_01",
        "upperArm_L_01",
        "upperArm_R_01",
        "forearm_L_01",
        "forearm_R_01",
        "thigh_L_01",
        "thigh_R_01",
        "calf_L_01",
        "calf_R_01",
        "qunbai_point",
        "equipment_Hand_L_01",
        "equipment_Hand_R_01",
		"Root",
        "head_01",
        "foot_L_01",
        "foot_R_01",
        "hand_L_01",
        "hand_R_01",
        "forearm_R_01",
        "addPoint_01",
    };


    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) 
    {
        string path = "Assets/Arts/Objects/Act/Role/";
        string pathNPC = "Assets/Arts/Objects/Act/NPC/";
        string pathNPC_D = "Assets/Arts/Objects/Act/NPC_d/";
        foreach (string str in importedAssets)
        {
            bool isPlayer = str.StartsWithNonAlloc(path);
            bool isNpc = str.StartsWithNonAlloc(pathNPC);
            bool isNpc_d = str.StartsWithNonAlloc(pathNPC_D);
            if (isPlayer || isNpc || isNpc_d)
            {
                
                string ext = Path.GetExtension(str).ToLower();

                if (ext != ".fbx")
                    continue;

                var importer = AssetImporter.GetAtPath(str);
                if (importer == null)
                    continue;

                string assetName = Path.GetFileNameWithoutExtension(str);

                var modelImporter = importer as ModelImporter;
                
                if (ext == ".fbx" && assetName.Contains("@"))
                {
                    assetName = assetName.Replace("@", "_");
                    importer.assetBundleName = string.Format("animations/anim_{0}.bundle", assetName);
                }
                else if (ext == ".fbx" && assetName.Contains("skeleton"))
                {
                    string rawName = assetName.Substring(0, assetName.IndexOf("_"));
                    importer.assetBundleName = string.Format("models/skeleton_{0}.bundle", rawName);
                    modelImporter.optimizeGameObjects = false;
                }
                else if (ext == ".fbx" && isNpc)
                {
                    importer.assetBundleName = string.Format("models/mdl_{0}.bundle", assetName);

                    List<string> extraTransformList = new List<string>();
                    RetreiveExtraTransformList(extraTransformList, importer as ModelImporter);

                    modelImporter.extraExposedTransformPaths = extraTransformList.ToArray();
                    modelImporter.optimizeGameObjects = true;
                    modelImporter.isReadable = false;
                }
                else if (ext == ".fbx" && isNpc_d)
                {
                    importer.assetBundleName = string.Format("models/mdl_{0}.bundle", assetName);

                    List<string> extraTransformList = new List<string>();
                    RetreiveExtraTransformList(extraTransformList, importer as ModelImporter);

                    modelImporter.extraExposedTransformPaths = extraTransformList.ToArray();
                    modelImporter.optimizeGameObjects = true;
                    modelImporter.isReadable = false;

                    var model = AssetDatabase.LoadAssetAtPath<GameObject>(importer.assetPath);
                    string savePath = Path.GetDirectoryName(importer.assetPath);

                    string matdir = Path.Combine(savePath, "Materials");
                    string[] skinmatFiles = Directory.GetFiles(matdir);

                    string filesavePath;
                    string filename;
                    foreach (var file in skinmatFiles)
                    {
                        string fileext = Path.GetExtension(file).ToLower();
                        if (fileext != ".mat")
                            continue;

                        filename = Path.GetFileNameWithoutExtension(file);
                        string matfix = assetName + "_d";
                        if (!filename.Contains(matfix))
                            continue;

                        var skinmat = AssetDatabase.LoadAssetAtPath<Material>(file);
                        filesavePath = Path.Combine(savePath, "skin_" + filename + ".asset");
                        SaveSkinnedMesh(filesavePath, filename, model, extraTransformList.ToArray(), new Material[] { skinmat });
                    }

                }
                else if (ext == ".fbx" && isPlayer)
                {
                    var model = AssetDatabase.LoadAssetAtPath<GameObject>(importer.assetPath);
                    string savePath = Path.GetDirectoryName(importer.assetPath);
                   // savePath = Path.Combine(savePath, "skin_" + assetName + ".asset");

                    List<string> extraTransformList = new List<string>();
                    //RetreiveExtraTransformList(extraTransformList, importer as ModelImporter);

                    string dir = Path.GetDirectoryName(str);
                    string[] files = Directory.GetFiles(dir);

                    foreach (var file in files)
                    {
                        if (file.Contains("skeleton"))
                        {
                            var skeletonImporter = AssetImporter.GetAtPath(file);
                            RetreiveExtraTransformList(extraTransformList, skeletonImporter as ModelImporter);
                            break;
                        }
                    }

                    string matdir = Path.Combine(savePath, "Materials");
                    string[] skinmatFiles = Directory.GetFiles(matdir);

                    string filesavePath;
                    string filename;
                    foreach (var file in skinmatFiles)
                    {
                        string fileext = Path.GetExtension(file).ToLower();
                        if (fileext != ".mat")
                            continue;

                        filename = Path.GetFileNameWithoutExtension(file);
                        string matfix = assetName + "_d";
                        if (!filename.Contains(matfix))
                            continue;
                        
                        var skinmat = AssetDatabase.LoadAssetAtPath<Material>(file);
                        filesavePath = Path.Combine(savePath, "skin_" + filename + ".asset");
                        SaveSkinnedMesh(filesavePath, filename, model, extraTransformList.ToArray(), new Material[] { skinmat});
                    }
                }
            }
        }
    }

    private static void SaveSkinnedMesh(string savePath, string name, GameObject model, string[] extraTransformNames, Material[] mats)
    {
        var renderer = model.GetComponentInChildren<SkinnedMeshRenderer>(true);
        if (renderer == null)
            return;

        var data = ScriptableObject.CreateInstance<SkinnedMeshData>();

        var bones = renderer.bones;

        string[] bonePaths = new string[bones.Length];

        for (int i = 0; i < bones.Length; i++)
        {
            string bonePath = TransformUtil.GetTransformPath(model.transform, bones[i]);
            bonePaths[i] = bonePath;
        }

        data.bones = bonePaths;
        //data.materials = renderer.sharedMaterials;
        data.materials = mats;
        data.mesh = renderer.sharedMesh;
        data.extraTransformNames = extraTransformNames;

        AssetDatabase.CreateAsset(data, savePath);

        AssetImporter.GetAtPath(savePath).assetBundleName = string.Format("models/skin_{0}.bundle", name);
    }



    private static void RetreiveExtraTransformList(List<string> list, ModelImporter modelImporter)
    {
        foreach (var fullName in modelImporter.transformPaths)
        {
            var name = System.IO.Path.GetFileNameWithoutExtension(fullName);

            foreach (var exposedName in extraExposedTransforms)
            {
                if (exposedName == name)
                {
                    list.Add(exposedName);
                }
            }
        }
    }
}
