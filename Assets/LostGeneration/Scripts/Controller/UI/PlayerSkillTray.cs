using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LostGen;

public class PlayerSkillTray : MonoBehaviour {
    public SkillButton SkillButtonPrefab;
    public float ButtonSpacing = 32f;

    public PlayerController PlayerController;
    public RangedSkillController RangedSkillController;
    public DirectionalSkillController DirectionalSkillController;

    public Combatant Combatant {
        get { return _combatant; }
        set {
            if (value != _combatant) {
                _combatant = value;
                FillTray();
            }
        }
    }

    private Combatant _combatant;
    private List<GameObject> _buttonObjects = new List<GameObject>();

    private void FillTray() {
        for (int i = 0; i < _buttonObjects.Count; i++) {
            TrashMan.despawn(_buttonObjects[i]);
        }
        _buttonObjects = new List<GameObject>();

        int buttonCount = 1;
        foreach (ISkill skill in Combatant.GetSkills()) {
            GameObject buttonObj = TrashMan.spawn(SkillButtonPrefab.gameObject);

            buttonObj.transform.parent = transform;
            buttonObj.transform.localPosition = new Vector3(ButtonSpacing * buttonCount++, 0f, 0f);

            SkillButton skillButton = buttonObj.GetComponent<SkillButton>();
            skillButton.PlayerController = PlayerController;
            skillButton.Skill = skill;
            

            if (skill is RangedSkill) {
                skillButton.SkillController = RangedSkillController;
            } else if (skill is DirectionalSkill) {
                skillButton.SkillController = DirectionalSkillController;
            }

            _buttonObjects.Add(buttonObj);
        }

        for (int i = 0; i < _buttonObjects.Count; i++) {
            Button button = _buttonObjects[i].GetComponent<Button>();
            for (int j = 0; j < _buttonObjects.Count; j++) {
                if (i != j) {
                    SkillButton skillButton = _buttonObjects[j].GetComponent<SkillButton>();
                    button.onClick.AddListener(skillButton.OnOtherClick);
                }
            }
        }
    }
}
