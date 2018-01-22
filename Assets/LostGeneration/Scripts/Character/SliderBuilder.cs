using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using LostGen.Util;

namespace LostGen.Character {
    public class SliderBuilder : MonoBehaviour {
        [SerializeField]private Transform _contentParent;
        [SerializeField]private GameObject _sliderTemplate;
        [SerializeField]private GameObject _foldoutTemplate;
        [SerializeField]private CharacterBody _body;

        private NameTree _nameTree;
        private string _currentParent;
        private Queue<GameObject> _sliderPool = new Queue<GameObject>();
        private Queue<GameObject> _foldoutPool = new Queue<GameObject>();

        #region MonoBehavour
        private void Start() {
            _nameTree = new NameTree('.');
            _nameTree.Add(_body.BlendShapes.Keys);

            BuildPage();
        }
        #endregion

        public void GoUp() {
            if (_currentParent.Length > 0) {
                BuildPage(_nameTree.GetParent(_currentParent));
            }
        }

        public void BuildPage(string parent = "") {
            _currentParent = parent;

            foreach (GameObject s in _sliderPool) { s.SetActive(false); }
            foreach (GameObject f in _foldoutPool) { f.SetActive(false); }

            foreach (string child in _nameTree.GetChildren(parent)) {
                if (_nameTree.IsLeaf(child)) {
                    GameObject sliderObj = GetFromPool(child, _sliderTemplate, _sliderPool);
                    Slider slider = sliderObj.GetComponentInChildren<Slider>();
                    slider.onValueChanged.RemoveAllListeners();
                    slider.onValueChanged.AddListener(v => {
                        _body.SetBlendShapeWeight(child, v);
                    });
                } else {
                    GameObject foldoutObj = GetFromPool(child, _foldoutTemplate, _foldoutPool);
                    Button button = foldoutObj.GetComponentInChildren<Button>();
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => {
                        BuildPage(child);
                    });
                }
            }
        }

        private GameObject GetFromPool(string path, GameObject template, Queue<GameObject> pool) {
            GameObject obj = pool.FirstOrDefault(o => !o.activeSelf);
            if (obj == null) {
                obj = GameObject.Instantiate(template);
                pool.Enqueue(obj);
            }
            obj.SetActive(true);
            obj.transform.SetParent(_contentParent);
            Text text = obj.GetComponentInChildren<Text>();
            text.text = _nameTree.GetName(path);
            return obj;
        }
    }

}