using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class BoardPathView : MonoBehaviour {
    private List<Point> _path = new List<Point>();


    public void PushPoint(Point point) {
        _path.Add(point);
    }

    public void Clear() {
        _path.Clear();
    }
}
