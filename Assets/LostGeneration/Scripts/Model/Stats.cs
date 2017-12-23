using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen.Model {
    public struct Stats {
        public int Health;
        public int Attack;
        public int Defense;
        public int Magic;
        /// <summary>Determines sorting order</summary>
        public int Agility;
        /// <summary>Number of Action Points replenished every turn</summary>
        public int Stamina;

        public static Stats operator + (Stats s1, Stats s2) {
            return new Stats {
                Health = s1.Health + s2.Health,
                Attack = s1.Attack + s2.Attack,
                Defense = s1.Defense + s2.Defense,
                Magic = s1.Magic + s2.Magic,
                Agility = s1.Agility + s2.Agility,
                Stamina = s1.Stamina + s2.Stamina
            };
        }

        public static Stats operator - (Stats s1, Stats s2) {
            return new Stats {
                Health = s1.Health - s2.Health,
                Attack = s1.Attack - s2.Attack,
                Defense = s1.Defense - s2.Defense,
                Magic = s1.Magic - s2.Magic,
                Agility = s1.Agility - s2.Agility,
                Stamina = s1.Stamina - s2.Stamina
            };
        }
    }
}
