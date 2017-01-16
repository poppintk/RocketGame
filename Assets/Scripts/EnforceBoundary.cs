using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TX;
using TX.Game;

public class EnforceBoundary : BaseBehaviour {

	public float Padding;

	private float LeftBound;
	private float RightBound;

	void Awake() {
		LeftBound = GameMetadata.MapRegion.xMin - Padding;
		RightBound = GameMetadata.MapRegion.xMax + Padding;
	}


	// Update is called once per frame
	void Update () {
		Vector3 position = transform.position;

		while (position.x > RightBound)
			position.x -= GameMetadata.MapSize.x;
		
		while (position.x < LeftBound)
			position.x += GameMetadata.MapSize.x;
			
		transform.position = position;
	}
}
