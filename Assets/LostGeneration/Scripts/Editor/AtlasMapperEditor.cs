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
			_target.TintRed = EditorGUILayout.ColorField("Tint Red", _target.TintRed);
			_target.TintGreen = EditorGUILayout.ColorField("Tint Green", _target.TintGreen);
			_target.TintBlue = EditorGUILayout.ColorField("Tint Blue", _target.TintBlue);

		if (EditorGUI.EndChangeCheck()) {
			_target.ApplyMaterial();
			SceneView.RepaintAll();
		}
	}

}
