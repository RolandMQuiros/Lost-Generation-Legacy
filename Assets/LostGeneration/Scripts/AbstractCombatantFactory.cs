using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LostGen;

public abstract class AbstractCombatantFactory {

    public abstract CombatantView CreateCombatantView(int combatantID);
}
