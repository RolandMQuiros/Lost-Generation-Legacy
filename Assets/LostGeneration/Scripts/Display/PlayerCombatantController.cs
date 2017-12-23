using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LostGen.Model;

namespace LostGen.Display {
    [RequireComponent(typeof(PlayerSkillController))]
    [RequireComponent(typeof(PlayerTimelineController))]
    public class PlayerCombatantController : MonoBehaviour {
        public event Action<Combatant> CombatantActivated; 
        
        private PlayerSkillController _skillController;
        private PlayerTimelineController _timelines;

        private List<Combatant> _combatants = new List<Combatant>();
        private Combatant _activeCombatant = null;
        private int _activeIdx = 0;
        private int _step = 0;
        [SerializeField]private int _debugActionPoints;
        
        public void AddCombatant(Combatant combatant) {
            if (!_combatants.Contains(combatant))
            {
                _combatants.Add(combatant);
                _timelines.AddPawn(combatant.Pawn);
            }
        }
        
        public Combatant CycleForward() {
            if (_combatants.Count > 0)
            {
                if (_activeIdx < _combatants.Count && _combatants[_activeIdx] != _activeCombatant) {
                    _activeIdx = 0;
                } else {
                    _activeIdx = (_activeIdx + 1) % _combatants.Count;
                }

                if (_activeCombatant != _combatants[_activeIdx]) {
                    _activeCombatant = _combatants[_activeIdx];
                    _skillController.OnCombatantActivated(_activeCombatant);
                    
                    Timeline timeline = _activeCombatant.Pawn.GetComponent<Timeline>();
                    if (timeline != null) {
                        int step = Math.Min(_timelines.Step, timeline.Count);
                        _timelines.SetAllStep(step);
                    }
                }
            }
            else if (_activeCombatant != null) {
                _activeCombatant = null;
                _skillController.DeactivateSkill();
            }
            if (CombatantActivated != null) { CombatantActivated(_activeCombatant); }

            return _activeCombatant;
        }

        private void Awake() {
            _skillController = GetComponent<PlayerSkillController>();
            _timelines = GetComponent<PlayerTimelineController>();
        }

        private void Update() {
            _debugActionPoints = _activeCombatant.ActionPoints.Current;
            if (Input.GetKeyUp(KeyCode.Tab)) {
                _skillController.CancelSkill();
                CycleForward();
            }

            if (Input.GetKeyUp(KeyCode.Escape)) {
                _skillController.CancelSkill();
            }

            if (Input.GetKeyDown(KeyCode.LeftBracket)) {
                _skillController.CancelSkill();
                _timelines.SetAllStep(_timelines.Step - 1);
            }
            
            if (Input.GetKeyDown(KeyCode.RightBracket)) {
                _skillController.CancelSkill();
                _timelines.SetAllStep(_timelines.Step + 1);
            }
        }
    }
}