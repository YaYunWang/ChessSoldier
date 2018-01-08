using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using System.Text;

public class SceneProcessor
{
    class TerrainTrunk
    {
        public int xIndex;
        public int zIndex;
        public Vector3 localPos;
        public Vector3 worldPos;
        public Terrain terrian;
        public List<Transform> bigAssetTrans;
        public List<Transform> smallAssetTrans;
    }

    [MenuItem("Assets/Scene/Split Scene (切分场景)")]
    public static void SplitSelectScene()
    {
        SceneAsset sceneAsset = Selection.activeObject as SceneAsset;
        var scene = EditorSceneManager.OpenScene(AssetDatabase.GetAssetOrScenePath(sceneAsset), OpenSceneMode.Additive);

        var terrainRootGo = GameObject.Find("/Terrain");

        try
        {
            if (terrainRootGo == null)
                return;

            var terrainRoot = terrainRootGo.transform;

            //Vector3 rootPos = terrainRoot.position;
            int width = 0;
            int depth = 0;
            int size = 0;

            List<TerrainTrunk> trunks;

            GetTrunksSettings(terrainRoot, out width, out depth, out size, out trunks);

            // Group assets
            var bigAssetRoot = GameObject.Find("Asset_Big");
            if (bigAssetRoot != null)
                GroupBigAssets(bigAssetRoot.transform, trunks, size);

            var smallAssetRoot = GameObject.Find("Asset_Small");
            if (smallAssetRoot != null)
                GroupSmallAssets(smallAssetRoot.transform, trunks, size);

            // Create terrain trunks
            var sceneTrunkData = terrainRootGo.SafeAddComponent<SceneTrunkMgr>();
            sceneTrunkData.width = width;
            sceneTrunkData.depth = depth;
            sceneTrunkData.trunkSize = size;
            sceneTrunkData.entries = new SceneTrunkDataEntry[trunks.Count];

            SaveTrunksAsScenes(scene.name, sceneTrunkData, trunks);

            // Destroy terrains
            while (terrainRoot.childCount > 0)
            {
                var child = terrainRoot.GetChild(terrainRoot.childCount - 1);
                Object.DestroyImmediate(child.gameObject);
            }

            SaveScene(scene);
        }
        finally
        {
            EditorSceneManager.CloseScene(scene, true);
        }
    }

    [MenuItem("Assets/Scene/Split Scene (切分场景)", validate = true)]
    public static bool SplitSelectSceneValidation()
    {
        if (Selection.activeObject != null && Selection.activeObject.GetType() == typeof(SceneAsset))
        {
            return true;
        }

        return false;
    }

    private static void SaveTrunksAsScenes(string sceneName, SceneTrunkMgr sceneTrunkMgr, List<TerrainTrunk> trunks)
    {
        string sceneSubDir = string.Format("Assets/Scenes/{0}_Trunks", sceneName);
        if (System.IO.Directory.Exists(sceneSubDir))
        {
            System.IO.Directory.Delete(sceneSubDir, true);
        }

        System.IO.Directory.CreateDirectory(sceneSubDir);

        for (int i = 0; i < trunks.Count; i++)
        {
            var trunk = trunks[i];

            string trunkSceneName = string.Format("{0}_{1}_{2}", sceneName, trunk.xIndex, trunk.zIndex);
            string trunkScenePath = string.Format("{0}/{1}.unity", sceneSubDir, trunkSceneName);

            var trunkScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            EditorSceneManager.SetActiveScene(trunkScene);

            trunk.terrian.transform.parent = null;
            EditorSceneManager.MoveGameObjectToScene(trunk.terrian.gameObject, trunkScene);

            EditorSceneManager.SaveScene(trunkScene, trunkScenePath);
            EditorSceneManager.CloseScene(trunkScene, true);

            var trunkAsset = AssetImporter.GetAtPath(trunkScenePath);
            var trunkName = string.Format("trunk_{0}_{1}.bundle", trunk.xIndex, trunk.zIndex);

            trunkAsset.assetBundleName = string.Format("scenes/{0}/{1}", sceneName, trunkName);

            sceneTrunkMgr.entries[i] = new SceneTrunkDataEntry()
            {
                x = trunk.xIndex,
                z = trunk.zIndex,
                trunkBundlePath = trunkAsset.assetBundleName,
                trunkName = trunkSceneName,
            };
        }
    }

    private static void GetTrunksSettings(Transform terrainRoot, out int width, out int depth, out int size, out List<TerrainTrunk> trunks)
    {
        width = 0;
        depth = 0;
        size = 0;
        trunks = new List<TerrainTrunk>();

        foreach (Transform terrainCell in terrainRoot)
        {
            string[] nameSliced = terrainCell.name.Split('_');
            if (nameSliced.Length < 3)
                continue;

            if (nameSliced[0].ToLower() != "terrain")
                continue;

            int x;
            int z;

            if (!int.TryParse(nameSliced[1], out x))
            {
                Debug.LogWarningFormat("Can not parse terrain trunk name {0}", terrainCell.name);
                continue;
            }

            if (!int.TryParse(nameSliced[2], out z))
            {
                Debug.LogWarningFormat("Can not parse terrain trunk name {0}", terrainCell.name);
                continue;
            }

            Terrain terrain = terrainCell.GetComponent<Terrain>();

            if (size == 0 && terrain != null)
            {
                size = (int)terrain.terrainData.size.x;
            }

            if (x > width)
                width = x;

            if (z > depth)
                depth = z;

            TerrainTrunk trunk = new TerrainTrunk();

            trunk.xIndex = x;
            trunk.zIndex = z;
            trunk.localPos = terrainCell.localPosition;
            trunk.worldPos = terrainCell.position;
            trunk.terrian = terrain;

            trunks.Add(trunk);
        }

        width++;
        depth++;
    }

    private static void SaveScene(UnityEngine.SceneManagement.Scene scene)
    {
        string scenePath = string.Format("Assets/Scenes/{0}.unity", scene.name);

        EditorSceneManager.SaveScene(scene, scenePath, true);

        AssetDatabase.Refresh(ImportAssetOptions.Default);

        var sceneImporter = AssetImporter.GetAtPath(scenePath);
        sceneImporter.assetBundleName = string.Format("scenes/{0}.bundle", scene.name);
    }

    private static void GroupBigAssets(Transform assetRoot, List<TerrainTrunk> trunks, float trunkSize)
    {
        int childCount = assetRoot.childCount;

        for (int i = 0; i < childCount; i++)
        {
            var child = assetRoot.GetChild(i);

            float minDist = float.MaxValue;
            int minDistTrunk = -1;

            Vector3 pos = child.position;

            for (int j = 0; j < trunks.Count; j++)
            {
                float dist = Vector3.Distance(pos, trunks[j].worldPos + new Vector3(trunkSize * 0.5f, 0, trunkSize * 0.5f));

                if (dist < minDist)
                {
                    minDist = dist;
                    minDistTrunk = j;
                }
            }

            if (trunks[minDistTrunk].bigAssetTrans == null)
            {
                trunks[minDistTrunk].bigAssetTrans = new List<Transform>();
            }

            trunks[minDistTrunk].bigAssetTrans.Add(child);
        }

        foreach (var trunk in trunks)
        {
            if (trunk.bigAssetTrans == null)
                continue;

            GameObject bigAssetRootGo = new GameObject("BigAssets");
            Transform bigAssetRoot = bigAssetRootGo.transform;

            bigAssetRoot.SetParent(trunk.terrian.transform);
            bigAssetRoot.localPosition = Vector3.zero;

            foreach (var asset in trunk.bigAssetTrans)
            {
                asset.SetParent(bigAssetRoot, true);
            }
        }
    }

    private static void GroupSmallAssets(Transform assetRoot, List<TerrainTrunk> trunks, float trunkSize)
    {
        int childCount = assetRoot.childCount;

        for (int i = 0; i < childCount; i++)
        {
            var child = assetRoot.GetChild(i);

            float minDist = float.MaxValue;
            int minDistTrunk = -1;

            Vector3 pos = child.position;

            for (int j = 0; j < trunks.Count; j++)
            {
                float dist = Vector3.Distance(pos, trunks[j].worldPos + new Vector3(trunkSize * 0.5f, 0, trunkSize * 0.5f));

                if (dist < minDist)
                {
                    minDist = dist;
                    minDistTrunk = j;
                }
            }

            if (trunks[minDistTrunk].smallAssetTrans == null)
            {
                trunks[minDistTrunk].smallAssetTrans = new List<Transform>();
            }

            trunks[minDistTrunk].smallAssetTrans.Add(child);
        }

        foreach (var trunk in trunks)
        {
            if (trunk.smallAssetTrans == null)
                continue;

            GameObject smallAssetRootGo = new GameObject("SmallAssets");
            Transform smallAssetRoot = smallAssetRootGo.transform;

            smallAssetRoot.SetParent(trunk.terrian.transform);
            smallAssetRoot.localPosition = Vector3.zero;

            foreach (var asset in trunk.smallAssetTrans)
            {
                asset.SetParent(smallAssetRoot, true);
            }
        }
    }
}
