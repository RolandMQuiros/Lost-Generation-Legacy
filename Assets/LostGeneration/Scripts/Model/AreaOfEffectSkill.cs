using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public abstract class AreaOfEffectSkill : ISkill {
        public Pawn Pawn { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public virtual int ActionPoints { get; }
        public event Action AreaOfEffectChanged;
        private Combatant _combatant;

        public AreaOfEffectSkill(Pawn owner, string name, string description) {
            Pawn = owner;
            Name = name;
            Description = description;
            _combatant = Pawn.GetComponent<Combatant>();
        }

        public abstract IEnumerable<Point> GetAreaOfEffect();
        public abstract PawnAction Fire();
        
        public virtual bool IsUsable()
        {
            return _combatant.ActionPoints >= ActionPoints;
        }

        protected void InvokeAreaOfEffectChange()
        {
            if (AreaOfEffectChanged != null)
            {
                AreaOfEffectChanged();
            }
        }
    }
}
