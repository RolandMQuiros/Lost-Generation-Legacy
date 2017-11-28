using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterBody))]
public class CharacterBodyEditor : Editor {
    private CharacterBody _target;
    private SkinnedMeshRenderer _newAttachment = null;
    private Vector2 _attachmentScroll = new Vector2();
    private List<SkinnedMeshRenderer> _bodyMeshes = new List<SkinnedMeshRenderer>();

    private void OnEnable() {
        _target = (CharacterBody)target;
    }

    public override void OnInspectorGUI() {
        _target.Skeleton = (Transform)EditorGUILayout.ObjectField("Skeleton Root", _target.Skeleton, typeof(Transform), true);
        if (_target.Skeleton) {
            ShowAttachments();
            ShowBlendWeights();
        }
    }

    private void ShowAttachments() {
        bool scrollToBottom = false;

        _attachmentScroll = GUILayout.BeginScrollView(_attachmentScroll, GUILayout.MaxHeight(100f));
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
        GUILayout.EndScrollView();
        toDetach.ForEach(a => _target.Detach(a) );
        
        EditorGUILayout.BeginHorizontal();
        _newAttachment = (SkinnedMeshRenderer)EditorGUILayout.ObjectField(_newAttachment, typeof(SkinnedMeshRenderer), true);
        if (_newAttachment != null && GUILayout.Button("+", GUILayout.MaxWidth(16f))) {
            _target.Attach(_newAttachment, false);
            _newAttachment = null;
            scrollToBottom = true;
        }
        EditorGUILayout.EndHorizontal();

        if (scrollToBottom) { _attachmentScroll.y = 100f; }
    }

    private void ShowBlendWeights() {
        Dictionary<string, float> oldWeights = new Dictionary<string, float>(_target.BlendShapes);

        // Decompose the blend shape names into a tree of group names
        // This is only for ease of display, so we can make dropdown groups based on the tokens
        Dictionary<string, List<string>> grouping = new Dictionary<string, List<string>>();
        foreach (string shapeName in _target.BlendShapes.Keys) {
            string[] groups = shapeName.Split(":");
            for (int g = 1; g < groups.Length; g++) {
                string parent = groups[g - 1];
                string child = groups[g];

                List<string> children;
                if (!grouping.TryGetValue(parent, out children)) {
                    children = new List<string>();
                    grouping[parent] = children;
                }
                children.Add(child);
            }
        }

        string fullName = "";
        int level = 0;
        string group;

        while (grouping.Count > 0) {
            Stack<string> open = new Stack<string>();
            group = grouping.First().Key;
            open.Push(group);

            while (open.Count > 0) {
                group = open.Pop();

                // If leaf node, create the weight field
                if (grouping[group].Count <= 0) {
                    float oldWeight = oldWeights[fullName];
                    float newWeight = EditorGUILayout.FloatField(group, oldWeight);
                    newWeight = Mathf.Clamp(newWeight, 0f, 100f);
                    if (newWeight != oldWeight) {
                        _target.SetBlendShapeWeight(fullName, newWeight);
                    }
                // If not, create a folding layer
                } else {
                    fullName += ':';
                    level++;

                    group.Value.ForEach(g => open.Push(grouping[g]));
                }
            }
        }

        foreach (KeyValuePair<string, float> blendShape in oldWeights) {
            
            


            float newWeight = EditorGUILayout.FloatField(blendShape.Key, blendShape.Value);
            newWeight = Mathf.Clamp(newWeight, 0f, 100f);
            if (newWeight != blendShape.Value) {
                _target.SetBlendShapeWeight(blendShape.Key, newWeight);
            }
        }
    }
}