using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen.Model {
    public abstract class Item {
        public string Name { get; protected set; }
        public string Description { get; protected set; }

        public Item(string name, string description) {
            Name = name;
            Description = description;
        }
    }
}
