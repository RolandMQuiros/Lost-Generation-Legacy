using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen.Model {
    public class Gear : Item {
        public Stats Modifier { get; private set; }

        public Gear(string name, string description, Stats modifier)
            : base(name, description) {


        }
    }
}
