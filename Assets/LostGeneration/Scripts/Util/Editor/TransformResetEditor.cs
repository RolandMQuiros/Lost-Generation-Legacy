using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TransformReset))]
public class TransformResetEditor : Editor {
    public override void OnInspectorGUI() {
        TransformReset controlBone = (TransformReset)target;

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Initial Values");
        if (GUILayout.Button("Copy Transform")) {
            controlBone.Set();
        }
        EditorGUILayout.EndHorizontal();

        controlBone.InitialPosition = EditorGUILayout.Vector3Field("Position", controlBone.InitialPosition);
        controlBone.InitialRotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation", controlBone.InitialRotation.eulerAngles));
        controlBone.InitialScale = EditorGUILayout.Vector3Field("Scale", controlBone.InitialScale);

        if (GUILayout.Button("Reset")) {
            controlBone.Reset();
        }

        EditorGUI.EndChangeCheck();
    }
}