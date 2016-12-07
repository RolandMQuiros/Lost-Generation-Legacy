using System;
using UnityEngine.Events;
using LostGen;

[Serializable]
public class CombatantEvent : UnityEvent<Combatant> { }

[Serializable]
public class CombatantSkillEvent : UnityEvent<Combatant, ISkill> { }

[Serializable]
public class PointEvent : UnityEvent<Point> { }

[Serializable]
public class SkillEvent : UnityEvent<ISkill> { }