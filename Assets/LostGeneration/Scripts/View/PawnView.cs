using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

///<summary>
/// Receives IPawnMessages from the Model layer and distributes them to each PawnComponentView on the View layer
///</summary>
public class PawnView : MonoBehaviour {
	private List<PawnComponentView> _components;

	public void HandleMessage(IPawnMessage message)
	{
		for (int i = 0; i < _components.Count; i++)
		{
			_components[i].HandleMessage(message);
		}
	}

	#region MonoBehaviour
	private void Start()
	{
		_components = new List<PawnComponentView>
		(
			GetComponents(typeof(PawnComponentView)).Cast<PawnComponentView>()
		);
	}
	#endregion MonoBehaviour
}
