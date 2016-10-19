using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

[RequireComponent(typeof(MeshFilter))]
public class BoardGridField : MonoBehaviour {
    public float Offset = 0.1f;

    public bool IsEmpty {
        get { return _points.Count == 0; }
    }

    private BoardTheme _theme;

    private MeshFilter _meshFilter;
    private Dictionary<Point, Sprite> _points = new Dictionary<Point, Sprite>();
    private bool _wasChanged = true;

    public void Initialize(BoardTheme theme) {
        _theme = theme;
    }

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

    public void RebuildMesh() {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        int triangleOffset = 0;

        float halfWidth = _theme.TileWidth / 2f;
        float halfHeight = _theme.TileHeight / 2f;

        Vector3 up = _theme.PointToVector3(Point.Up) * halfHeight;
        Vector3 left = _theme.PointToVector3(Point.Left) * halfWidth;
        Vector3 down = _theme.PointToVector3(Point.Down) * halfHeight;
        Vector3 right = _theme.PointToVector3(Point.Right) * halfWidth;

        foreach (KeyValuePair<Point, Sprite> pointSprite in _points) {
            Vector3 center = _theme.PointToVector3(pointSprite.Key);
            Vector3 upperLeft = center + up + left + (Vector3.up * Offset);
            Vector3 bottomLeft = center + down + left + (Vector3.up * Offset);
            Vector3 bottomRight = center + down + right + (Vector3.up * Offset);
            Vector3 upperRight = center + up + right + (Vector3.up * Offset);

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

    #region MonoBehaviour
    private void Awake() {
        _meshFilter = GetComponent<MeshFilter>();
    }
    #endregion MonoBehaviour
}
