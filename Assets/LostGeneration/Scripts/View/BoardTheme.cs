using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

[Serializable]
public class BoardTheme {
    public GameObject FloorTile;
    public AutoTile WallTile;
    public float TileWidth = 1f;
    public float TileHeight = 1f;

    public Vector3 PointToVector3(Point point) {
        return new Vector3(-point.X * TileWidth, 0f, point.Y * TileHeight);
    }

    public Vector3 Snap(Vector3 vector) {
        return new Vector3(Mathf.Floor(vector.x / TileWidth), vector.y, Mathf.Floor(vector.z / TileHeight));
    }

    public Point Vector3ToPoint(Vector3 vector) {
        return new Point(Mathf.FloorToInt(-vector.x / TileWidth), Mathf.FloorToInt(vector.z / TileHeight));
    }
}
