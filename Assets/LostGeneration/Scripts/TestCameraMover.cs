using UnityEngine;
using System;
using System.Collections.Generic;

public class TestCameraMover : MonoBehaviour {
    public BoardCameraController Camera; 
    private BoardCursor _cursor;
	// Use this for initialization
	void Awake () {
        _cursor = GetComponent<BoardCursor>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0)) {
            Camera.AddPan(_cursor.BoardPoint, 1f);
        }

        if (Input.GetKeyDown(KeyCode.Plus)) {
            Camera.AddZoom(0.1f, 1f);
        }

        if (Input.GetKeyDown(KeyCode.Minus)) {
            Camera.AddZoom(0.1f, 1f);
        }
	}
}
