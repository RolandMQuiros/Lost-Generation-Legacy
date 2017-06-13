using System;
using UnityEngine;
using UnityEngine.UI;
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

    private void Awake()
    {
        Text buttonText = GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.text = Skill.Name;
        }
    }
}