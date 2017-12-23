using System;
using System.Collections.Generic;
using UnityEngine.Events;
using LostGen.Model;

namespace LostGen.Display {
    [Serializable]public class PawnActionEvent : UnityEvent<PawnAction> { }
    [Serializable]public class PawnActionsEvent : UnityEvent<IEnumerable<PawnAction>> { }
    [Serializable]public class BooleanEvent : UnityEvent<bool> { }
}