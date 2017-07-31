using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LostGen;

public class SkillTray : MonoBehaviour
{
    [SerializeField]private PlayerSkillController _skillController;
    [SerializeField]private GameObject _buttonPrefab;
    [SerializeField]private Transform _buttonParent;
    private List<SkillButton> _buttons = new List<SkillButton>();

    public void SetupButtons(Combatant combatant)
    {
        SkillSet skillSet = combatant.Pawn.GetComponent<SkillSet>();
        if (skillSet == null) {
            _buttons.ForEach(button => button.gameObject.SetActive(false));
        }
        else {
            int buttonIdx = 0;
            foreach (ISkill skill in skillSet.Skills) {
                SkillButton skillButton;
                if (buttonIdx < _buttons.Count) {
                    skillButton = _buttons[buttonIdx];
                } else {
                    GameObject buttonObj = GameObject.Instantiate(_buttonPrefab, _buttonParent);
                    skillButton = buttonObj.GetComponent<SkillButton>();
                    skillButton.SkillActivated += _skillController.SetActiveSkill;
                    _buttons.Add(skillButton);
                }
                
                skillButton.Skill = skill;
                skillButton.gameObject.SetActive(true);
                buttonIdx++;
            }

            _buttons.ForEach(button => button.CheckActionPoints());
        }
    }

    public void CheckActionPoints() {
        _buttons.ForEach(button => button.CheckActionPoints());
    }

    #region MonoBehaviour
    private void Awake() {
        _buttonPrefab.SetActive(false);
    }
    #endregion MonoBehaviour
}