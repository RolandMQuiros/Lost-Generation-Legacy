using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LostGen.Model;
using LostGen.Util;

namespace LostGen.Display {
	[RequireComponent(typeof(BoardCamera))]
	public class BoardCameraEventHandlers : MonoBehaviour {
		[SerializeField]private float _panDuration = 0.25f;
		private BoardCamera _camera;

		public void OnPawnActivated(Pawn pawn) {
			_camera.Pan(pawn.Position, _panDuration);
		}	

		private void Awake() {
			_camera = GetComponent<BoardCamera>();
		}	
	}
}