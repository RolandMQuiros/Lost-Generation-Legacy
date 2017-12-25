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
        private PlayerPawnController _pawnController;
        private PlayerSkillController _skillController;
        private TimelineSync _timelineController;
        
        [SerializeField]private BoardRef _boardRef;
        [SerializeField]private BoardCamera _camera;
        [SerializeField]private BoardCursor _cursor;
        [SerializeField]private SkillTray _skillTray;
        [SerializeField]private AreaOfEffectSkillController _aoeController;
        [SerializeField]private PawnViewManager _pawnViewManager;
        [SerializeField]private Button _goButton;


        private void Awake() {
            _pawnController = GetComponent<PlayerPawnController>();
            _skillController = GetComponent<PlayerSkillController>();
            _timelineController = GetComponent<TimelineSync>();

            _cursor.Moved.AddListener(_aoeController.SetTarget);
            _cursor.TappedDown.AddListener(_aoeController.SetTarget);
            _cursor.TappedUp.AddListener(p => {_skillController.FireActiveSkill(); });
            
            _pawnViewManager.MessagesFinished.AddListener(() => { _goButton.gameObject.SetActive(false); });

            _goButton.onClick.AddListener(Go);
        }

        private void OnPawnActivated(Pawn pawn) {
            _skillTray.Pawn = pawn;
            _aoeController.Skill = null;
        }

        private void OnSkillActivated(Skill skill) {
            _aoeController.Skill = skill;
        }

        private void OnSkillFired(Skill skill) {
            
        }

        private void OnAllPointsSpent() {
            _goButton.gameObject.SetActive(true);
        }

        public void Go() {
            // Move actions from timelines into pawn queues
            _timelineController.ApplyTimelines();
            // Process the actions
            _boardRef.Turn();
            _pawnViewManager.HandleMessages();
            _boardRef.BeginTurn();
        }
    }
}