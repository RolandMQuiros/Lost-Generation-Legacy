using System;
using UnityEngine;
using UnityEngine.EventSystems;
using LostGen;

public class SkillButton : MonoBehaviour,
                           IPointerClickHandler
{
    public ISkill Skill { get; set; }

    public event Action<ISkill> SkillActivated; 

    public void OnPointerClick(PointerEventData eventData)
    {
        if (SkillActivated != null)
        {
            SkillActivated(Skill);
        }
    }
}