using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Block Properties", menuName = "Lost Generation/Block Properties")]
public class BlockProperties : ScriptableObject {
	private const int _SIDE_COUNT = 6;
	public AutoTile Top;
	public AutoTile Right;
	public AutoTile Bottom;
	public AutoTile Left;
	public AutoTile Forward;
	public AutoTile Backward;

	public AutoTile[] SideTiles {
		get { return _sideTiles; }
	}

	private AutoTile[] _sideTiles;
	
	private void OnEnable() {
		Setup();
	}

	private void OnValidate() {
		Setup();
	}

	public void Setup() {
		_sideTiles = new AutoTile[] {
			Top, Right, Bottom, Left, Forward, Backward
		};
		for (int i = 0; i < _sideTiles.Length; i++) {
			if (_sideTiles[i] != null)
			{
				_sideTiles[i].Setup();
			}
		}
	}
}
