using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TX;
using TX.Game;

public abstract class SelfRegistration : BaseBehaviour {
	protected abstract HashSet<MonoBehaviour> registry { get; }

	void Awake(){
		registry.Add(this);
	}

	protected void InitializationComplete(){
		this.enabled = false;
		registry.Remove(this);
	}
}

public abstract class SceneInitializer : SelfRegistration {
	protected override HashSet<MonoBehaviour> registry { get { return GameManager.Instance.sceneInitializers; } }
}

public abstract class PostSceneInitializer : SelfRegistration {
	protected override HashSet<MonoBehaviour> registry { get { return GameManager.Instance.postSceneInitializers; } }
}
