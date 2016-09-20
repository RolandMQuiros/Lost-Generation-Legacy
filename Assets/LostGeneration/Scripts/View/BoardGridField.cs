using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class BoardGridField : MonoBehaviour {
    public BoardView BoardView;
    public BoardCursor Cursor;
    public Rect SpriteUV;

    private MeshFilter _meshFilter;
    private HashSet<Point> _points = new HashSet<Point>();

    public void Awake() {
        _meshFilter = GetComponent<MeshFilter>();
    }

    // Use this for initialization
    public void OnEnable() {
        RecreateMesh();
	}

	// Update is called once per frame
	public void Update () {
	    if (!_points.Contains(Cursor.Point)) {
            _points.Add(Cursor.Point);
            RecreateMesh();
        }
	}

    private void RecreateMesh() {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        int triangleOffset = 0;

        float halfWidth = BoardView.Theme.TileWidth / 2f;
        float halfHeight = BoardView.Theme.TileHeight / 2f;

        Vector3 up = BoardView.Theme.PointToVector3(Point.Up) * halfHeight;
        Vector3 left = BoardView.Theme.PointToVector3(Point.Left) * halfWidth;
        Vector3 down = BoardView.Theme.PointToVector3(Point.Down) * halfHeight;
        Vector3 right = BoardView.Theme.PointToVector3(Point.Right) * halfWidth;

        foreach (Point point in _points) {
            Vector3 center = BoardView.Theme.PointToVector3(point);
            Vector3 upperLeft = center + up + left;
            Vector3 bottomLeft = center + down + left;
            Vector3 bottomRight = center + down + right;
            Vector3 upperRight = center + up + right;

            vertices.Add(upperLeft);
            vertices.Add(bottomLeft);
            vertices.Add(bottomRight);
            vertices.Add(upperRight);

            uvs.Add(new Vector2(SpriteUV.xMin, SpriteUV.yMin));
            uvs.Add(new Vector2(SpriteUV.xMin, SpriteUV.yMax));
            uvs.Add(new Vector2(SpriteUV.xMax, SpriteUV.yMax));
            uvs.Add(new Vector2(SpriteUV.xMax, SpriteUV.yMin));

            triangles.Add(triangleOffset);
            triangles.Add(triangleOffset + 1);
            triangles.Add(triangleOffset + 3);

            triangles.Add(triangleOffset + 3);
            triangles.Add(triangleOffset + 1);
            triangles.Add(triangleOffset + 2);

            triangleOffset += 4;
        }

        Mesh mesh = _meshFilter.mesh;

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    public void AddPoint(Point point) {
        _points.Add(point);
    }
}
