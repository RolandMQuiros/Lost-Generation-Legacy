using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

[RequireComponent(typeof(MeshFilter))]
public class BoardGridField : MonoBehaviour {
    public float Offset = 0.1f;
    public BoardTheme Theme;

    private MeshFilter _meshFilter;
    private Dictionary<Point, Sprite> _points = new Dictionary<Point, Sprite>();
    private bool _wasChanged = true;

    #region MonoBehaviour
    private void Awake() {
        _meshFilter = GetComponent<MeshFilter>();
    }

    private void LateUpdate() {
        if (_wasChanged) {
            RebuildMesh();
            _wasChanged = false;
            //DebugPrint();
        }
	}
    #endregion MonoBehaviour

    public void AddPoint(Point point, Sprite sprite) {
        _points.Add(point, sprite);
        _wasChanged = true;
    }

    public void AddPoints(IEnumerable<Point> points, Sprite sprite) {
        foreach (Point point in points) {
            _points[point] = sprite;
            _wasChanged = true;
        }
    }

    public void RemovePoint(Point point) {
        _wasChanged = _points.Remove(point);
    }

    public void RemovePoints(IEnumerable<Point> points) {
        foreach (Point point in points) {
            _wasChanged |= _points.Remove(point);
        }
    }

    public void ClearPoints() {
        _points.Clear();
        _wasChanged = true;
    }

    private void RebuildMesh() {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        int triangleOffset = 0;

        float halfWidth = Theme.TileWidth / 2f;
        float halfHeight = Theme.TileHeight / 2f;

        Vector3 up = Theme.PointToVector3(Point.Up) * halfHeight + (Vector3.up * Offset);
        Vector3 left = Theme.PointToVector3(Point.Left) * halfWidth + (Vector3.up * Offset);
        Vector3 down = Theme.PointToVector3(Point.Down) * halfHeight + (Vector3.up * Offset);
        Vector3 right = Theme.PointToVector3(Point.Right) * halfWidth + (Vector3.up * Offset);

        foreach (KeyValuePair<Point, Sprite> pointSprite in _points) {
            Vector3 center = Theme.PointToVector3(pointSprite.Key);
            Vector3 upperLeft = center + up + left;
            Vector3 bottomLeft = center + down + left;
            Vector3 bottomRight = center + down + right;
            Vector3 upperRight = center + up + right;

            vertices.Add(bottomLeft);
            vertices.Add(bottomRight);
            vertices.Add(upperLeft);
            vertices.Add(upperRight);

            uvs.Add(pointSprite.Value.uv[0]);
            uvs.Add(pointSprite.Value.uv[1]);
            uvs.Add(pointSprite.Value.uv[2]);
            uvs.Add(pointSprite.Value.uv[3]);

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
}
