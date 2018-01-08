using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorReplay : MonoBehaviour {

    public Animator animator;

	void Start ()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
	}


    private void OnDisable()
    {
        if (animator != null)
        {
            var state = animator.GetCurrentAnimatorStateInfo(0);
            animator.Play(state.shortNameHash, 0, 0);
        }
    }
}
