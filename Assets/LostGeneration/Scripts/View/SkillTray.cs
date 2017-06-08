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
                SkillButton button;
                if (buttonIdx < _buttons.Count)
                {
                    button = _buttons[buttonIdx];
                }
                else
                {
                    GameObject buttonObj = GameObject.Instantiate(_buttonPrefab, _buttonParent);
                    button = buttonObj.GetComponent<SkillButton>();
                    button.SkillActivated += _skillController.SetActiveSkill;

                    _buttons.Add(button);
                }
                
                button.transform.localPosition = new Vector3
                (
                    _buttonPadding + (_buttonPadding + _buttonRect.width) * (buttonIdx % _buttonsPerRow),
                    -(_buttonPadding + (_buttonPadding + _buttonRect.height) * (buttonIdx / _buttonsPerRow)),
                    0f
                );
                button.Skill = skill;
                button.gameObject.SetActive(true);
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