using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterBody))]
public class CharacterBodyEditor : Editor {
    private CharacterBody _target;
    private SkinnedMeshRenderer _newAttachment = null;
    private Vector2 _attachmentScroll = new Vector2();
    private NameTree _blendShapeNames = new NameTree(':');
    private HashSet<string> _unfolded = new HashSet<string>();

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
        bool updateBlendShapeNames = false;

        _attachmentScroll = GUILayout.BeginScrollView(_attachmentScroll, GUILayout.MaxHeight(100f));
        List<SkinnedMeshRenderer> toDetach = new List<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer attachment in _target.Attachments) {
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.LabelField(attachment.name);
            if (GUILayout.Button("-", GUILayout.MaxWidth(16f))) {
                toDetach.Add(attachment);
                scrollToBottom = true;
                updateBlendShapeNames = true;
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
            updateBlendShapeNames = true;
        }
        EditorGUILayout.EndHorizontal();

        if (updateBlendShapeNames) {
            _blendShapeNames = new NameTree(':', _target.BlendShapes.Keys);
        }

        if (scrollToBottom) { _attachmentScroll.y = 100f; }
    }

    private class BlendShapeGroup {
        public string Path;
        public List<BlendShapeGroup> Children = new List<BlendShapeGroup>();
    }

    private void ShowBlendWeights() {
        bool unfolded = true;
        foreach (string path in _blendShapeNames) {
            string label = _blendShapeNames.GetName(path);
            if (_blendShapeNames.IsLeaf(label)) {
                if (unfolded) {
                    float oldWeight = _target.BlendShapes[path];
                    float newWeight = Mathf.Clamp(EditorGUILayout.FloatField(label, oldWeight), 0f, 100f);
                    if (oldWeight != newWeight) {
                        _target.SetBlendShapeWeight(path, newWeight);
                    }
                }
            } else {
                if (unfolded = EditorGUILayout.Foldout(_unfolded.Contains(path), label)) {
                    _unfolded.Add(path);
                } else {
                    _unfolded.Remove(path);
                }
            }
        }
    }

    private void ShowBlendWeightsOld() {
        Dictionary<string, HashSet<string>> groups = new Dictionary<string, HashSet<string>>();
        
        HashSet<string> rootChildren = new HashSet<string>();
        groups.Add("_", rootChildren);

        foreach (string key in _target.BlendShapes.Keys) {
            string[] tokens = key.Split(new char[] { ':' });
            string path = tokens[0];
            rootChildren.Add(path);

            for (int t = 1; t < tokens.Length; t++) {
                HashSet<string> children;
                if (!groups.TryGetValue(path, out children)) {
                    children = new HashSet<string>();
                    groups[path] = children;
                }
                path += ':' + tokens[t];
                children.Add(path);
            }
        }

        Stack<string> open = new Stack<string>();
        open.Push("_");

        while (open.Count > 0) {
            string path = open.Pop();
            
            int lastColon = path.LastIndexOf(':');
            string label = path;
            if (lastColon != -1) {
                label = path.Substring(lastColon + 1);
            }

            HashSet<string> children;
            if (!groups.TryGetValue(path, out children)) { // Leaf
                float oldWeight = _target.BlendShapes[path];
                float newWeight = Mathf.Clamp(EditorGUILayout.FloatField(label, oldWeight), 0f, 100f);
                if (oldWeight != newWeight) {
                    _target.SetBlendShapeWeight(path, newWeight);
                }
            } else { // Non-leaf
                bool unfolded = true;
                if (path != "_") { // Ignore the root foldout
                    unfolded = EditorGUILayout.Foldout(_unfolded.Contains(path), label);
                }
                
                if (unfolded) {
                    _unfolded.Add(path);
                    foreach (string child in children) {
                        open.Push(child);
                    }
                } else {
                    _unfolded.Remove(path);
                }
            }
        }
    }
}