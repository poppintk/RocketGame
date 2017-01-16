using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TX;
using TX.Game;

public class SeamlessBackground : PostSceneInitializer {
	public Transform Background;

	void Update(){
		if (GameManager.Instance.State == GameState.PostLoading) {
			
			Transform left = transform.InstantiateAsChild(Background.gameObject, "FakeBackground_Left");
			left.position = new Vector3(
				left.position.x - GameMetadata.MapSize.x,
				left.position.y,
				left.position.z);

			Transform right = transform.InstantiateAsChild(Background.gameObject, "FakeBackground_Right");
			right.position = new Vector3(
				right.position.x + GameMetadata.MapSize.x,
				right.position.y,
				right.position.z);

			InitializationComplete();
		}
	}
}
