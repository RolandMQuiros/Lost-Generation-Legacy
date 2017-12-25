using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LostGen.Model;

namespace LostGen.Display {
    [RequireComponent(typeof(PlayerSkillController))]
    [RequireComponent(typeof(TimelineSync))]
    public class PlayerPawnController : MonoBehaviour {
        [Serializable]public class PawnEvent : UnityEvent<Pawn> { }
        public PawnEvent PawnActivated;
        private PlayerSkillController _skillController;
        private TimelineSync _timelines;
        private List<Pawn> _pawns = new List<Pawn>();
        private Pawn _activePawn = null;
        private int _activeIdx = 0;
        private int _step = 0;
        
        public void AddPawn(Pawn pawn) {
            if (!_pawns.Contains(pawn)) {
                _pawns.Add(pawn);
            }
        }
        public Pawn CycleForward() {
            if (_pawns.Count > 0) {
                if (_activeIdx < _pawns.Count && _pawns[_activeIdx] != _activePawn) {
                    _activeIdx = 0;
                }
                else {
                    _activeIdx = (_activeIdx + 1) % _pawns.Count;
                }

                if (_activePawn != _pawns[_activeIdx]) {
                    _activePawn = _pawns[_activeIdx];
                    _skillController.ActiveSkill = null;;
                    
                    Timeline timeline = _activePawn.GetComponent<Timeline>();
                    if (timeline != null) {
                        _timelines.Step = Math.Min(_timelines.Step, timeline.Count);
                    }
                }
            }
            else if (_activePawn != null) {
                _activePawn = null;
                _skillController.DeactivateSkill();
            }
            PawnActivated.Invoke(_activePawn);

            return _activePawn;
        }

        private void Awake() {
            _skillController = GetComponent<PlayerSkillController>();
            _timelines = GetComponent<TimelineSync>();
        }

        private void Update() {
            if (Input.GetKeyUp(KeyCode.Tab)) {
                _skillController.DeactivateSkill();
                CycleForward();
            }
            if (Input.GetKeyUp(KeyCode.Escape)) {
                _skillController.DeactivateSkill();
            }
            if (Input.GetKeyDown(KeyCode.LeftBracket)) {
                _skillController.DeactivateSkill();
                _timelines.Step = _timelines.Step - 1;
            }
            if (Input.GetKeyDown(KeyCode.RightBracket)) {
                _skillController.DeactivateSkill();
                _timelines.Step = _timelines.Step + 1;
            }
        }
    }
}