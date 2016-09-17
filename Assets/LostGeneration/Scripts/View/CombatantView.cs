using UnityEngine;
using System.Collections;

using LostGen;

public class CombatantView : MonoBehaviour {
    private Animator _animator;

	
    // Use this for initialization
	public void Awake () {
        _animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	public void Update () {
	
	}

    public void OnMessage(MessageArgs message) {

    }
}
