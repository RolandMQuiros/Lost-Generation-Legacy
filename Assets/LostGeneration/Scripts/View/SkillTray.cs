using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LostGen;

public class SkillTray : MonoBehaviour
{
    [SerializeField]private SkillController _skillController;
    [SerializeField]private GameObject _buttonPrefab;
    [SerializeField]private Transform _buttonParent;
    [SerializeField]private float _buttonPadding = 8f;
    [SerializeField]private int _buttonsPerRow = 8;
    private List<SkillButton> _buttons = new List<SkillButton>();
    private Rect _buttonRect;

    public void SetupButtons()
    {
        if (_skillController.SkillSet == null)
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                _buttons[i].gameObject.SetActive(false);
            }
        }
        else
        {
            int buttonIdx = 0;
            foreach (ISkill skill in _skillController.SkillSet.Skills)
            {
                SkillButton skillButton;
                if (buttonIdx < _buttons.Count)
                {
                    skillButton = _buttons[buttonIdx];
                }
                else
                {
                    GameObject buttonObj = GameObject.Instantiate(_buttonPrefab, _buttonParent);
                    skillButton = buttonObj.GetComponent<SkillButton>();
                    skillButton.SkillActivated += _skillController.SetActiveSkill;
                    _buttons.Add(skillButton);
                }
                
                skillButton.Skill = skill;
                skillButton.gameObject.SetActive(true);
                buttonIdx++;
            }
        }
    }

    #region MonoBehaviour

    private void Awake()
    {
        _buttonRect = ((RectTransform)_buttonPrefab.transform).rect;
        _buttonPrefab.SetActive(false);
    }
    
    #endregion
}