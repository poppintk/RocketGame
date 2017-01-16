using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TX;
using TX.Game;

public class ChainExplosion : BaseBehaviour {

	public GameObject SubExplosion;

	public float Scattering;
	public float Interval;
	public int Count;

	void Start() {
		StartCoroutine(SpawnSubExplosions());
	}

	IEnumerator SpawnSubExplosions(){
		for (int i = 0; i < Count; i++) {
			float angle = Random.Range(0, Mathf.PI * 2);
			float distance = Random.Range(0, Scattering);

			var explosionTransform = transform.InstantiateAsChild(SubExplosion);
			explosionTransform.localPosition = new Vector3(
				distance * Mathf.Cos(angle),
				distance * Mathf.Sin(angle),
				0);

			yield return new WaitForSeconds(Interval);
		}
		StartCoroutine(WaitForChildren());
	}

	IEnumerator WaitForChildren(){
		while (transform.childCount > 0) {
			yield return new WaitForSeconds(0.5f);
		}
		Destroy(gameObject);
	}
}
