using UnityEngine;
using UnityEngine.Profiling;
using System.Collections;
using System;
using System.Collections.Generic;

public class PlayerEntity : BaseEntity
{
    private RoleKeyCfgLoader roleKeyCfgLoader = ConfigManager.Get<RoleKeyCfgLoader>();

	private GameObject playerSkeleton = null;
	private string lastSkinName = null;
	private SkinnedMeshRenderer skinnedMeshRenderer = null;

	protected override void OnSetupLayer()
	{
		int layer = LayerMask.NameToLayer("Player");
		SetLayer(layer);
	}

	protected override IEnumerator OnSetupAvatar()
	{
		string resPath = null;

		int roleKey = PlayerData.roleType;
		RoleKeyCfg roleKeyCfg = roleKeyCfgLoader.GetConfig(roleKey);

		string skeletonName = roleKeyCfg.ShowBone;
		resPath = string.Format("models/skeleton_{0}.bundle", skeletonName);

		var request = AssetBundleManager.LoadAssetAsync(resPath, skeletonName + "_skeleton", typeof(GameObject));

		AutoUnloadAsset(request);
		yield return request;

		if (request == null)
			yield break;

		GameObject prefab = request.GetAsset<GameObject>();
		Profiler.BeginSample("EntityInstantiate");
		playerSkeleton = Instantiate(prefab) as GameObject;
		Profiler.EndSample();

		OnBodyChange();

		SetAvatar(playerSkeleton);
	}

	private void OnBodyChange()
	{
		int roleKey = PlayerData.roleType;
		RoleKeyCfg roleKeyCfg = roleKeyCfgLoader.GetConfig(roleKey);

		string skinName = roleKeyCfg.BornBody;

		if (skinName == lastSkinName)
			return;

		lastSkinName = skinName;

		if (skinnedMeshRenderer == null)
		{
			skinnedMeshRenderer = playerSkeleton.AddComponent<SkinnedMeshRenderer>();
		}
		else
		{
			DestroyImmediate(skinnedMeshRenderer);
			skinnedMeshRenderer = playerSkeleton.AddComponent<SkinnedMeshRenderer>();
		}

		AnimatorUtility.DeoptimizeTransformHierarchy(playerSkeleton);

		string resPath = string.Format("models/skin_{0}.bundle", skinName);

		//添加个使用默认皮肤
		SkinnedMeshData skinData = null;
		skinData = AssetBundleManager.LoadAsset<SkinnedMeshData>(resPath, "skin_" + skinName);

		Transform[] bones = new Transform[skinData.bones.Length];

		Transform modelTrans = playerSkeleton.transform;

		for (int i = 0; i < skinData.bones.Length; i++)
		{
			var bonePath = skinData.bones[i];
			bones[i] = modelTrans.Find(bonePath);
			if (bones[i] == null)
			{
				Debug.LogWarningFormat("Can not find bone {0} on {1}", bonePath, skinName);
			}
		}

		skinnedMeshRenderer.bones = bones;
		skinnedMeshRenderer.sharedMaterials = skinData.materials;
		skinnedMeshRenderer.sharedMesh = skinData.mesh;

		AnimatorUtility.OptimizeTransformHierarchy(playerSkeleton, skinData.extraTransformNames);

		OnRendererChange();
	}

	protected override IEnumerator OnSetupController()
	{
		string resPath = null;
		string controllerName = null;

		int roleKey = PlayerData.roleType;
		RoleKeyCfg roleKeyCfg = roleKeyCfgLoader.GetConfig(roleKey);
		if (roleKeyCfg != null)
		{
			string conroller = roleKeyCfg.Controller;
			resPath = string.Format("controllers/{0}.bundle", conroller);
			controllerName = conroller;
		}

		var request = AssetBundleManager.LoadAssetAsync(resPath, controllerName, typeof(RuntimeAnimatorController));
		AutoUnloadAsset(request);
		yield return request;

		var controller = request.GetAsset<RuntimeAnimatorController>();

		SetAnimatorController(controller);
	}
}
