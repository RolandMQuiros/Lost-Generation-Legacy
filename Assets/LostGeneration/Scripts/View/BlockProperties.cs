using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "BlockProperties", menuName = "Lost Generation/BlockProperties")]
public class BlockProperties : ScriptableObject {
	public Sprite TopSprite;
	public bool TopNormalReversed = false;
	public Sprite RightSprite;
	public bool RightNormalReversed = false;
	public Sprite DownSprite;
	public bool DownNormalReversed = false;
	public Sprite LeftSprite;
	public bool LeftNormalReversed = false;
	public Sprite ForwardSprite;
	public bool ForwardNormalReversed = false;
	public Sprite BackwardSprite;	
	public bool BackwardNormalReversed = false;

	public Sprite[] SideSprites
	{
		get { return _sideSprites; }
	}

	public bool [] AreNormalsReversed
	{
		get { return _normalsReversed; }
	}

	private Sprite[] _sideSprites;
	private bool[] _normalsReversed;
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

		_normalsReversed = new bool[]
		{
			TopNormalReversed,
			RightNormalReversed,
			DownNormalReversed,
			LeftNormalReversed,
			ForwardNormalReversed,
			BackwardNormalReversed
		};
	}
}
