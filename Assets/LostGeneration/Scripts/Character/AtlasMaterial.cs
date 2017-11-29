using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AtlasMaterial : MonoBehaviour {
	public Sprite Sprite;
	public Color DarkColor;
	public Color ColorR1 = new Color(0.5f, 0f, 0f);
	public Color ColorR2 = new Color(1f, 0f, 0f);
	public Color ColorG1 = new Color(0f, 0.5f, 0f);
	public Color ColorG2 = new Color(0f, 1f, 0f);
	public Color ColorB1 = new Color(0f, 0f, 0.5f);
	public Color ColorB2 = new Color(0f, 0f, 1f);
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

		_matBlock.SetColor("_ColorR1", ColorR1);
		_matBlock.SetColor("_ColorR2", ColorR2);
		_matBlock.SetColor("_ColorG1", ColorG1);
		_matBlock.SetColor("_ColorG2", ColorG2);
		_matBlock.SetColor("_ColorB1", ColorB1);
		_matBlock.SetColor("_ColorB2", ColorB2);
		_renderer.SetPropertyBlock(_matBlock);
	}

	#region MonoBehaviour
	private void OnEnable() {
		_renderer = GetComponent<Renderer>();
		_matBlock = new MaterialPropertyBlock();
	}

	private void Start() {
		ApplyMaterial();
	}
	#endregion MonoBehaviour
}
