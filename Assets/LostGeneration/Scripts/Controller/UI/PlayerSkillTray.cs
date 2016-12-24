﻿using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using LostGen;

public class PlayerSkillTray : MonoBehaviour {
    #region EditorFields
    public SkillButton SkillButtonPrefab;
    public float ButtonSpacing = 32f;
    public SkillEvent SkillToggled; 
    #endregion EditorFields

    public Combatant Combatant {
        get { return _combatant; }
        set { SetCombatant(value); }
    }

    public int Page {
        get { return _page; }
        set { _page = SetPage(value); }
    }

    private const int _SKILLS_PER_PAGE = 9;
    private int _page;
    private Combatant _combatant;
    private Dictionary<Combatant, List<Button>> _buttons = new Dictionary<Combatant, List<Button>>();

    public void AddCombatant(Combatant combatant) {
        List<Button> buttons;
        _buttons.TryGetValue(combatant, out buttons);

        if (buttons == null) {
            buttons = new List<Button>();

            foreach (ISkill skill in combatant.Skills) {
                Button newButton = CreateButton(skill);
                buttons.Add(newButton);
            }

            _buttons[combatant] = buttons;
        } else {
            foreach (ISkill skill in combatant.Skills.Except(buttons.Select(b => b.Skill))) {
                Button newButton = CreateButton(skill);
                buttons.Add(newButton);
            } 
        }

        if (_combatant == null) {
            _combatant = combatant;
            _page = SetPage(0);
        }
    }

    public void RemoveCombatant(Combatant combatant) {
        List<Button> buttons;
        _buttons.TryGetValue(combatant, out buttons);

        if (buttons != null) {
            buttons.ForEach(b => TrashMan.despawn(b.gameObject));
            _buttons.Remove(combatant);
        } else {
            throw new ArgumentException("Combatant " + combatant + " was not handled by this PlayerSkillTray, and cannot be removed.", "combatant");
        }
    }

    public void EnableInteractions() {
        foreach (List<Button> buttons in _buttons.Values) {
            for (int i = 0; i < buttons.Count; i++) {
                buttons[i].interactable = true;
            }
        }
    }

    public void DisableInteractions() {
        foreach (List<Button> buttons in _buttons.Values) {
            for (int i = 0; i < buttons.Count; i++) {
                buttons[i].interactable = false;
            }
        }
    }

    #region PrivateMethods
    private void SetCombatant(Combatant combatant) {
        if (!_buttons.ContainsKey(combatant)) {
            throw new ArgumentException("Combatant " + combatant + " is not handled by this PlayerSkillTray. Add it by calling PlayerSkillTray.AddCombatant.", "combatant");
        }

        _combatant = combatant;
        _page = SetPage(0);
    }

    private Button CreateButton(ISkill skill) {
        GameObject buttonObj = TrashMan.spawn(SkillButtonPrefab.gameObject);
        buttonObj.SetActive(false);
        
        Button button = buttonObj.GetComponent<Button>();
        button.onClick.AddListener(delegate {
            if (SkillToggled != null) {
                SkillToggled.Invoke(skill);
            }
        });

        return button;
    }

    private int SetPage(int page) {
        int newPage = -1;
        int pages = (_combatant.SkillCount / _SKILLS_PER_PAGE) +
                    (_combatant.SkillCount % _SKILLS_PER_PAGE > 0 ? 1 : 0);

        if (page <= pages) {
            newPage = page;

            int start = page * _SKILLS_PER_PAGE;
            int end = Math.Min(_combatant.SkillCount - start, start + _SKILLS_PER_PAGE);
            int offset = 1;

            foreach (List<SkillButton> buttonLists in _buttons.Values) {
                buttonLists.ForEach(b => b.gameObject.SetActive(false));
            }

            List<SkillButton> buttons = _buttons[_combatant];
            for (int i = start; i < end; i++) {
                RectTransform transform = buttons[i].GetComponent<RectTransform>();
                transform.anchoredPosition = new Vector2(offset * ButtonSpacing, 0f);
                buttons[i].gameObject.SetActive(true);
                offset++;
            }
        } 
        
        return newPage;
    }
    #endregion PrivateMethods
}
