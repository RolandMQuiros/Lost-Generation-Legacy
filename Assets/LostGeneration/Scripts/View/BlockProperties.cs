using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "BlockProperties", menuName = "Lost Generation/BlockProperties")]
public class BlockProperties : ScriptableObject {
	private const int _SIDE_COUNT = 6;
	public AutoTile[] SideTiles = new AutoTile[_SIDE_COUNT];
	
	private void OnValidate() {
		if (SideTiles.Length != _SIDE_COUNT) {
			Array.Resize(ref SideTiles, _SIDE_COUNT);
		}
		SetupTiles();
	}

	private void SetupTiles() {
		for (int i = 0; i < SideTiles.Length; i++) {
			if (SideTiles[i] != null)
			{
				SideTiles[i].SetupTiles();
			}
		}
	}

	private void OnEnable() {
		SetupTiles();
	}
}
