using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "BlockProperties", menuName = "Lost Generation/BlockProperties")]
public class BlockProperties : ScriptableObject {
	public Sprite TopSprite;
	public Sprite RightSprite;
	public Sprite DownSprite;
	public Sprite LeftSprite;
	public Sprite ForwardSprite;
	public Sprite BackwardSprite;

	public Sprite[] SideSprites
	{
		get { return _sideSprites; }
	}

	private Sprite[] _sideSprites;
	private void OnEnable()
	{
		_sideSprites = new Sprite[]
		{
			TopSprite,
			RightSprite,
			DownSprite,
			LeftSprite,
			ForwardSprite,
			BackwardSprite
		};
	}
}
