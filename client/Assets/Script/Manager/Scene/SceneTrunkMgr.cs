using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SceneTrunkDataEntry
{
    public int x;
    public int z;
    public string trunkBundlePath;
    public string trunkName;
}

public class RuntimeSceneTrunkDataEntry
{
    public SceneTrunkDataEntry data;

    public GameObject terrainObj;
    public Terrain terrain;
}

public class SceneTrunkMgr : MonoBehaviour {

    const int TRUNK_CACHE_NUM = 4;

    public int width;
    public int depth;
    public int trunkSize;
    public SceneTrunkDataEntry[] entries;

    public float updateInterval;
    public int cacheTrunkCount = 4;

    private Vector3 origin;
    private float timer;
    private Transform target;
    private RuntimeSceneTrunkDataEntry[] runtimeData;

    private int curTrunkIndex = -1;

    private List<int> loadedTrunks;
    private List<int> loadingTrunks;
    private List<int> significantTrunks;
    private List<int> cachedTrunkIndices;

    private bool dirtyFlag;

    private static List<int> s_tmpNeighbourIndices = new List<int>(4);
    private static List<int> s_tmpIndices = new List<int>();
    private bool initilized = false;

    private ISceneTrunkPolicy loadPolicy = new SceneTrunkNinePolicy();

    public void SetAgent(Transform target)
    {
        this.target = target;
    }

    void Awake()
    {
        this.tag = "SceneTrunkMgr";
    }

    void Start()
    {
        origin = transform.position;
        runtimeData = new RuntimeSceneTrunkDataEntry[width * depth];

        loadedTrunks = new List<int>();
        loadingTrunks = new List<int>();
        significantTrunks = new List<int>();
        cachedTrunkIndices = new List<int>();
        dirtyFlag = true;

        for (int i = 0; i < entries.Length; i++)
        {
            var trunkData = entries[i];
            if (trunkData == null)
                continue;

            runtimeData[trunkData.z * width + trunkData.x] = new RuntimeSceneTrunkDataEntry()
            {
                data = trunkData,
            };
        }

        loadPolicy.Setup(this);

        initilized = true;
    }

    void OnDestroy()
    {
        runtimeData = null;
    }

    void Update()
    {
        if (target == null || !initilized)
            return;

        if (updateInterval <= 0)
        {
            UpdateAgentPos(target.position);
            return;
        }
        else
        {
            timer += Time.deltaTime;
            if (timer > updateInterval)
            {
                timer = 0;
                UpdateAgentPos(target.position);
            }
            return;
        }
    }

    private void UpdateAgentPos(Vector3 pos)
    {
        Vector3 mapPos = pos - origin;

        int xIndex = Mathf.Clamp(Mathf.FloorToInt(mapPos.x / trunkSize), 0, width - 1);
        int zIndex = Mathf.Clamp(Mathf.FloorToInt(mapPos.z / trunkSize), 0, depth - 1);

        int index = zIndex * width + xIndex;

        Vector3 trunkOrigin = new Vector3(xIndex * trunkSize, 0, zIndex * trunkSize);
        Vector3 trunkLocalPos = mapPos - trunkOrigin;

        bool farJump = false;

        if (index != curTrunkIndex)
        {
            if (curTrunkIndex == -1)
            {
                farJump = true;
            }
            else
            {
                farJump = IsNeighbour(index, curTrunkIndex);
            }
            
            LoadTrunk(index);
            curTrunkIndex = index;
        }

        s_tmpNeighbourIndices.Clear();
        loadPolicy.CalcSignificantIndicies(index, trunkLocalPos, ref s_tmpNeighbourIndices);

        for (int i = 0; i < s_tmpNeighbourIndices.Count; i++)
        {
            LoadTrunk(s_tmpNeighbourIndices[i]);
        }

        significantTrunks.Clear();
        significantTrunks.Add(index);
        significantTrunks.AddRange(s_tmpNeighbourIndices);

        if (loadPolicy.CanUnloadInsignificant(trunkLocalPos) || farJump)
        {
            if (dirtyFlag)
            {
                UnloadInsignificantTrunks();
                dirtyFlag = false;
            }
        }
        else
        {
            dirtyFlag = true;
        }
    }

    private bool IsNeighbour(int a, int b)
    {
        if (a == b)
            return true;

        if (GetTopTrunkIndex(a) == b)
            return true;

        if (GetRightTrunkIndex(a) == b)
            return true;

        if (GetBottomTrunkIndex(a) == b)
            return true;

        if (GetLeftTrunkIndex(a) == b)
            return true;

        return false;
    }

    public int GetLeftTrunkIndex(int index)
    {
        int zIndex = index / width;
        int xIndex = index % width;

        //return zIndex * width + Mathf.Clamp(xIndex - 1, 0, width - 1);
        xIndex--;

        if (xIndex < 0)
            return -1;

        return zIndex * width + xIndex;
    }

    public int GetTopTrunkIndex(int index)
    {
        int zIndex = index / width;
        int xIndex = index % width;

        //return Mathf.Clamp(zIndex + 1, 0, depth - 1) * width + xIndex;
        zIndex++;

        if (zIndex >= depth)
            return -1;

        return zIndex * width + xIndex;
    }

    public int GetRightTrunkIndex(int index)
    {
        int zIndex = index / width;
        int xIndex = index % width;

        //return zIndex * width + Mathf.Clamp(xIndex + 1, 0, width - 1);
        xIndex++;

        if (xIndex >= width)
            return -1;

        return zIndex * width + xIndex;
    }

    public int GetBottomTrunkIndex(int index)
    {
        int zIndex = index / width;
        int xIndex = index % width;

        //return Mathf.Clamp(zIndex - 1, 0, depth - 1) * width + xIndex;
        zIndex--;

        if (zIndex < 0)
            return -1;

        return zIndex * width + xIndex;
    }

    private void UnloadInsignificantTrunks()
    {
        s_tmpIndices.Clear();
        s_tmpIndices.AddRange(loadedTrunks);

        for (int i = s_tmpIndices.Count - 1; i >= 0; i--)
        {
            int index = s_tmpIndices[i];

            if (significantTrunks.IndexOf(index) == -1)
                UnloadTrunk(index);
        }

        for (int i = loadingTrunks.Count - 1; i >= 0; i--)
        {
            int index = loadingTrunks[i];

            if (significantTrunks.IndexOf(index) == -1)
                loadingTrunks.Remove(index);
        }
    }

    private void LoadTrunk(int index)
    {
        if (runtimeData[index] == null)
            return;

        if (loadedTrunks.IndexOf(index) != -1)
            return;

        if (loadingTrunks.IndexOf(index) != -1)
            return;

        if (runtimeData[index].terrainObj != null)
        {
            // We have trunk cached. Load it immediatley
            cachedTrunkIndices.Remove(index);
            runtimeData[index].terrainObj.SetActive(true);
            loadedTrunks.Add(index);
        }
        else
        {
            // Load trunk from resource
            loadingTrunks.Add(index);

            StartCoroutine(LoadTerrainTrunk(index));
        }
    }

    private IEnumerator LoadTerrainTrunk(int index)
    {
        RuntimeSceneTrunkDataEntry trunkData = runtimeData[index];

        if (trunkData == null)
        {
            loadedTrunks.Add(index);
            yield break;
        }

        if (string.IsNullOrEmpty(trunkData.data.trunkBundlePath) || string.IsNullOrEmpty(trunkData.data.trunkName))
        {
            loadedTrunks.Add(index);
            yield break;
        }

        loadingTrunks.Add(index);

        var asyncOp = AssetBundleManager.LoadLevelAsync(trunkData.data.trunkBundlePath, trunkData.data.trunkName, true);
        yield return asyncOp;

        // Loading request has been cancelled
        if (loadingTrunks.IndexOf(index) == -1)
            yield break;
        
        GameObject terrainObj = GameObject.Find(string.Format("/Terrain_{0}_{1}", trunkData.data.x, trunkData.data.z));
        runtimeData[index].terrainObj = terrainObj;
        runtimeData[index].terrain = terrainObj.GetComponent<Terrain>();

        loadingTrunks.Remove(index);
        loadedTrunks.Add(index);

        RefreshLodRelation();

        AssetBundleManager.UnloadAssetBundle(trunkData.data.trunkBundlePath);

        yield break;
    }

    private void UnloadTrunk(int index)
    {
        while (cachedTrunkIndices.Count >= TRUNK_CACHE_NUM && cachedTrunkIndices.Count > 0)
        {
            int cacheIndex = cachedTrunkIndices[cachedTrunkIndices.Count - 1];
            cachedTrunkIndices.RemoveAt(cachedTrunkIndices.Count - 1);

            if (runtimeData[cacheIndex].terrainObj == null)
                continue;

            //UnityEngine.SceneManagement.SceneManager.UnloadScene(runtimeData[cacheIndex].data.trunkName);
			UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(runtimeData[cacheIndex].data.trunkName);

            runtimeData[cacheIndex].terrainObj = null;
            runtimeData[cacheIndex].terrain = null;
        }

        loadingTrunks.Remove(index);
        loadedTrunks.Remove(index);

        if (runtimeData[index].terrainObj == null)
            return;

        if (cachedTrunkIndices.Count < TRUNK_CACHE_NUM)
        {
            runtimeData[index].terrainObj.SetActive(false);
            cachedTrunkIndices.Insert(0, index);
        }
        else
        {
            //UnityEngine.SceneManagement.SceneManager.UnloadScene(runtimeData[index].data.trunkName);
			UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(runtimeData[index].data.trunkName);

			runtimeData[index].terrainObj = null;
            runtimeData[index].terrain = null;
        }
    }

    private void RefreshLodRelation()
    {
        for (int i = 0; i < loadedTrunks.Count; i++)
        {
            var t0 = loadedTrunks[i];

            Terrain top = null;
            Terrain right = null;
            Terrain bottom = null;
            Terrain left = null;

            int k;

            k = GetTopTrunkIndex(t0);
            if (k != -1 && runtimeData[k] != null)
            {
                top = runtimeData[k].terrain;
            }

            k = GetRightTrunkIndex(t0);
            if (k != -1 && runtimeData[k] != null)
            {
                right = runtimeData[k].terrain;
            }

            k = GetBottomTrunkIndex(t0);
            if (k != -1 && runtimeData[k] != null)
            {
                bottom = runtimeData[k].terrain;
            }

            k = GetLeftTrunkIndex(t0);
            if (k != -1 && runtimeData[k] != null)
            {
                left = runtimeData[k].terrain;
            }

            runtimeData[t0].terrain.SetNeighbors(left, top, right, bottom);
        }
    }

    public bool HasLoadingTrunk()
    {
        return loadingTrunks.Count > 0;
    }
}
