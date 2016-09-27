using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectRecycler))]
public class ObjectRecyclerEditor : Editor {
    private ObjectRecycler _recycler;

    private GameObject _newPrefab = null;
    private int _newCapacity = 1;

    private string _errorMessage;

    public void OnEnable() {
        _recycler = (ObjectRecycler)target;
    }

    public override void OnInspectorGUI() {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Prefab", GUILayout.ExpandWidth(true));
        EditorGUILayout.LabelField("Capacity", GUILayout.ExpandWidth(true));
        EditorGUILayout.EndHorizontal();

        int newCapacity;
        GameObject newPrefab;
        bool isDirty = false;

        EditorGUI.BeginDisabledGroup(Application.isPlaying);

        foreach (KeyValuePair<GameObject, int> entry in _recycler.GetRegistry()) {
            newPrefab = entry.Key;
            newCapacity = entry.Value;

            EditorGUILayout.BeginHorizontal();
            newPrefab = (GameObject)EditorGUILayout.ObjectField(entry.Key, typeof(GameObject), false);
            newCapacity = EditorGUILayout.IntField(entry.Value);

            if ((newPrefab != null && newPrefab != entry.Key) || newCapacity != entry.Value) {
                _recycler.RegisterPrefab(newPrefab, (newCapacity > 1 ? newCapacity : 1));
                isDirty = true;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        _newPrefab = (GameObject)EditorGUILayout.ObjectField(_newPrefab, typeof(GameObject), false, GUILayout.ExpandWidth(true));
        _newCapacity = EditorGUILayout.IntField(_newCapacity, GUILayout.ExpandWidth(true));
        if (_newPrefab != null) {
            try {
                _recycler.RegisterPrefab(_newPrefab, _newCapacity >= 1 ? _newCapacity : 0);
                _errorMessage = string.Empty;
                isDirty = true;
            } catch (ArgumentException excep) {
                _errorMessage = excep.Message;
            } finally {
                _newPrefab = null;
                _newCapacity = 1;
            }
        }

        EditorGUILayout.EndHorizontal();

        if (!string.IsNullOrEmpty(_errorMessage)) {
            EditorGUILayout.LabelField(_errorMessage);
        }

        EditorGUILayout.EndVertical();

        if (isDirty) {
            EditorUtility.SetDirty(_recycler);
        }

        EditorGUI.EndDisabledGroup();
    }
}
