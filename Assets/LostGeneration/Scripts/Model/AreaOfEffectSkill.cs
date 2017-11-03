using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public abstract class AreaOfEffectSkill : Skill {
        public override abstract int ActionPoints { get; }
        public event Action AreaOfEffectChanged;
        private ActionPoints _actionPoints;

        public AreaOfEffectSkill(string name, string description)
        : base(name, description) { }

        protected override void Awake() {
            _actionPoints = Pawn.RequireComponent<ActionPoints>();
        } 

        public abstract IEnumerable<Point> GetAreaOfEffect();
        public override abstract IEnumerable<PawnAction> Fire();
        
        public override bool IsUsable() {
            return _actionPoints.Current >= 1;
        }

        protected void InvokeAreaOfEffectChange() {
            if (AreaOfEffectChanged != null) { AreaOfEffectChanged(); }
        }
    }
}
