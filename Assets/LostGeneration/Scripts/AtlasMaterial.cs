using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AtlasMaterial : MonoBehaviour {
	public Sprite Sprite;
	public Color DarkColor;
	public Color ColorR1;
	public Color ColorR2;
	public Color ColorG1;
	public Color ColorG2;
	public Color ColorB1;
	public Color ColorB2;
	private Renderer _renderer;
	private MaterialPropertyBlock _matBlock;
	
	public Rect DebugTextureRect;
	public Vector4 DebugUVs;

	public void ApplyMaterial() {
		_renderer.GetPropertyBlock(_matBlock);

		Vector4 uvOffset = new Vector4(
			Sprite.rect.x / Sprite.texture.width,
			Sprite.rect.y / Sprite.texture.height,
			Sprite.rect.size.x / Sprite.texture.width,
			Sprite.rect.size.y / Sprite.texture.height
		);

		_matBlock.SetVector("_UVOffset", uvOffset);
		//_matBlock.SetColor("_DarkColor", DarkColor);
		_matBlock.SetColor("_ColorR1", ColorR1);
		_matBlock.SetColor("_ColorR2", ColorR2);
		_matBlock.SetColor("_ColorG1", ColorG1);
		_matBlock.SetColor("_ColorG2", ColorG2);
		_matBlock.SetColor("_ColorB1", ColorB1);
		_matBlock.SetColor("_ColorB2", ColorB2);
		_renderer.SetPropertyBlock(_matBlock);

		DebugTextureRect = Sprite.rect;
		DebugUVs = uvOffset;
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
