using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    public abstract class AreaOfEffectSkill : ISkill {
        public Pawn Owner { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public virtual int ActionPoints { get; }
        public bool IsReadyToFire { get; set; }
        public event Action AreaOfEffectChanged;

        public AreaOfEffectSkill(Pawn owner, string name, string description) {
            Owner = owner;
            Name = name;
            Description = description;
        }

        public abstract IEnumerable<Point> GetAreaOfEffect();
        public abstract PawnAction Fire();

        protected void InvokeAreaOfEffectChange()
        {
            if (AreaOfEffectChanged != null)
            {
                AreaOfEffectChanged();
            }
        }
    }
}
