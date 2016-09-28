using System;
using System.Collections;
using UnityEngine;
using LostGen;

public class RangedSkillView : MonoBehaviour {
    public RangedSkill Skill { get; set; }
    public BoardCursor Cursor;
    public BoardGridField Range;
    public BoardGridField AreaOfEffect;

    private BoardView _boardView;
    private Point _rangeOrigin;
    private Point _areaOfEffectOrigin;

    public void Start() {
        _boardView = Cursor.BoardView;
    }

	public void OnEnable () {
	    foreach (Point point in Skill.GetRange()) {
            Range.AddPoint(point);
        }

        foreach (Point point in Skill.GetAreaOfEffect()) {
            AreaOfEffect.AddPoint(point);
        }
	}

    public void Update() {
    }
}
