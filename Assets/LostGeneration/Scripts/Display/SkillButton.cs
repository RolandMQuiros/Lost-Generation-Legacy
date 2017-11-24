using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using LostGen;

[RequireComponent(typeof(Button))]
public class SkillButton : MonoBehaviour,
                           IPointerClickHandler
{
    public Skill Skill {
        get { return _skill; }
        set { _skill = value; }
    }

    public event Action<Skill> SkillActivated; 

    private Skill _skill;
    private Button _button;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (SkillActivated != null)
        {
            SkillActivated(Skill);
        }
    }

    public void CheckActionPoints()
    {
        ActionPoints actionPoints = _skill.Pawn.GetComponent<ActionPoints>();
        if (actionPoints != null) {
            _button.interactable = _skill.ActionPoints <= actionPoints.Current;
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

    private void OnEnable() {
        CheckActionPoints();
    }
    #endregion MonoBehaviour
}