using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName = "AutoTile", menuName = "Lost Generation/View/AutoTile", order = 2)]
public class AutoTile : ScriptableObject {
    public const int AUTOTILE_COUNT = 16;
    private List<GameObject[]> _variations = new List<GameObject[]>();
    public int VariationCount { get { return _variations.Count; } }

    public void AddTileVariation(GameObject[] objects) {
        if (objects.Length != AUTOTILE_COUNT) {
            throw new ArgumentException("Array of GameObjects must contain exactly 16 elements", "objects");
        }

        GameObject[] newVariation = new GameObject[AUTOTILE_COUNT];
        _variations.Add(newVariation);
        for (int i = 0; i < objects.Length; i++) {
            newVariation[i] = objects[i];
        }
    }

    public void DeleteTileVariation(int variation) {
        _variations.RemoveAt(variation);
    }

    public GameObject GetTile(int variation, int autoTileCode) {
        if (autoTileCode < 0 || autoTileCode >= AUTOTILE_COUNT) {
            throw new ArgumentOutOfRangeException("The AutoTile code must be between 0 and 15", "autoTileCode");
        }

        return _variations[variation][autoTileCode];
    }

    public void SetTile(int variation, int autoTileCode, GameObject tile) {
        if (autoTileCode < 0 || autoTileCode >= AUTOTILE_COUNT) {
            throw new ArgumentOutOfRangeException("The AutoTile code must be between 0 and 15", "autoTileCode");
        }

        _variations[variation][autoTileCode] = tile;
    }
}
