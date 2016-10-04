using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

[RequireComponent(typeof(BoardView))]
[RequireComponent(typeof(MeshFilter))]
public class BoardGridField : MonoBehaviour {
    public float Offset = 0.1f;

    private BoardView _boardView;
    private MeshFilter _meshFilter;
    private Dictionary<Point, Sprite> _points = new Dictionary<Point, Sprite>();
    private bool _wasChanged = true;

    public void Awake() {
        _boardView = GetComponent<BoardView>();
        _meshFilter = GetComponent<MeshFilter>();
    }

    public void Start() {
        _boardView = _boardView ?? GetComponentInParent<BoardView>();
    }

    public void LateUpdate() {
        if (_wasChanged) {
            RebuildMesh();
            _wasChanged = false;
            //DebugPrint();
        }
	}

    private void DebugPrint() {
        string boardPrint = string.Empty;
        for (int y = 0; y < _boardView.Board.Height; y++) {
            string line = string.Empty;
            for (int x = 0; x < _boardView.Board.Width; x++) {
                Point p = new Point(x, y);
                if (_points.ContainsKey(p)) {
                    line += '╬';
                } else {
                    switch (_boardView.Board.GetTile(p)) {
                        case Board.WALL_TILE: line += '█'; break;
                        case Board.FLOOR_TILE: line += '░'; break;
                    }
                }
            }
            boardPrint += line + '\n';
        }

        Debug.Log(boardPrint);
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

    private void RebuildMesh() {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        int triangleOffset = 0;

        float halfWidth = _boardView.Theme.TileWidth / 2f;
        float halfHeight = _boardView.Theme.TileHeight / 2f;

        Vector3 up = _boardView.Theme.PointToVector3(Point.Up) * halfHeight + (Vector3.up * Offset);
        Vector3 left = _boardView.Theme.PointToVector3(Point.Left) * halfWidth + (Vector3.up * Offset);
        Vector3 down = _boardView.Theme.PointToVector3(Point.Down) * halfHeight + (Vector3.up * Offset);
        Vector3 right = _boardView.Theme.PointToVector3(Point.Right) * halfWidth + (Vector3.up * Offset);

        foreach (KeyValuePair<Point, Sprite> pointSprite in _points) {
            Vector3 center = _boardView.Theme.PointToVector3(pointSprite.Key);
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
