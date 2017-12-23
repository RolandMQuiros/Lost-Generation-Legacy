using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LostGen.Model;

namespace LostGen.Display {

    /// <summary>
    /// Handles the event binds for all the controllers and view objects to create a cohesive player control flow.
    /// This was originally accomplished with tons of UnityEvents, but those were annoying as hell to actually manage.
    /// </summary>
    public class PlayerController : MonoBehaviour {
        private PlayerCombatantController _combatantController;
        private PlayerSkillController _skillController;
        private PlayerTimelineController _timelineController;
        
        [SerializeField]private BoardRef _boardRef;
        [SerializeField]private BoardCamera _camera;
        [SerializeField]private BoardCursor _cursor;
        [SerializeField]private SkillTray _skillTray;
        [SerializeField]private AreaOfEffectSkillController _areaOfEffectController;
        [SerializeField]private PawnViewManager _pawnViewManager;
        [SerializeField]private Button _goButton;


        private void Awake() {
            _combatantController = GetComponent<PlayerCombatantController>();
            _skillController = GetComponent<PlayerSkillController>();
            _timelineController = GetComponent<PlayerTimelineController>();

            _combatantController.CombatantActivated += OnCombatantActivated;
            _skillController.SkillActivated += OnSkillActivated;
            _skillController.SkillFired += OnSkillFired;
            _timelineController.ActionDone += OnActionDone;
            _timelineController.ActionUndone += OnActionUndone;
            _timelineController.ActionsAdded += OnActionsAdded;
            _timelineController.ActionInterrupted += OnActionInterrupted;
            _timelineController.AllPointsSpent += OnAllPointsSpent;

            _cursor.Moved += _areaOfEffectController.SetTarget;
            _cursor.TappedDown += _areaOfEffectController.SetTarget;
            _cursor.TappedUp += p => { _skillController.FireActiveSkill(); };

            _goButton.onClick.AddListener(Go);
        }

        private void OnCombatantActivated(Combatant combatant) {
            _skillTray.Build(combatant);
            _areaOfEffectController.ClearSkill();
        }

        private void OnSkillActivated(Skill skill) {
            _areaOfEffectController.SetSkill(skill);
        }

        private void OnSkillFired(Skill skill) {
            _skillTray.CheckActionPoints();
        }

        private void OnActionDone(PawnAction action) {
            _skillTray.CheckActionPoints();
        }

        private void OnActionUndone(PawnAction action) {
            _skillTray.CheckActionPoints();
        }

        private void OnActionsAdded(IEnumerable<PawnAction> actions) {

        }

        private void OnActionInterrupted(PawnAction action) {

        }

        private void OnAllPointsSpent() {
            _goButton.gameObject.SetActive(true);
        }

        public void Go() {
            // Move actions from timelines into pawn queues
            _timelineController.ApplyTimelines();
            // Process the actions
            _boardRef.Turn();
            // 
            _pawnViewManager.HandleMessages();
            _boardRef.BeginTurn();
        }
    }
}