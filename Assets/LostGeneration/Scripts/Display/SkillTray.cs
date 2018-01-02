using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LostGen.Model;

namespace LostGen.Display {
    public class SkillTray : MonoBehaviour {
        public Pawn Pawn {
            get { return _pawn; }
            set {
                if (value != null && _pawn != value) {
                    Unbind();
                    Bind(value);
                    Build(value);
                }
            }
        }
        public bool Interactable {
            get { return _interactable; }
            set {
                _interactable = value;
                CheckUsability();
            }
        }
        #region Fields
        [SerializeField]private GameObject _buttonPrefab;
        [SerializeField]private Transform _buttonParent;
        [Serializable]public class SkillEvent : UnityEngine.Events.UnityEvent<Skill> { }
        public SkillEvent SkillActivated;
        #endregion
        #region Private Members
        private Pawn _pawn = null;
        private Timeline _timeline = null;
        private List<GameObject> _pool = new List<GameObject>();
        private Dictionary<Skill, Button> _buttons = new Dictionary<Skill, Button>(); 
        private bool _interactable = true;

        private void Bind(Pawn pawn) {
            _pawn = pawn;
            _timeline = pawn.GetComponent<Timeline>();
            _timeline.ActionDone += OnTimelineChanged;
            _timeline.ActionUndone += OnTimelineChanged;
        }
        private void Unbind() {
            if (_timeline != null) {
                _timeline.ActionUndone -= OnTimelineChanged;
                _timeline.ActionDone -= OnTimelineChanged;
            }
        }
        private void Build(Pawn pawn) {
            foreach (Button button in _buttons.Values) {
                button.gameObject.SetActive(false);
            }
            _buttons.Clear();

            foreach (Skill skill in pawn.GetComponents<Skill>()) {
                Button button;
                if (!_buttons.TryGetValue(skill, out button)) {
                    GameObject buttonObj = _pool.FirstOrDefault(b => !b.activeInHierarchy);
                    if (buttonObj == null) {
                        buttonObj = GameObject.Instantiate(_buttonPrefab, _buttonParent);
                        _pool.Add(buttonObj);
                    }
                    button = buttonObj.GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => { SkillActivated.Invoke(skill); });
                    _buttons[skill] = button;

                    Text text = button.GetComponentInChildren<Text>();
                    text.text = skill.Name;
                }
                
                button.gameObject.SetActive(true);
            }
        }
        private void OnTimelineChanged(PawnAction action) { CheckUsability(); }
        private void CheckUsability() {
            foreach (KeyValuePair<Skill, Button> pair in _buttons) {
                pair.Value.interactable = _interactable && pair.Key.IsUsable;
            }
        }
        #endregion
        #region MonoBehaviour
        private void Awake() { _buttonPrefab.SetActive(false); }
        #endregion MonoBehaviour
    }
}