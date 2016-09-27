using System;
using System.Collections.Generic;
using UnityEngine;
using LostGen;

public class DirectionalSkillView : MonoBehaviour {
    public DirectionalSkill Skill;
    public BoardView BoardView;
    public BoardGridField AreaOfEffect;
    public BoardCursor Cursor;

    private CardinalDirection _direction;

    public void Awake() {
        BoardView = BoardView ?? GetComponentInParent<BoardView>();
        Cursor = Cursor ?? BoardView.GetComponentInChildren<BoardCursor>();
    }

    public void Update() {
        CardinalDirection direction = DirectionBetweenPoints(Skill.Owner.Position, Cursor.BoardPoint);

        if (_direction != direction) {
            _direction = direction;
            AreaOfEffect.ClearPoints();
            foreach (Point point in Skill.GetAreaOfEffect(_direction)) {
                AreaOfEffect.AddPoint(point);
            }
        }

        if (Input.GetButtonDown("Click")) {
            Skill.Direction = direction;
        }
    }

    private CardinalDirection DirectionBetweenPoints(Point p1, Point p2) {
        Point offset = p2 - p1;
        float angle = Mathf.Atan2(offset.Y, offset.X) - (Mathf.PI / 4f);
        CardinalDirection direction = (CardinalDirection)((Mathf.PI / 2f) / angle);

        return direction;
    }
}
