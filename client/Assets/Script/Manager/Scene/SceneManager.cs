using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

// 场景管理
// 负责管理Unity中的场景物件
public class SceneManager : ManagerTemplate<SceneManager>
{
    private static SceneTrunkMgr trunkMgr;
	private static AMLoadOperation m_operation;

	public static float progress
	{
		get
		{
			return m_progress;
		}
	}

	private static float m_progress;

	protected override void InitManager()
    {
    }

	public static void ChangeLoginScene(Action sceneLoadComplete = null)
	{
		GameEventManager.RaiseEvent(GameEventTypes.ChangeScene);

		Instance.StartCoroutine(Instance.ChangeLoginInternal(sceneLoadComplete));
	}

	private IEnumerator ChangeLoginInternal(Action sceneLoadComplete = null)
	{
		GameEventManager.RaiseEvent(GameEventTypes.ExitScene);

		string sceneBundleName = string.Format("scenes/scene_login.bundle");

		Resources.UnloadUnusedAssets();

		m_operation = AssetBundleManager.LoadLevelAsync(sceneBundleName, "login", false);

		while (!m_operation.IsDone())
		{
			m_progress = m_operation.Progress;
			yield return null;
		}

		m_operation.UnloadAssetBundle();

		AssetBundleManager.Clear();

		GameEventManager.RaiseEvent(GameEventTypes.EnterScene, 0);

		if(sceneLoadComplete != null)
			sceneLoadComplete();
	}

	public static void ChangeScene(int sceneCfgID, Action sceneLoadComplete = null)
    {
        GameEventManager.RaiseEvent(GameEventTypes.ChangeScene);
        Instance.StartCoroutine(Instance.ReadyChangeScene(sceneCfgID, sceneLoadComplete));
    }

	private IEnumerator ReadyChangeScene(int sceneCfgID, Action sceneLoadComplete = null)
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("empty");

		yield return ChangeSceneInternal(sceneCfgID, sceneLoadComplete);
	}

	private IEnumerator ChangeSceneInternal(int sceneCfgID, Action sceneLoadComplete = null)
    {
        GameEventManager.RaiseEvent(GameEventTypes.ExitScene);

        GameEventManager.EnableEventFiring = false;
        string assetName = ConfigManager.Get<SceneCfgLoader>().GetSceneAssetName(sceneCfgID);

		string sceneBundleName = string.Format("scenes/scene_{0}.bundle", assetName.ToLower());

		m_operation = AssetBundleManager.LoadLevelAsync(sceneBundleName, assetName, false);

		m_progress = 0f;

		while (!m_operation.IsDone())
		{
			m_progress = m_operation.Progress;
			yield return null;
		}

		m_operation.UnloadAssetBundle();

		AssetBundleManager.Clear();

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

        GameEventManager.RaiseEvent(GameEventTypes.EnterScene, sceneCfgID);

		if (sceneLoadComplete != null)
			sceneLoadComplete();
	}

    public static void SetAgent(Transform target)
    {
        if (trunkMgr != null)
            trunkMgr.SetAgent(target);
    }
}
