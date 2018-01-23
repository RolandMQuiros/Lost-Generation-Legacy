using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LostGen.Model;

namespace LostGen.Display {
    [RequireComponent(typeof(PlayerPawnController))]
    [RequireComponent(typeof(TimelineSync))]
    public class PlayerSkillController : MonoBehaviour {
        public Skill ActiveSkill {
            get { return _activeSkill; }
            set { SetActiveSkill(value); }
        }
        [Serializable]public class SkillEvent : UnityEvent<Skill> { }
        public SkillEvent SkillActivated;
        public SkillEvent SkillFired;
        private Timeline _timeline;
        private TimelineSync _sync;
        private Skill _activeSkill;

        public void SetActiveSkill(Skill skill) {
            if (skill == null) {
                DeactivateSkill();
            } else {
                _activeSkill = skill;
                _timeline = _activeSkill.Pawn.GetComponent<Timeline>();
                _sync.Step = _timeline.Step;
                SkillActivated.Invoke(_activeSkill);
            }
        }

        public void DeactivateSkill() {
            _activeSkill = null;
            _timeline = null;
            SkillActivated.Invoke(null);
        }

        public void FireActiveSkill() {
            if (_activeSkill != null) {
                IEnumerable<PawnAction> actions = _activeSkill.Fire();
                _timeline.Splice(actions);
                _timeline.ToLast();
                SkillFired.Invoke(_activeSkill);
                DeactivateSkill();
            }
        }

        private void Awake() {
            _sync = GetComponent<TimelineSync>();
        }
    }
}