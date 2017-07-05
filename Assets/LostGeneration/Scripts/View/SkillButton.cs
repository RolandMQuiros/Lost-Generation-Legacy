using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using LostGen;

[RequireComponent(typeof(Button))]
public class SkillButton : MonoBehaviour,
                           IPointerClickHandler
{
    public ISkill Skill {
        get { return _skill; }
        set { _skill = value; }
    }

    public event Action<ISkill> SkillActivated; 

    private ISkill _skill;
    private Button _button;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (SkillActivated != null)
        {
            SkillActivated(Skill);
        }
    }

    #region MonoBehaviour
    private void Awake()
    {
        _button = GetComponent<Button>();
    }
    private void Start()
    {
        Text buttonText = GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.text = Skill.Name;
        }
    }
    private void OnEnable()
    {
        _button.interactable = _skill.IsUsable();
    }
    #endregion MonoBehaviour
}