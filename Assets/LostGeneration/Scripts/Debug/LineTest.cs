using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LostGen.Model;
using LostGen.Display;

public class LineTest : MonoBehaviour {
	private BlockMesh _mesh;
	// Use this for initialization
	private void Awake () {
		_mesh = GetComponent<BlockMesh>();
		_mesh.Resize(100 * Point.One);
	}
	
	public void OnCursorMove(Point to) {
		_mesh.Clear();
		foreach (Point point in Point.Line(Point.Zero, to)) {
			_mesh.SetBlock(point, 1);
		}
		_mesh.Build();
	}
}
