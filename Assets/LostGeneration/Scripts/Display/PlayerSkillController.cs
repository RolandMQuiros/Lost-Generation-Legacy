using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LostGen.Model;

namespace LostGen.Display {
    [RequireComponent(typeof(PlayerCombatantController))]
    [RequireComponent(typeof(PlayerTimelineController))]
    public class PlayerSkillController : MonoBehaviour
    {

        [System.Serializable]
        public class SkillEvent : UnityEvent<Skill> { }
        public Skill ActiveSkill {
            get { return _activeSkill; }
        }
        public SkillEvent SkillActivated;
        public SkillEvent SkillFired;

        private PlayerCombatantController _playerController;
        private PlayerTimelineController _timelineController;

        private Combatant _combatant;
        private Timeline _timeline;
        private Skill _activeSkill;
        [SerializeField]
        private int _activationStep;

        public void SetActiveSkill(Skill skill) {
            if (skill.Pawn == _combatant.Pawn) {
                _activeSkill = skill;
                
                // Save the timeline step when skill was activated
                //_activationStep = Math.Min(Math.Max(0, _timeline.Count), _timelineController.Step);
                // Move forward all timelines for Combatants that are slower than the active Combatant
                //_timelineController.SetSlowerStep(_activationStep + 1, _activeSkill.Pawn);

                SkillActivated.Invoke(_activeSkill);
            }
        }

        public void CancelSkill() {
            if (_activeSkill != null) {
                // Rewind timeline back to when skill was activated
                //_timelineController.SetAllStep(_activationStep);
                DeactivateSkill();
            }
        }

        public void DeactivateSkill() {
            _activeSkill = null;
            SkillActivated.Invoke(null);
        }
        
        public void OnCombatantActivated(Combatant combatant)
        {
            _combatant = combatant;
            _timeline = _combatant.Pawn.GetComponent<Timeline>();
        }

        public void FireActiveSkill()
        {
            if (_activeSkill != null) {
                IEnumerable<PawnAction> actions = _activeSkill.Fire();
                _timelineController.SetStepActions(actions);
                SkillFired.Invoke(_activeSkill);
                DeactivateSkill();
            }
        }

        #region MonoBehaviour
        private void Awake() {
            _playerController = GetComponent<PlayerCombatantController>();
            _timelineController = GetComponent<PlayerTimelineController>();
        }
        #endregion MonoBehaviour
    }
}