using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CharacterBody : SerializedMonoBehaviour {
    public Transform Skeleton {
        get { return _skeleton; }
        set { _skeleton = value; }
    }

    public IEnumerable<TransformReset> ControlBones {
        get { return GetComponentsInChildren<TransformReset>(); }
    }

    public IEnumerable<SkinnedMeshRenderer> Attachments {
        get { return _attachments.Where(a => a != null); }
    }

    public IDictionary<string, float> BlendShapes {
        get { return _blendShapes; }
    }

    [SerializeField]private HashSet<SkinnedMeshRenderer> _attachments = new HashSet<SkinnedMeshRenderer>();
    [SerializeField]private Dictionary<string, float> _blendShapes = new Dictionary<string, float>();
    [SerializeField]private Transform _skeleton;
    [SerializeField]private List<Transform> _controlBones = new List<Transform>();

    public void Attach(SkinnedMeshRenderer mesh, bool destroyOldParent = true) {
        if (mesh == null) {
            throw new NullReferenceException();
        }
        if (!_attachments.Contains(mesh)) {
            GameObject oldParent = mesh.transform.parent.gameObject;

            if (mesh.rootBone != _skeleton.GetChild(0)) {
                MapBonesToSkeleton(mesh);
            }
            AddBlendShapes(mesh);
            _attachments.Add(mesh);

            if (destroyOldParent && oldParent != null && !oldParent.transform.IsChildOf(transform)) {
                GameObject.DestroyImmediate(oldParent);
            }
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

    public void Clear() {
        _attachments.Clear();
        _blendShapes.Clear();
    }

    public bool IsAttached(SkinnedMeshRenderer mesh) {
        return _attachments.Contains(mesh);
    }

    public bool SetBlendShapeWeight(string name, float weight) {
        bool meshFound = false;
        if (_blendShapes.ContainsKey(name)) {
            _blendShapes[name] = weight;

            foreach (SkinnedMeshRenderer mesh in _attachments) {
                int blendShapeIndex = mesh.sharedMesh.GetBlendShapeIndex(name);
                if (blendShapeIndex != -1) {
                    meshFound = true;
                    mesh.SetBlendShapeWeight(blendShapeIndex, weight);
                }
            }
        }
        return meshFound;
    }

    public void ClearWeights() {
        foreach (string key in _blendShapes.Keys.ToList()) {
            if (!SetBlendShapeWeight(key, 0f)) {
                _blendShapes.Remove(key);
            }
        }
    }
    
    private void MapBonesToSkeleton(SkinnedMeshRenderer mesh) {
        Dictionary<string, Transform> skeletonBones = _skeleton.GetComponentsInChildren<Transform>().ToDictionary(b => b.name, b => b);
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

    #region MonoBehaviour
    private void Awake() {
        if (_skeleton == null) {
            throw new NullReferenceException("This CharacterModel needs a skeleton root bone!");
        }
    }
    #endregion
}