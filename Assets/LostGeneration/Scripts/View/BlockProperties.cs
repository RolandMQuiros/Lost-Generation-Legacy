using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Block Properties", menuName = "Lost Generation/Block Properties")]
public class BlockProperties : ScriptableObject,
							   IBlockProperties {
	private const int _SIDE_COUNT = 6;
	[SerializeField]
	public AutoTile _top;
	[SerializeField]
	public AutoTile _right;
	[SerializeField]
	public AutoTile _bottom;
	[SerializeField]
	public AutoTile _left;
	[SerializeField]
	public AutoTile _forward;
	[SerializeField]
	public AutoTile _backward;

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
			_top, _right, _bottom, _left, _forward, _backward
		};
		for (int i = 0; i < _sideTiles.Length; i++) {
			if (_sideTiles[i] != null)
			{
				_sideTiles[i].Setup();
			}
		}
	}
}
