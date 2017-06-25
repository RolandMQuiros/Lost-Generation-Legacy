using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LostGen;

public class PlayerCombatantController : MonoBehaviour
{
    public int TimelineStep
    {
        get { return _step; }
        set { SetStep(value); }
    };

    [Serializable]
    public class CombatantEvent : UnityEvent<Combatant> { }

    public CombatantEvent CombatantActivated;
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
            _timelines.Insert
            PawnActionTimeline timeline = new PawnActionTimeline();
            timeline.SetStep(_step);
            _timelines[combatant] = timeline;
        }
    }
    
    public void PushAction(Combatant combatant, PawnAction action)
    {
        PawnActionTimeline timeline;
        if (_timelines.TryGetValue(combatant, ref timeline))
        {
            timeline.TruncateAt(_step);
            timeline.PushBack(action);
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
                CombatantActivated.Invoke(_activeCombatant);
            }
        }
        else if (_activeCombatant != null) {
            _activeCombatant = null;
            CombatantActivated.Invoke(null);
        }

        return _activeCombatant;
    }
}