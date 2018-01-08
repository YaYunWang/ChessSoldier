using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

// 场景管理
// 负责管理Unity中的场景物件
public class SceneManager : ManagerTemplate<SceneManager>
{
    private static SceneTrunkMgr trunkMgr;

    protected override void InitManager()
    {
    }

    public static void ChangeScene(int sceneCfgID)
    {
        Instance.StartCoroutine(Instance.ChangeSceneInternal(sceneCfgID));
    }

    private IEnumerator ChangeSceneInternal(int sceneCfgID)
    {
        GameEventManager.RaiseEvent(GameEventTypes.ExitScene);

        GameEventManager.EnableEventFiring = false;
        string assetName = ConfigManager.Get<SceneCfgLoader>().GetSceneAssetName(sceneCfgID);

        yield return AssetBundleManager.LoadLevelAsync(string.Format("scenes/{0}.bundle", assetName), assetName, false);

        GameEventManager.EnableEventFiring = true;

        AssetBundleManager.UnloadAssetBundle(string.Format("scenes/{0}.bundle", assetName));

        var trunkMgrGo = GameObject.FindGameObjectWithTag("SceneTrunkMgr");

        if (trunkMgrGo != null)
            trunkMgr = trunkMgrGo.GetComponent<SceneTrunkMgr>();

        // 等待主角周围地块加载完毕
        if (trunkMgr != null)
        {
            while (trunkMgr.HasLoadingTrunk())
            {
                yield return null;
            }
        }
        GameEventManager.RaiseEvent(GameEventTypes.EnterScene);
    }

    public static void SetAgent(Transform target)
    {
        if (trunkMgr != null)
            trunkMgr.SetAgent(target);
    }
}
