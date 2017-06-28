using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LostGen;

[RequireComponent(typeof(SkillController))]
public class PlayerCombatantController : MonoBehaviour
{
    public int TimelineStep
    {
        get { return _step; }
        set { SetStep(value); }
    }

    [Serializable]
    public class CombatantEvent : UnityEvent<Combatant> { }

    public CombatantEvent CombatantActivated;
    
    private SkillController _skillController;
    private List<Combatant> _combatants = new List<Combatant>();
    private Dictionary<Combatant, PawnActionTimeline> _timelines = new Dictionary<Combatant, PawnActionTimeline>();
    private Combatant _activeCombatant = null;
    private int _activeIdx = 0;

    private int _step = 0;
    
    public void AddCombatant(Combatant combatant)
    {
        if (!_combatants.Contains(combatant))
        {
            _combatants.Add(combatant);
            PawnActionTimeline timeline = new PawnActionTimeline();
            _timelines[combatant] = timeline;
        }
    }
    
    public void SetAction(PawnAction action)
    {
        PawnActionTimeline timeline;
        if (_timelines.TryGetValue(action.Owner.GetComponent<Combatant>(), out timeline))
        {
            timeline.TruncateAt(_step);
            timeline.PushBack(action);
            SetStep(_step + 1);
        }
    }

    public void SetStep(int step)
    {
        foreach (PawnActionTimeline timeline in _timelines.Values)
        {
            timeline.SetStep(step);
        }
    }

    public Combatant CycleForward()
    {
        if (_combatants.Count > 0)
        {
            if (_activeIdx < _combatants.Count &&
                _combatants[_activeIdx] != _activeCombatant)
            {
                _activeIdx = 0;
            }
            else
            {
                _activeIdx = _activeIdx + 1 % _combatants.Count;
            }

            if (_activeCombatant != _combatants[_activeIdx])
            {
                _activeCombatant = _combatants[_activeIdx];
                _skillController.OnCombatantActivated(_activeCombatant);
                CombatantActivated.Invoke(_activeCombatant);
            }
        }
        else if (_activeCombatant != null) {
            _activeCombatant = null;
            CombatantActivated.Invoke(null);
        }

        return _activeCombatant;
    }

    private void Awake()
    {
        _skillController = GetComponent<SkillController>();
    }
}