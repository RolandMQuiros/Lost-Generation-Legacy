using System;
using System.Collections;
using UnityEngine;
using LostGen;

public class RangedSkillController : MonoBehaviour {
    public RangedSkill Skill { get; set; }
    public BoardCursor Cursor;
    public BoardGridField Range;
    public BoardGridField AreaOfEffect;

    private Point _oldTarget;

    public void BeginStep() {
        foreach (Point point in Skill.GetRange()) {
            Range.AddPoint(point);
        }
    }

    public void Update() {
        if (Skill.Target != Cursor.BoardPoint && Skill.InRange(Cursor.BoardPoint)) {
            Skill.Target = Cursor.BoardPoint;
            UpdateAreaOfEffect();
        }
    }

    private void UpdateAreaOfEffect() {
        AreaOfEffect.ClearPoints();
        foreach (Point point in Skill.GetAreaOfEffect()) {
            AreaOfEffect.AddPoint(point);
        }
    }
}
