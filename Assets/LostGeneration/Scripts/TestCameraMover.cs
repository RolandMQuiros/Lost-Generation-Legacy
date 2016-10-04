using UnityEngine;
using System;
using System.Collections.Generic;

public class TestCameraMover : MonoBehaviour {
    public BoardCamera Camera; 
    private BoardCursor _cursor;
    private float _zoom = 1f;
    private float _rotate = 0f;
	// Use this for initialization
	void Awake () {
        _cursor = GetComponent<BoardCursor>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(1)) {
            Camera.Pan(_cursor.BoardPoint, 1f);
        }

        if (Input.GetKeyDown(KeyCode.PageDown)) {
            _zoom *= 2f;
            Camera.Zoom(_zoom, 1f);
        }

        if (Input.GetKeyDown(KeyCode.PageUp)) {
            _zoom /= 2f;
            Camera.Zoom(_zoom, 1f);
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            _rotate = Mathf.Repeat(_rotate + 45f, 359f);
            Camera.Rotate(_rotate, 1f);
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            _rotate = Mathf.Repeat(_rotate - 45f, 359f);
            Camera.Rotate(_rotate, 1f);
        }
	}
}
