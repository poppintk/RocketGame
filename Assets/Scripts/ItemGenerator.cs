using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TX;
using TX.Game;

public class ItemGenerator : SceneInitializer {
	
	public GameObject[] Prefabs;

	[MinMaxRange(0f, 1f, 0.001f)]
	public Rangef AltitudeRange;

	public float MinSize;
	public float MaxSize;

	[Range(0f, 0.01f)]
	public float Density;
	public bool Mirroring;

	private Transform GameMap;
	private Rect Region;

	void Awake() {
		Region = Rect.MinMaxRect(
			GameMetadata.MapRegion.xMin, GameMetadata.MapSize.y * AltitudeRange.min,
			GameMetadata.MapRegion.xMax, GameMetadata.MapSize.y * AltitudeRange.max);
	}

	// Use this for initialization
	void Start () {
		Generate();
	}

	void Generate(){
		int itemCount = (int)(Density * Region.Area());

		for (int i = 0; i < itemCount; i++) {

			// randomly select an item from the list of templates
			int randomSelection = Random.Range(0, Prefabs.Length);
			GameObject prefab = Prefabs[randomSelection];
			Transform item = transform.InstantiateAsChild(prefab);

			// alter position
			Vector2 position = new Vector2(
				Random.Range(Region.xMin, Region.xMax),
				Random.Range(Region.yMin, Region.yMax));
			item.localPosition = position; 

			// alter size and mirroring
			Vector2 size = Vector2.one * (Random.Range(MinSize, MaxSize));
			if (Mirroring) {
				if (Random.value > 0.5) {
					SpriteRenderer itemSprite = item.GetComponent<SpriteRenderer>();
					itemSprite.flipX = true;
				}
			}
			item.localScale = new Vector3(size.x, size.y, 1);
		}

		InitializationComplete();
	}
}
