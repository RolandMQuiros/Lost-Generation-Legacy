using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class LineTest : MonoBehaviour {
	private BlockMesh _mesh;
	// Use this for initialization
	private void Awake () {
		_mesh = GetComponent<BlockMesh>();
	}
	
	public void OnCursorMove(Point to) {
		_mesh.Resize(to);
		foreach (Point point in Point.Line3D(Point.Zero, to)) {
			_mesh.SetBlock(point, 1);
		}
		_mesh.Build();
	}
}
