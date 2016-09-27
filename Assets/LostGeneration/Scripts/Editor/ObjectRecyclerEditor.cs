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
        ObjectRegistryGUI();
    }

    private void ObjectRegistryGUI() {
        bool isDirty = false;

        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Prefab", GUILayout.ExpandWidth(true));
        EditorGUILayout.LabelField("Capacity", GUILayout.ExpandWidth(true));
        EditorGUILayout.EndHorizontal();

        EditorGUI.BeginDisabledGroup(Application.isPlaying);

        // Display the prefabs registered in the recycler
        foreach (KeyValuePair<GameObject, int> entry in _recycler.GetRegistry()) {
            GameObject newPrefab = entry.Key;
            int newCapacity = entry.Value;
            bool delete = false;

            EditorGUILayout.BeginHorizontal();

            newPrefab = (GameObject)EditorGUILayout.ObjectField(entry.Key, typeof(GameObject), false);
            newCapacity = EditorGUILayout.IntField(entry.Value, GUILayout.ExpandWidth(false));
            delete = GUILayout.Button("-", GUILayout.Width(32f), GUILayout.ExpandHeight(false));

            if (delete) {
                // Delete the registry pair
                _recycler.UnregisterPrefab(entry.Key);
                isDirty = true;
            } else if ((newPrefab != null && newPrefab != entry.Key) || newCapacity != entry.Value) {
                // If anything about the registry field pair is different, update the registry
                _recycler.RegisterPrefab(newPrefab, (newCapacity > 1 ? newCapacity : 1));
                isDirty = true;
            }

            EditorGUILayout.EndHorizontal();
        }

        // Display an open Prefab/Capacity input pair to add new prefabs to registry
        EditorGUILayout.BeginHorizontal();
        _newPrefab = (GameObject)EditorGUILayout.ObjectField(_newPrefab, typeof(GameObject), false, GUILayout.ExpandWidth(true));
        _newCapacity = EditorGUILayout.IntField(_newCapacity, GUILayout.ExpandWidth(false));
        GUILayout.Space(36f);
        if (_newPrefab != null) {
            // If the prefab is already registered, preempt the field assignment
            if (_recycler.IsRegistered(_newPrefab)) {
                _newPrefab = null;
            } else {
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
