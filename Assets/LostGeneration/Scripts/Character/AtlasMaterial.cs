using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AtlasMaterial : MonoBehaviour {
	public Sprite Sprite;
	public Color TintRed = new Color(1f, 0f, 0f);
	public Color TintGreen = new Color(0f, 1f, 0f);
	public Color TintBlue = new Color(0f, 0f, 1f);

	private Renderer _renderer;
	private MaterialPropertyBlock _matBlock;

	public void ApplyMaterial() {
		_renderer.GetPropertyBlock(_matBlock);

		if (Sprite != null) {
			Vector4 uvOffset = new Vector4(
				Sprite.rect.x / Sprite.texture.width,
				Sprite.rect.y / Sprite.texture.height,
				Sprite.rect.size.x / Sprite.texture.width,
				Sprite.rect.size.y / Sprite.texture.height
			);
			_matBlock.SetVector("_UVOffset", uvOffset);
		}

		_matBlock.SetColor("_TintRed", TintRed);
		_matBlock.SetColor("_TintGreen", TintGreen);
		_matBlock.SetColor("_TintBlue", TintBlue);
		_renderer.SetPropertyBlock(_matBlock);
	}

	#region MonoBehaviour
	private void OnEnable() {
		_renderer = GetComponent<Renderer>();
		_matBlock = new MaterialPropertyBlock();
	}

	private void OnValidate() {
		_renderer = GetComponent<Renderer>();
		_matBlock = new MaterialPropertyBlock();
		ApplyMaterial();
	}

	private void Start() {
		ApplyMaterial();
	}
	#endregion MonoBehaviour
}
