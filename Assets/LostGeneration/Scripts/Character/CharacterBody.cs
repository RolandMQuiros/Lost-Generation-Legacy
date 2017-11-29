using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBody : MonoBehaviour, ISerializationCallbackReceiver {
    public Transform Skeleton {
        get { return _skeleton; }
        set { _skeleton = value; }
    }

    public IEnumerable<SkinnedMeshRenderer> Attachments {
        get { return _attachments; }
    }

    public IDictionary<string, float> BlendShapes {
        get { return _blendShapes; }
    }

    [SerializeField]private Transform _skeleton;
    [SerializeField]private List<SkinnedMeshRenderer> _attachments = new List<SkinnedMeshRenderer>();
    [SerializeField]private List<string> _blendShapeNames = new List<string>();
    [SerializeField]private List<float> _blendShapeWeights = new List<float>();
    private Dictionary<string, float> _blendShapes = new Dictionary<string, float>();
    public void Attach(SkinnedMeshRenderer mesh, bool destroyOldParent = true) {
        GameObject oldParent = mesh.transform.parent.gameObject;

        if (mesh.rootBone != _skeleton.GetChild(0)) {
            MapBonesToSkeleton(mesh);
        }
        AddBlendShapes(mesh);
        _attachments.Add(mesh);

        if (destroyOldParent && oldParent != null) {
            GameObject.DestroyImmediate(oldParent);
        }
    }

    public void Detach(SkinnedMeshRenderer mesh) {
        _attachments.Remove(mesh);

        // Check if any of the remaining attachments share a blendshape with the removed mesh.
        // If not, remove the blendshape from the dictionary.
        for (int s = 0; s < mesh.sharedMesh.blendShapeCount; s++) {
            string blendShapeName = mesh.sharedMesh.GetBlendShapeName(s);
            if (!_attachments.Any(a => a.sharedMesh.GetBlendShapeIndex(blendShapeName) != -1)) {
                _blendShapes.Remove(blendShapeName);
            }
        }
    }

    public bool IsAttached(SkinnedMeshRenderer mesh) {
        return _attachments.Contains(mesh);
    }

    public void SetBlendShapeWeight(string name, float weight) {
        if (_blendShapes.ContainsKey(name)) {
            _blendShapes[name] = weight;

            foreach (SkinnedMeshRenderer mesh in _attachments) {
                int blendShapeIndex = mesh.sharedMesh.GetBlendShapeIndex(name);
                if (blendShapeIndex != -1) {
                    mesh.SetBlendShapeWeight(blendShapeIndex, weight);
                }
            }
        }
    }

    private void MapBonesToSkeleton(SkinnedMeshRenderer mesh) {
        Dictionary<string, Transform> skeletonBones = GetComponentsInChildren<Transform>().ToDictionary(b => b.name, b => b);
        Transform[] newBones = new Transform[mesh.bones.Length];
        for (int m = 0; m < newBones.Length; m++) {
            Transform skeletonBone;
            if (skeletonBones.TryGetValue(mesh.bones[m].name, out skeletonBone)) {
                newBones[m] = skeletonBone;
            }
        }
        mesh.bones = newBones;
        mesh.rootBone = _skeleton.GetChild(0);
        mesh.transform.localPosition = Vector3.zero;
        mesh.transform.parent = _skeleton.parent;
    }

    private void AddBlendShapes(SkinnedMeshRenderer mesh) {
        for (int s = 0; s < mesh.sharedMesh.blendShapeCount; s++) {
            string blendShapeName = mesh.sharedMesh.GetBlendShapeName(s);
            
            float weight;
            if (_blendShapes.TryGetValue(blendShapeName, out weight)) {
                mesh.SetBlendShapeWeight(s, weight);
            } else {
                _blendShapes.Add(blendShapeName, 0f);
            }
        }
    }

    #region ISerializationCallbackReceiver
    public void OnBeforeSerialize() {
        _blendShapeNames.Clear();
        _blendShapeWeights.Clear();

        foreach (KeyValuePair<string, float> pair in _blendShapes) {
            _blendShapeNames.Add(pair.Key);
            _blendShapeWeights.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize() {
        _blendShapes = new Dictionary<string, float>();
        for (int s = 0; s < _blendShapeNames.Count; s++) {
            _blendShapes.Add(_blendShapeNames[s], _blendShapeWeights[s]);
        }
    }
    
    #endregion

    #region MonoBehaviour
    private void Awake() {
        if (_skeleton == null) {
            throw new NullReferenceException("This CharacterModel needs a skeleton root bone!");
        }
    }
    #endregion
}