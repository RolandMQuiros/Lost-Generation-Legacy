using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LostGen.Model;

namespace LostGen.Display {
    public class SkillTray : MonoBehaviour
    {
        [SerializeField]private PlayerSkillController _skillController;
        [SerializeField]private GameObject _buttonPrefab;
        [SerializeField]private Transform _buttonParent;
        private List<SkillButton> _buttons = new List<SkillButton>();

        public void Build(Combatant combatant)
        {
            IEnumerable<Skill> skills = combatant.Pawn.GetComponents<Skill>();
            if (!skills.Any()) {
                _buttons.ForEach(button => button.gameObject.SetActive(false));
            }
            else {
                int buttonIdx = 0;
                foreach (Skill skill in combatant.Pawn.GetComponents<Skill>()) {
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
}