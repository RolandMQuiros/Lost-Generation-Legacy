using System;
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

    public void Update() {
        if (IsTargeting) {
            if (Skill.InRange(Cursor.BoardPoint) && Skill.Target != Cursor.BoardPoint) {
                Skill.Target = Cursor.BoardPoint;
                BoardGridField.AddPoints(Skill.GetRange(), RangeSprite);
                BoardGridField.AddPoints(Skill.GetAreaOfEffect(), AreaOfEffectSprite);

                if (Cursor.TapUp) {
                    IsTargeting = false;
                }
            }
        }
    }
}
