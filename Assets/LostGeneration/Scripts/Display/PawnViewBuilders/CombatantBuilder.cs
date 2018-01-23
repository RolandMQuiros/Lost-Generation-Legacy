using System;
using UnityEngine;
using LostGen.Model;

namespace LostGen.Display {
    public class CombatantBuilder : PawnViewBuilder {
        [SerializeField]private TimelineSync _timelineSync;
        public override void Attach(GameObject target, Pawn pawn) {
            Combatant combatant = pawn.GetComponent<Combatant>();
            if (combatant != null) {
                if (target.GetComponent<CombatantView>() == null) { target.AddComponent<CombatantView>(); }
                TimelinePreviewer previewer = target.GetComponent<TimelinePreviewer>() ?? target.AddComponent<TimelinePreviewer>();
                previewer.Timeline = pawn.RequireComponent<Timeline>();
                _timelineSync.AddTimeline(previewer.Timeline);
            }
        }

        public override void Detach(GameObject target, Pawn pawn) {
            TimelinePreviewer previewer = target.GetComponent<TimelinePreviewer>();
            _timelineSync.RemoveTimeline(previewer.Timeline);
            previewer.Timeline = null;
        }
    }
}