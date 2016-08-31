using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(AutoTile))]
public class AutoTileEditor : Editor {
    private const int _ROW_WIDTH = 4;
    private AutoTile _autoTile;
    private bool _showVariations = false;
    private List<bool> _foldouts;
    private Vector2 _tileScroll = new Vector2();

    public void OnEnable() {
        _autoTile = (AutoTile)target;
        if (_autoTile.VariationCount == 0) {
            _autoTile.AddTileVariation(new GameObject[AutoTile.AUTOTILE_COUNT]);
        }

        _foldouts = new List<bool>();
        for (int i = 0; i < _autoTile.VariationCount; i++) {
            _foldouts.Add(false);
        }
    }

    public override void OnInspectorGUI() {
        EditorGUILayout.BeginHorizontal();
        _showVariations = EditorGUILayout.Foldout(_showVariations, "Tile Variations");
        bool addVariation = GUILayout.Button("+", GUILayout.ExpandWidth(false));
        EditorGUILayout.EndHorizontal();

        if (_showVariations) {
            _tileScroll = EditorGUILayout.BeginScrollView(_tileScroll, GUILayout.ExpandHeight(false), GUILayout.Height(200f));
            EditorGUI.indentLevel++;
            for (int i = 0; i < _autoTile.VariationCount; i++) { 
                EditorGUILayout.BeginHorizontal();
                _foldouts[i] = EditorGUILayout.Foldout(_foldouts[i], "Variation #" + (i + 1));
                bool deleteVariation = false;
                if (i > 0) {
                    deleteVariation = GUILayout.Button("-", GUILayout.ExpandWidth(false));
                }
                EditorGUILayout.EndHorizontal();

                if (deleteVariation) {
                    _autoTile.DeleteTileVariation(i);
                } else if (_foldouts[i]) {
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginHorizontal();
                    for (int j = 0; j < AutoTile.AUTOTILE_COUNT; j++) {
                        if (i > 0 && i % _ROW_WIDTH == 0) {
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                        }

                        GameObject tile = null;
                        if (i < _autoTile.VariationCount) {
                            tile = _autoTile.GetTile(i, j);
                        }

                        GameObject newTile = EditorGUILayout.ObjectField(
                            tile,
                            typeof(GameObject),
                            true
                        ) as GameObject;
                        if (newTile != tile) {
                            _autoTile.SetTile(i, j, newTile);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndScrollView();
        }

        if (addVariation) {
            AddVariation();
        }
    }

    private void AddVariation() {
        GameObject[] newVariation = new GameObject[AutoTile.AUTOTILE_COUNT];
        int lastVariation = _autoTile.VariationCount - 1;
        for (int i = 0; i < AutoTile.AUTOTILE_COUNT; i++) {
            newVariation[i] = _autoTile.GetTile(lastVariation, i);
        }

        _autoTile.AddTileVariation(new GameObject[AutoTile.AUTOTILE_COUNT]);
    }
}
