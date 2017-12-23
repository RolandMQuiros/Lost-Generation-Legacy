using System;
using UnityEngine;
using LostGen.Model;

namespace LostGen.Display {
    public class CombatantBuilder : PawnViewBuilder {
        [SerializeField]private PlayerTimelineController _timelines;
        public override void Attach(GameObject target, Pawn pawn) {
            Combatant combatant = pawn.GetComponent<Combatant>();
            if (combatant != null) {
                CombatantView combatantView = target.AddComponent<CombatantView>();
                combatantView.Pawn = pawn;
                target.AddComponent<CombatantAnimator>();
                PawnActionPreviewer previewer = target.AddComponent<PawnActionPreviewer>();
                if (previewer != null && pawn.GetComponent<Timeline>() != null) {
                    _timelines.ActionDone += previewer.OnActionDone; 
                    _timelines.ActionUndone += previewer.OnActionUndone;
                    _timelines.ActionsAdded += previewer.OnActionsAdded;
                }
            }
        }

        public override void Detach(GameObject target, Pawn pawn) {
            PawnActionPreviewer previewer = target.GetComponent<PawnActionPreviewer>();
            if (previewer != null) {
                _timelines.ActionDone -= previewer.OnActionDone;
			    _timelines.ActionUndone -= previewer.OnActionUndone;
			    _timelines.ActionsAdded -= previewer.OnActionsAdded;
            }
        }
    }
}