using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class BoardGridField : MonoBehaviour {
    public BoardView BoardView;
    public Sprite Sprite;

    private MeshFilter _meshFilter;
    private HashSet<Point> _points = new HashSet<Point>();

    public void Awake() {
        BoardView = BoardView ?? GetComponentInParent<BoardView>();
        _meshFilter = GetComponent<MeshFilter>();
    }

    // Use this for initialization
    public void OnEnable() {
        RecreateMesh();
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

            vertices.Add(bottomLeft);
            vertices.Add(bottomRight);
            vertices.Add(upperLeft);
            vertices.Add(upperRight);

            uvs.Add(Sprite.uv[0]);
            uvs.Add(Sprite.uv[1]);
            uvs.Add(Sprite.uv[2]);
            uvs.Add(Sprite.uv[3]);

            triangles.Add(triangleOffset + 2);
            triangles.Add(triangleOffset + 1);
            triangles.Add(triangleOffset);

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

    public void RemovePoint(Point point) {
        _points.Remove(point);
    }

    public void ClearPoints() {
        _points.Clear();
    }
}
