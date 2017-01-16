using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using TX;
using TX.Game;
using UnityStandardAssets._2D;

public enum GameState
{
	Loading,
	PostLoading,
	Ready,
	InGame,
	GameOver
}

public class GameManager : Singleton<GameManager> {

	public GameState State { get; private set; }

	public HashSet<MonoBehaviour> sceneInitializers = new HashSet<MonoBehaviour>();

	public HashSet<MonoBehaviour> postSceneInitializers = new HashSet<MonoBehaviour>();

	public Camera MainCamera;
	public GameObject Rocket;
	public UnityEvent GameOverEvent;

	public Transform Destination;

	public Transform ScoreRegion;
	public Transform FailRegion;

	public AudioClip WinningSound;

	public void GameOver(bool won){
		if (GameState.InGame == State) {
			State = GameState.GameOver;
			GameOverEvent.Invoke ();
			if (won) {
				MainCamera.GetComponent<Camera2DFollow>().target = Destination;
				MainCamera.GetComponent<CameraSmoothZoom>().TargetZoom = 25;
				MainCamera.GetComponent<AudioSource> ().mute = true;
				Destination.GetComponent<Valley>().Buff(1);
				Rocket.gameObject.SetActive(false);

				var audioSrc = GetComponent<AudioSource> ();
				audioSrc.clip = WinningSound;
				audioSrc.Play ();
			}
		}
	}

	public void GameStart(){
		if (GameState.Ready == State) {
			State = GameState.InGame;
			MainCamera.GetComponent<CameraSmoothZoom>().TargetZoom = 20;
		}
	}

	void Awake() {
		State = GameState.Loading;
		Rocket = GameObject.FindWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		switch (State) {
		case GameState.Loading:
			if (sceneInitializers.Count == 0) {
				State = GameState.PostLoading;
			}
			break;

		case GameState.PostLoading:
			if (postSceneInitializers.Count == 0) {
				MainCamera.GetComponent<CameraSmoothZoom>().TargetZoom = 5;
				State = GameState.Ready;
			}
			break;

		case GameState.InGame:
			break;
		case GameState.GameOver:
			break;
		}
	}
}

public static class GameMetadata {
	public static readonly Vector2 MapSize = new Vector2(100, 2560);
	public static readonly Rect MapRegion = 
		Rect.MinMaxRect(
			-MapSize.x/2, 0,
			MapSize.x/2, MapSize.y);
}
