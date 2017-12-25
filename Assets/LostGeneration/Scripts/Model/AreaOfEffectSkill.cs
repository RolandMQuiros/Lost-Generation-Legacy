using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen.Model {
    public abstract class AreaOfEffectSkill : Skill {
        public override abstract int ActionPoints { get; }
        public override bool IsUsable {
            get { return _actionPoints.Current >= 1; }
        }
        public event Action AreaOfEffectChanged;
        private ActionPoints _actionPoints;

        public AreaOfEffectSkill(string name, string description)
        : base(name, description) { }

        protected override void Awake() {
            _actionPoints = Pawn.RequireComponent<ActionPoints>();
        } 

        public abstract IEnumerable<Point> GetAreaOfEffect();
        public override abstract IEnumerable<PawnAction> Fire();

        protected void InvokeAreaOfEffectChange() {
            if (AreaOfEffectChanged != null) { AreaOfEffectChanged(); }
        }
    }
}
