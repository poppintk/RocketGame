using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TX;
using TX.Game;

public class Valley : BaseBehaviour {

	public Sprite BuffedSprite;

	public Transform BuffAnimation;

	public void Buff(float delay){
		Invoke("StartBuff", delay);
	}

	[InspectorButton("Buff")]
	void StartBuff(){
		SpriteRenderer renderer = GetComponent<SpriteRenderer>();
		renderer.sprite = BuffedSprite;

		BuffAnimation.gameObject.SetActive(true);
	}
}
