using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TX;
using TX.Game;

public class Bird : BaseBehaviour {
	[Range(0, 10)]
	public float PatrolSpeed;

	[Range(1, 50)]
	public float PatrolRange;

	public float Mass;

	public AudioClip HitSound;

	private AudioSource sfxSource;
	private SpriteRenderer sprite;
	private float patrolCenter;

	void Awake(){
		patrolCenter = transform.position.x + PatrolRange / 2;
		sprite = GetComponent<SpriteRenderer> ();

		sfxSource = GameManager.Instance.GetComponent<AudioSource> ();
	}

	// Update is called once per frame
	void Update () {
		Vector3 position = transform.position;
		float deltaX = PatrolSpeed * Time.smoothDeltaTime;

		if (sprite.flipX) {
			position.x -= deltaX;
		} else {
			position.x += deltaX;
		}

		transform.position = position;

		// turn around
		if (position.x < patrolCenter - PatrolRange / 2)
			sprite.flipX = false;
		if (position.x > patrolCenter + PatrolRange / 2)
			sprite.flipX = true;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player" && this.enabled) {
			// stick to the rocket...

			Rigidbody2D rocket = other.GetComponent<Rigidbody2D> ();
			rocket.mass += Mass;

			transform.SetParent (other.transform);
			transform.localPosition = 
				new Vector3(
					transform.localPosition.x,
					transform.localPosition.y,
					Random.value > 0.5 ? -1 : 1);
			this.enabled = false;
			GetComponent<EnforceBoundary> ().enabled = false;

			sfxSource.clip = HitSound;
			sfxSource.Play ();
		}
	}
}
