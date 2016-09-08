using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen {
    //public class AttackNearestEnemyDecision : DecisionNode {
    //    private Board _board;
    //    private Combatant _source;
    //    private int _maxRange;

    //    private Combatant _target;

    //    public AttackNearestEnemyDecision(Board board, Combatant source, int maxRange) {
    //        _board = board;
    //        _source = source;
    //        _maxRange = maxRange;
    //    }

    //    public override bool ArePreconditionsMet(StateOffset state) {
    //        Point myPosition = state.GetStateValue(StateOffset.CombatantKey(_source, "position"), _source.Position);
    //        List<Board.Node> range = new List<Board.Node>(Pathfinder<Board.Node>.FloodFill(new Board.Node(_board, myPosition), -1, _maxRange));

    //        Combatant closestEnemy = null;
    //        int minDistance = Int32.MaxValue;

    //        for (int i = 0; i < range.Count; i++) {
    //            HashSet<Pawn> pawnsAt = _board.PawnsAt(range[i].Point);
    //            foreach (Pawn pawn in pawnsAt) {
    //                Combatant enemy = pawn as Combatant;
    //                if (enemy != null) {
    //                    Point enemyPosition = state.GetStateValue(StateOffset.CombatantKey(enemy, "position"), enemy.Position);
    //                    int distance = Point.Distance(myPosition, enemyPosition);

    //                    if (distance < minDistance) {
    //                        minDistance = distance;
    //                        closestEnemy = enemy;
    //                    }
    //                }
    //            }
    //        }

            
    //    }
    //}
}
