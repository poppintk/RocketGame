using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValleyGPS : MonoBehaviour {

	public Transform origin;
	public Transform target;

	// Update is called once per frame
	void Update () {
		Vector3 direction = target.position - origin.position;
		direction.z = 0;

		transform.rotation = Quaternion.FromToRotation(
			new Vector3(0, 1, 0),
			direction.normalized);
	}
}
