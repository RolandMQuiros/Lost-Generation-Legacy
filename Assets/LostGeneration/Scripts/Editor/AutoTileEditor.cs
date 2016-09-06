using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(AutoTile))]
public class AutoTileEditor : Editor {
    private Texture[] _icons = new Texture[(int)AutoTile.TileEdge.Count];
    private AutoTile _autoTile;

    public void OnEnable() {
        _autoTile = (AutoTile)target;

        _icons[(int)AutoTile.TileEdge.All]          = Resources.Load("Icons/AutoTile/autotile_0000") as Texture;
        _icons[(int)AutoTile.TileEdge.ThreeSides]    = Resources.Load("Icons/AutoTile/autotile_1000") as Texture;
        _icons[(int)AutoTile.TileEdge.OppositeSides] = Resources.Load("Icons/AutoTile/autotile_0101") as Texture;
        _icons[(int)AutoTile.TileEdge.Corner]        = Resources.Load("Icons/AutoTile/autotile_1100") as Texture;
        _icons[(int)AutoTile.TileEdge.OneSide]       = Resources.Load("Icons/AutoTile/autotile_1101") as Texture;
        _icons[(int)AutoTile.TileEdge.None]           = Resources.Load("Icons/AutoTile/autotile_1111") as Texture;
    }

    public override void OnInspectorGUI() {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField("Tiles");

        EditorGUIUtility.labelWidth = 32;
        for (int i = 0; i < (int)AutoTile.TileEdge.Count; i++) {
            GameObject edge = _autoTile.GetEdge((AutoTile.TileEdge)i);
            GameObject old = edge;

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(new GUIContent(_icons[i]), GUILayout.Width(32f), GUILayout.Height(32f));
            edge = (GameObject)EditorGUILayout.ObjectField(edge, typeof(GameObject), false);

            EditorGUILayout.EndHorizontal();

            if (edge != old) {
                _autoTile.SetEdge((AutoTile.TileEdge)i, edge);
                EditorUtility.SetDirty(_autoTile);
            }
        }
        EditorGUIUtility.labelWidth = 0;

        EditorGUILayout.EndVertical();
    }
}
