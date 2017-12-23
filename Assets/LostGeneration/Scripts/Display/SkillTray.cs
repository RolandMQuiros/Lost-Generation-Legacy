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
        private List<GameObject> _pool = new List<GameObject>();
        private Dictionary<Skill, Button> _buttons = new Dictionary<Skill, Button>(); 

        public void Build(Combatant combatant)
        {
            IEnumerable<Skill> skills = combatant.Pawn.GetComponents<Skill>();
            
            _pool.ForEach(button => button.SetActive(false));
            foreach (Skill skill in combatant.Pawn.GetComponents<Skill>()) {
                Button button;
                if (!_buttons.TryGetValue(skill, out button)) {
                    GameObject buttonObj = _pool.FirstOrDefault(b => !b.activeInHierarchy);
                    if (buttonObj == null) {
                        buttonObj = GameObject.Instantiate(_buttonPrefab, _buttonParent);
                        _pool.Add(buttonObj);
                    }
                    button = buttonObj.GetComponent<Button>();
                    button.onClick.AddListener(() => { _skillController.SetActiveSkill(skill); });
                    _buttons[skill] = button;

                    Text text = button.GetComponentInChildren<Text>();
                    text.text = skill.Name;
                }
                
                button.gameObject.SetActive(true);
            }
            CheckActionPoints();
        }

        public void CheckActionPoints() {
            foreach (KeyValuePair<Skill, Button> pair in _buttons) {
                ActionPoints actionPoints = pair.Key.Pawn.GetComponent<ActionPoints>();
                if (actionPoints != null) {
                    pair.Value.interactable = pair.Key.ActionPoints <= actionPoints.Current;
                }
            }
        }

        #region MonoBehaviour
        private void Awake() {
            _buttonPrefab.SetActive(false);
        }
        #endregion MonoBehaviour
    }
}