using UnityEngine;
using System;
using System.Collections.Generic;
using LostGen;

[System.Serializable]
[CreateAssetMenu(fileName = "AutoTile", menuName = "Lost Generation/View/AutoTile", order = 2)]
public class AutoTile : ScriptableObject {
    public const int AUTOTILE_COUNT = 16;
    public enum TileEdge {
        All = 0,          // 0b0000 [ ]
        OneSide = 1,       // 0b1101  _
        OppositeSides = 2, // 0b0101  =
        ThreeSides = 3,    // 0b1000 |_|
        Corner = 4,        // 0b1100  _|
        None = 5,           // 0b1111  X
        Count = 6
    }

    private struct EdgeRotation {
        public TileEdge Edge;
        public Quaternion Rotation;

        public EdgeRotation(TileEdge edge, Quaternion rotation) {
            Edge = edge;
            Rotation = rotation;
        }
    }

    // 0bNWSE
    private static readonly EdgeRotation[] _ROTATIONS = new EdgeRotation[] {
        new EdgeRotation(TileEdge.All          , Quaternion.identity),            // 0b0000
        new EdgeRotation(TileEdge.ThreeSides   , Quaternion.Euler(0f, 90f, 0f)),  // 0b0001
        new EdgeRotation(TileEdge.ThreeSides   , Quaternion.Euler(0f, 180f, 0f)), // 0b0010
        new EdgeRotation(TileEdge.Corner       , Quaternion.Euler(0f, 180f, 0f)), // 0b0011
        new EdgeRotation(TileEdge.ThreeSides   , Quaternion.Euler(0f, -90f, 0f)), // 0b0100
        new EdgeRotation(TileEdge.OppositeSides, Quaternion.identity),            // 0b0101 
        new EdgeRotation(TileEdge.Corner       , Quaternion.Euler(0f, -90f, 0f)), // 0b0110
        new EdgeRotation(TileEdge.OneSide      , Quaternion.Euler(0f, 180f, 0f)), // 0b0111
        new EdgeRotation(TileEdge.ThreeSides   , Quaternion.identity),            // 0b1000
        new EdgeRotation(TileEdge.Corner       , Quaternion.Euler(0f, 90f, 0f)),  // 0b1001
        new EdgeRotation(TileEdge.OppositeSides, Quaternion.Euler(0f, 90f, 0f)),  // 0b1010
        new EdgeRotation(TileEdge.OneSide      , Quaternion.Euler(0f, 90f, 0f)),  // 0b1011
        new EdgeRotation(TileEdge.Corner       , Quaternion.identity),            // 0b1100
        new EdgeRotation(TileEdge.OneSide      , Quaternion.identity),            // 0b1101
        new EdgeRotation(TileEdge.OneSide      , Quaternion.Euler(0f, -90f, 0f)), // 0b1110 east open
        new EdgeRotation(TileEdge.None         , Quaternion.identity)             // 0b1111 no sides open
    };

    [SerializeField]
    private GameObject[] _edges = new GameObject[(int)TileEdge.Count];

    public void SetEdge(TileEdge edge, GameObject prefab) {
        if (edge == TileEdge.Count) {
            throw new ArgumentException("Invalid Tile Edge", "edge");
        }

        _edges[(int)edge] = prefab;
    }

    public GameObject GetEdge(TileEdge edge) {
        if (edge == TileEdge.Count) {
            throw new ArgumentException("Invalid Tile Edge", "edge");
        }

        return _edges[(int)edge];
    }

    public GameObject GetTile(int index, out Quaternion rotation) {
        if (index >= AUTOTILE_COUNT) {
            throw new ArgumentException("Index must be in range [0, " + AUTOTILE_COUNT + "]", "index");
        }

        rotation = _ROTATIONS[index].Rotation;
        return _edges[(int)_ROTATIONS[index].Edge];
    }

    public GameObject GetTile(bool right, bool down, bool left, bool up) {
        int edgeIndex = (right ? 1 : 0) +
                        (down  ? 2 : 0) +
                        (left  ? 4 : 0) + 
                        (up    ? 8 : 0);

        TileEdge edge = _ROTATIONS[edgeIndex].Edge;
        Quaternion rotation = _ROTATIONS[edgeIndex].Rotation;

        GameObject tileObj = GameObject.Instantiate<GameObject>(_edges[(int)edge]);
        tileObj.transform.rotation = rotation;

        return tileObj;
    }
}
