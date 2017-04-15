using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AtlasMaterial : MonoBehaviour {
	public Sprite Sprite;
	public Vector2 UVOffset;
	public Vector2 UVSize;
	private Renderer _renderer;
	private MaterialPropertyBlock _matBlock;

	#region MonoBehaviour

	private void Awake() {
		_renderer = GetComponent<Renderer>();
		_matBlock = new MaterialPropertyBlock();
	}

	private void Start() {
		_renderer.GetPropertyBlock(_matBlock);

		Debug.Log("sprite.rect = " + Sprite.rect);
		Debug.Log("sprite.textureRect = " + Sprite.textureRect);
		Debug.Log("sprite.textureRectOffset = " + Sprite.textureRectOffset);
		Vector4 uvOffset = new Vector4(
			Sprite.rect.x / Sprite.texture.width,
			Sprite.rect.y / Sprite.texture.height,
			0f, 1f
		);
		Vector4 uvSize = new Vector4(
			Sprite.rect.size.x / Sprite.texture.width,
			Sprite.rect.size.y / Sprite.texture.height,
			0f, 1f
		);

		Debug.Log("uvOffset = " + uvOffset);
		Debug.Log("uvSize = " + uvSize);

		_matBlock.SetVector("_UVRectOffset", uvOffset);
		_matBlock.SetVector("_UVRectSize", uvSize);
		_renderer.SetPropertyBlock(_matBlock);
	}
/*
	private void Update() {
		_renderer.GetPropertyBlock(_matBlock);
		_matBlock.SetVector("_UVRectOffset", UVOffset);
		_matBlock.SetVector("_UVRectSize", UVSize);
		_renderer.SetPropertyBlock(_matBlock);
	}
*/
	#endregion MonoBehaviour
}
