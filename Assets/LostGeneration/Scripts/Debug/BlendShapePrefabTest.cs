using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendShapePrefabTest : MonoBehaviour {
	public GameObject Mesh;
	// Use this for initialization
	void Start () {
		for (int i = 0; i < 1000; i++) {
			GameObject newObj = GameObject.Instantiate(Mesh, Random.Range(0f, 100f) * Random.insideUnitSphere, Quaternion.identity);

			CharacterBody body = newObj.GetComponent<CharacterBody>();
			foreach (string key in body.BlendShapes.Keys.ToList()) {
				body.SetBlendShapeWeight(key, Random.Range(0f, 100f));
			}

			Color r1 = Random.ColorHSV();
			Color r2 = Random.ColorHSV();
			Color g1 = Random.ColorHSV();
			Color g2 = Random.ColorHSV();
			Color b1 = Random.ColorHSV();
			Color b2 = Random.ColorHSV();

			foreach (AtlasMaterial atlas in newObj.GetComponentsInChildren<AtlasMaterial>()) {
				atlas.ColorR1 = r1;
				atlas.ColorR2 = r2;
				atlas.ColorG1 = g1;
				atlas.ColorG2 = g2;
				atlas.ColorB1 = b1;
				atlas.ColorB2 = b2;
				atlas.ApplyMaterial();
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
