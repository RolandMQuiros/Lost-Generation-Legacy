using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockSelector : MonoBehaviour {
	public enum SelectorAxis {
		XZ, XY
	}

	public SelectorAxis Axis;
	public int Row;
	public int Column;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
