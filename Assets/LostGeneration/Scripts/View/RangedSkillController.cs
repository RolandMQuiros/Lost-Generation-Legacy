﻿using System;
using System.Collections;
using UnityEngine;
using LostGen;

public class RangedSkillController : MonoBehaviour {
    public RangedSkill Skill { get; set; }
    public BoardCursor Cursor;
    public BoardGridField BoardGridField;
    public Sprite RangeSprite;
    public Sprite AreaOfEffectSprite;
    public bool IsTargeting;

    private bool _isRangeAssigned = false;

    public void Update() {
        if (IsTargeting) {
            if (!_isRangeAssigned) {
                BoardGridField.AddPoints(Skill.GetRange(), RangeSprite);
                _isRangeAssigned = true;
            }

            if (Skill.Target != Cursor.BoardPoint && Skill.InRange(Cursor.BoardPoint)) {
                Skill.Target = Cursor.BoardPoint;
                BoardGridField.RemovePoints(Skill.GetAreaOfEffect());
                BoardGridField.AddPoints(Skill.GetAreaOfEffect(), AreaOfEffectSprite);

                if (Cursor.TapUp) {
                    IsTargeting = false;
                }
            }
        }
    }
}
