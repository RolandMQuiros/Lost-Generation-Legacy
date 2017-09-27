using System;
using System.Collections.Generic;
using UnityEngine.Events;
using LostGen;

[Serializable]
public class PawnActionEvent : UnityEvent<PawnAction> { }

[Serializable]
public class PawnActionsEvent : UnityEvent<IEnumerable<PawnAction>> { }