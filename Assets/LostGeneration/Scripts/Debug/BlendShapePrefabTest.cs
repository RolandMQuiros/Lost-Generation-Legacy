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

			Color r = Random.ColorHSV();
			Color g = Random.ColorHSV();
			Color b = Random.ColorHSV();

			foreach (AtlasMaterial atlas in newObj.GetComponentsInChildren<AtlasMaterial>()) {
				atlas.TintRed = r;
				atlas.TintGreen = g;
				atlas.TintBlue = b;
				atlas.ApplyMaterial();
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
