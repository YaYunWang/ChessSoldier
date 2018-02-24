using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine;

public abstract partial class BaseEntity : MonoBehaviour
{
	public int ConfigID = 0;
	public bool Ready { set; get; }
	public Renderer[] Renderers;


	private Transform transCache;
	private Transform avatarRoot;
	private Transform avatarModel;
	private Animator animator;

	// 后面如果有NPC的需求，也是继承baseentity，但是会新家一个字段，特殊标记
	protected Avatar PlayerData = null;

	public void InitEntity(Avatar avatar_data)
	{
		this.PlayerData = avatar_data;
		this.Ready = false;
		this.ConfigID = 0;
		this.transCache = transform;

		StartCoroutine(InitEntityInternal());
	}

	protected void SetAvatar(GameObject model)
	{
		avatarModel = model.transform;
		model.transform.SetParent(avatarRoot.transform);
		model.transform.ResetPRS();
	}

	private IEnumerator InitEntityInternal()
	{
		GameObject avatarRootGo = new GameObject("avatar");
		avatarRoot = avatarRootGo.transform;
		avatarRoot.SetParent(transform);
		avatarRoot.ResetPRS();

		yield return StartCoroutine(OnSetupAvatar());

		foreach (var renender in Renderers)
			renender.enabled = false;

		OnSetupLayer();

		if (avatarModel != null)
		{
			animator = avatarModel.GetComponent<Animator>();
		}

		if (animator != null)
		{
			animator.cullingMode = AnimatorCullingMode.CullCompletely;
			animator.applyRootMotion = false;
			yield return StartCoroutine(OnSetupController());
		}

		foreach (var renender in Renderers)
			renender.enabled = true;

		Ready = true;

		OnReady();

		GameEventManager.RaiseEvent(GameEventTypes.EntityCreated, this);
	}

	public virtual string GetProperty(string key)
	{
		return string.Empty;
	}
}
