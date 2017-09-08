using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class TileField : MonoBehaviour {
	[SerializeField]private AutoTile _tile;
	public void Build(IEnumerable<Point> points) {
		if (_tile == null) {
			throw new NullReferenceException("This TileField was not provided an AutoTile"); 
		}
		
		
	} 
}
