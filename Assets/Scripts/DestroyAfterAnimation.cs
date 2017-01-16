using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DestroyAfterAnimation : MonoBehaviour {

	private Animator animator;

	void Awake () {
		animator = GetComponent<Animator>();		
	}

	void Start(){
		StartCoroutine(AnimationCheck());
	}

	IEnumerator AnimationCheck(){
		while (true) {
			if (animator.GetCurrentAnimatorStateInfo(0).length <=
				animator.GetCurrentAnimatorStateInfo(0).normalizedTime) {
				Destroy(gameObject);
				break;
			} 
			yield return new WaitForSeconds(0.1f);
		}
	}
}
