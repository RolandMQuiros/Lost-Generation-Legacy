using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModel : MonoBehaviour {
    public SkinnedMeshRenderer Body {
        get { return _body; }
    }

    public IEnumerable<SkinnedMeshRenderer> Attachments {
        get { return _attachments; }
    }


    [SerializeField]private SkinnedMeshRenderer _body;
    private HashSet<SkinnedMeshRenderer> _attachments = new HashSet<SkinnedMeshRenderer>();

    public void Start() {
        if (_body == null) {
            throw new NullReferenceException("This CharacterModel needs a body mesh!");
        }
    }

    public void Attach(SkinnedMeshRenderer mesh) {
        if (mesh.rootBone != _body.rootBone) { // Don't waste our time
            MapBonesToBody(mesh);
            _attachments.Add(mesh);
        }
    }

    public void Detach(SkinnedMeshRenderer mesh) {
        _attachments.Remove(mesh);
    }

    private void MapBonesToBody(SkinnedMeshRenderer mesh) {
        // Get the name intersection of the mesh's bones and the body's bones, then
        // assign the body's copy to the mesh
        mesh.bones = mesh.bones.Join(
            _body.bones,
            m => m.name,
            b => b.name,
            (m, b) => b
        ).ToArray();

        mesh.rootBone = _body.rootBone;
        mesh.transform.localPosition = Vector3.zero;
        mesh.transform.parent = _body.transform;
    }
}