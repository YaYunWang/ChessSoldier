using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class BaseEntity
{
    private int entityLayer;

	private List<AMLoadOperation> loadOps = new List<AMLoadOperation>(5);

	/// <summary>
	/// 重载 CreateAvatar() 创建模型
	/// </summary>
	/// <returns></returns>
	protected abstract IEnumerator OnSetupAvatar();


	protected abstract void OnSetupLayer();


	protected abstract IEnumerator OnSetupController();


	protected virtual void OnReady()
	{
	}

	public void SetLayer(int layer)
	{
		entityLayer = layer;
		SetLayerInternal(entityLayer);
	}

	private void SetLayerInternal(int layer)
	{
		gameObject.layer = layer;

		if (Renderers == null || Renderers.Length == 0)
		{
			return;
		}

		for (int i = 0; i < Renderers.Length; i++)
		{
			var renderer = Renderers[i];
			if (renderer == null)
				continue;

			renderer.gameObject.layer = layer;
		}
	}


	protected void AutoUnloadAsset(AMLoadOperation loadOp)
	{
		loadOps.Add(loadOp);
	}

	private void UnloadAssets()
	{
		for (int i = 0; i < loadOps.Count; i++)
		{
			loadOps[i].UnloadAssetBundle();
		}

		loadOps.Clear();
	}

	protected void OnRendererChange()
	{
		Renderers = null;
		Renderers = avatarRoot.GetComponentsInChildren<Renderer>(true);
	}
}
