using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class BaseEntity
{
	RuntimeAnimatorController baseController;
	AnimatorOverrideController overrideController;

	protected void SetAnimatorController(RuntimeAnimatorController controller)
	{
		baseController = controller;

		if (overrideController == null)
		{
			overrideController = new AnimatorOverrideController();
			overrideController.runtimeAnimatorController = baseController;
		}

		animator.runtimeAnimatorController = overrideController;
	}

	private void DestroyAnimatorController()
	{
		Object.DestroyImmediate(overrideController);
	}

	public bool GetAnimationBool(string name)
	{
		return animator.GetBool(name);
	}

	public void SetAnimationBool(string name, bool val)
	{
		animator.SetBool(name, val);
	}

	public void SetAnimationFloat(string name, float val)
	{
		animator.SetFloat(name, val);
	}

	public void SetAnimationTrigger(string name)
	{
		animator.SetTrigger(name);
	}
}
