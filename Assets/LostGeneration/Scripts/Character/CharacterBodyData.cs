// using System;
// using System.Linq;
// using System.Collections.Generic;
// using UnityEngine;

// [Serializable]
// public class CharacterBodyData : MonoBehaviour {
//     [Serializable]
//     private struct ControlBone {
//         public Transform Bone;
//         public Vector3 InitialPosition;
//         public Vector3 InitialEulers;
//         public Vector3 InitialScale;
//         public ControlBone(Transform bone) {
//             Bone = bone;
//             InitialPosition = bone.localPosition;
//             InitialEulers = bone.localEulerAngles;
//             InitialScale = bone.localScale;
//         }
//     } 

//     public Transform Skeleton {
//         get { return _skeleton; }
//         set {
//             if (_skeleton != value) {
//                 _skeleton = value;
//                 _controlBones = new List<ControlBone>(
//                     _skeleton.GetComponentsInChildren<Transform>()
//                         .Where(b => b.name.StartsWith("C_"))
//                         .Select(b => new ControlBone(b))
//                 );
//             }
//         }
//     }

//     public IDictionary<string, float> BlendShapes {
//         get { return GetBlendShapes().ToDictionary(p => p.Key, p => p.Value); }
//     }

//     public IEnumerable<SkinnedMeshRenderer> Attachments {
//         get { return _attachments; }
//     }

//     [SerializeField]private Transform _skeleton;
//     [SerializeField]private List<ControlBone> _controlBones;
//     [SerializeField]private List<SkinnedMeshRenderer> _attachments;
//     [SerializeField]private List<string> _blendShapeNames;
//     [SerializeField]private List<float> _blendShapeWeights;

//     public IEnumerable<KeyValuePair<string, float>> GetBlendShapes() {
//         for (int b = 0; b < _blendShapeNames.Count; b++) {
//             string name = _blendShapeNames[b];
//             float weight = _blendShapeWeights[b];
//             yield return new KeyValuePair<string, float>(name, weight);
//         }
//     }

//     public void Attach(SkinnedMeshRenderer mesh) {

//     }
// }