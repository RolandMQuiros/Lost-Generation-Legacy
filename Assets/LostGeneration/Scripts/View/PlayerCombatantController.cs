using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LostGen;

[RequireComponent(typeof(SkillController))]
[RequireComponent(typeof(PlayerTimelineController))]
public class PlayerCombatantController : MonoBehaviour {
    [Serializable]
    public class CombatantEvent : UnityEvent<Combatant> { }

    public CombatantEvent CombatantActivated;
    
    private SkillController _skillController;
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
            if (_activeIdx < _combatants.Count &&
                _combatants[_activeIdx] != _activeCombatant)
            {
                _activeIdx = 0;
            }
            else
            {
                _activeIdx = (_activeIdx + 1) % _combatants.Count;
            }

            if (_activeCombatant != _combatants[_activeIdx])
            {
                _activeCombatant = _combatants[_activeIdx];
                _skillController.OnCombatantActivated(_activeCombatant);
                
                Timeline timeline = _activeCombatant.Pawn.GetComponent<Timeline>();
                if (timeline != null) {
                    int step = Math.Min(_timelines.Step, timeline.Count);
                    _timelines.SetStep(step);
                }

                CombatantActivated.Invoke(_activeCombatant);
            }
        }
        else if (_activeCombatant != null) {
            _activeCombatant = null;
            _skillController.DeactivateSkill();
            CombatantActivated.Invoke(null);
        }

        return _activeCombatant;
    }

    private void Awake() {
        _skillController = GetComponent<SkillController>();
        _timelines = GetComponent<PlayerTimelineController>();
    }

    private void Update() {
        _debugActionPoints = _activeCombatant.ActionPoints.Current;
        if (Input.GetKeyUp(KeyCode.Tab)) {
            CycleForward();
        }

        if (Input.GetKeyDown(KeyCode.LeftBracket)) {
            _timelines.SetStep(_timelines.Step - 1);
		}
        
		if (Input.GetKeyDown(KeyCode.RightBracket)) {
            Timeline timeline = _activeCombatant.Pawn.GetComponent<Timeline>();
            if (timeline != null) {
			    _timelines.SetStep(_timelines.Step + 1);
            }
		}
    }
}