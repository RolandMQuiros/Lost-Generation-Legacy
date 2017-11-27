using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterBody))]
public class CharacterBodyEditor : Editor {
    private CharacterBody _target;
    private List<SkinnedMeshRenderer> _bodyMeshes = new List<SkinnedMeshRenderer>();

    private void OnEnable() {
        _target = (CharacterBody)target;
    }

    public override void OnInspectorGUI() {
        _target.Skeleton = (Transform)EditorGUILayout.ObjectField("Skeleton Root", _target.Skeleton, typeof(Transform), true);

        SerializedProperty bodyMeshes = serializedObject.FindProperty("_bodyMeshes");

        Dictionary<string, float> oldWeights = new Dictionary<string, float>(_target.BlendShapes);
        foreach (KeyValuePair<string, float> blendShape in oldWeights) {
            float newWeight = EditorGUILayout.FloatField(blendShape.Key, blendShape.Value);
            newWeight = Mathf.Clamp(newWeight, 0f, 100f);
            if (newWeight != blendShape.Value) {
                _target.SetBlendShapeWeight(blendShape.Key, newWeight);
            }
        }
    }
}