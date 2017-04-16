using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneMapper : MonoBehaviour {
	public SkinnedMeshRenderer Source;
	public SkinnedMeshRenderer[] Targets;

	// Use this for initialization
	private void Start () {
		if (Source) {
			for (int i = 0; i < Targets.Length; i++) {
				Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();
				foreach (Transform bone in Source.bones) {
					boneMap[bone.name] = bone;
				}

				SkinnedMeshRenderer target = Targets[i];
				Transform[] newBones = new Transform[target.bones.Length];
				for (int b = 0; b < target.bones.Length; b++) {
					GameObject boneObj = target.bones[b].gameObject;
					Transform newBone;
					if (boneMap.TryGetValue(boneObj.name, out newBone)) {
						newBones[b] = newBone;
					}
				}
				
				target.bones = newBones;
			}
		}
	}
}
