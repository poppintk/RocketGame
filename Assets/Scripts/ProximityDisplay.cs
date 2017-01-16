using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProximityDisplay : MonoBehaviour {

	public Transform Object1;
	public Transform Object2;

	private Text text;

	void Awake(){
		text = GetComponent<Text>();
	}

	void Update(){
		Vector2 vec = (Object1.transform.position - Object2.transform.position);
		int dist = (int) vec.magnitude;
		text.text = dist + "m";
	}
}
