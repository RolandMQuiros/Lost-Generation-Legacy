using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterBody))]
public class CharacterBodyEditor : Editor {
    private CharacterBody _target;
    private SkinnedMeshRenderer _newAttachment = null;
    private Vector2 _attachmentScroll = new Vector2();
    private HashSet<string> _unfoldedBlendShapes = new HashSet<string>();
    private HashSet<Transform> _unfoldedControlBones = new HashSet<Transform>();

    private void OnEnable() {
        _target = (CharacterBody)target;
    }

    public override void OnInspectorGUI() {
        Transform skeleton = (Transform)EditorGUILayout.ObjectField("Skeleton Root", _target.Skeleton, typeof(Transform), true);
        if (skeleton) {
            if (skeleton != _target.Skeleton) {
                _target.Skeleton = skeleton;
            }
            ShowControlBones();
            ShowAttachments();
            ShowBlendWeights();
        }
    }

    private void ShowAttachments() {
        bool scrollToBottom = false;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Attachments");
        if (GUILayout.Button("Clear")) {
            _target.Clear();
        }
        GUILayout.EndHorizontal();

        EditorGUI.indentLevel++;
        List<SkinnedMeshRenderer> toDetach = new List<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer attachment in _target.Attachments) {
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.LabelField(attachment.name);
            if (GUILayout.Button("-", GUILayout.MaxWidth(16f))) {
                toDetach.Add(attachment);
                scrollToBottom = true;
            }

            EditorGUILayout.EndHorizontal();
        }
        EditorGUI.indentLevel--;
        toDetach.ForEach(a => _target.Detach(a) );
        
        EditorGUILayout.BeginHorizontal();
        _newAttachment = (SkinnedMeshRenderer)EditorGUILayout.ObjectField(_newAttachment, typeof(SkinnedMeshRenderer), true);
        if (GUILayout.Button("+", GUILayout.MaxWidth(16f)) && _newAttachment != null) {
            _target.Attach(_newAttachment, false);
            _newAttachment = null;
            scrollToBottom = true;
        }
        EditorGUILayout.EndHorizontal();

        if (scrollToBottom) { _attachmentScroll.y = 100f; }
    }

    private void ShowControlBones() {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Control Bones");
        if (GUILayout.Button("Reset")) {
            _target.ResetControlBones();
        }
        GUILayout.EndHorizontal();

        EditorGUI.indentLevel++;
        foreach (Transform control in _target.ControlBones) {
            bool foldout = EditorGUILayout.Foldout(_unfoldedControlBones.Contains(control), control.name);
            if (foldout) {
                _unfoldedControlBones.Add(control);

                EditorGUI.indentLevel++;
                Vector3 position = EditorGUILayout.Vector3Field("Position", control.localPosition);
                Vector3 eulers = EditorGUILayout.Vector3Field("Rotation", control.localEulerAngles);
                Vector3 scale = EditorGUILayout.Vector3Field("Scale", control.localScale);
                EditorGUI.indentLevel--;

                if (position != control.localPosition) { control.localPosition = position; }
                if (eulers != control.localEulerAngles) { control.localEulerAngles = eulers; }
                if (scale != control.localScale) { control.localScale = scale; }
            } else {
                _unfoldedControlBones.Remove(control);
            }
        }
        EditorGUI.indentLevel--;
    }

    private void ShowBlendWeights() {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Blend Shapes");
        if (GUILayout.Button("Clear Weights")) {
            _target.ClearWeights();
        }
        EditorGUILayout.EndHorizontal();

        bool unfolded = true;
        NameTree blendShapeNames = new NameTree('.', _target.BlendShapes.Keys);
        foreach (string path in blendShapeNames) {
            string parent = blendShapeNames.GetParent(path);
            string label = blendShapeNames.GetName(path);
            EditorGUI.indentLevel = blendShapeNames.GetLevel(path);
            if (parent.Length == 0 || _unfoldedBlendShapes.Contains(parent)) {
                if (blendShapeNames.IsLeaf(path)) {
                    float oldWeight = _target.BlendShapes[path];
                    float newWeight = Mathf.Clamp(EditorGUILayout.FloatField(label, oldWeight), 0f, 100f);
                    if (oldWeight != newWeight) {
                        _target.SetBlendShapeWeight(path, newWeight);
                    }
                } else {
                    if (unfolded = EditorGUILayout.Foldout(_unfoldedBlendShapes.Contains(path), label)) {
                        _unfoldedBlendShapes.Add(path);
                    } else {
                        _unfoldedBlendShapes.Remove(path);
                    }
                }
            }
        }
    }
}