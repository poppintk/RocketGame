using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmoothZoom : MonoBehaviour {

	public float TargetZoom;
	public float Damping;

	private Camera cam;
	private float currentVelocity;

	// Use this for initialization
	void Awake () {
		cam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, TargetZoom, ref currentVelocity, Damping);
	}
}
