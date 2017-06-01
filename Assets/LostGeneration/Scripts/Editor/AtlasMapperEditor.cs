using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AtlasMaterial))]
public class AtlasMapperEditor : Editor {
	private AtlasMaterial _target;

	public void OnEnable() {
		_target = (AtlasMaterial)target;
	}

	public override void OnInspectorGUI() {

		EditorGUI.BeginChangeCheck();
			_target.Sprite = EditorGUILayout.ObjectField("Sprite", _target.Sprite, typeof(Sprite), false) as Sprite;
			_target.ColorR1 = EditorGUILayout.ColorField("Color R1", _target.ColorR1);
			_target.ColorR2 = EditorGUILayout.ColorField("Color R2", _target.ColorR2);
			_target.ColorG1 = EditorGUILayout.ColorField("Color G1", _target.ColorG1);
			_target.ColorG2 = EditorGUILayout.ColorField("Color G2", _target.ColorG2);
			_target.ColorB1 = EditorGUILayout.ColorField("Color B1", _target.ColorB1);
			_target.ColorB2 = EditorGUILayout.ColorField("Color B2", _target.ColorB2);

		if (EditorGUI.EndChangeCheck()) {
			_target.ApplyMaterial();
			SceneView.RepaintAll();
		}
	}

}
