using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LostGen.Decision {
    public class ApproachMeleeRange : CombatantDecision {
        public override int Cost { get { return _predictedCost; } }
        public Combatant Target {
            get { return _target; }
            set { _target = value; }
        }
        private Combatant _target;
        private Combatant _source;
        private WalkSkill _walk;
        private MeleeAttackSkill _melee;

        private CardinalDirection _direction;
        private Point _destination;
        private int _predictedCost;

        public ApproachMeleeRange(Combatant source) : base(source) {
            _source = (Combatant)source;
            if ((_walk = _source.GetSkill<WalkSkill>()) == null) {
                throw new ArgumentException("source", "Combatant " + source.Name + " doesn't have a Walk skill");
            }
            if ((_melee = _source.GetSkill<MeleeAttackSkill>()) == null) {
                throw new ArgumentException("source", "Combatant " + source.Name + " doesn't have a Melee Attack skill");
            }
        }

        public override StateOffset ApplyPostconditions(StateOffset state) {
            int actionPoints = state.Get(StateKey.AP(_source), _source.ActionPoints);
            state.Set(StateKey.AP(_source), actionPoints - _predictedCost);
            state.Set(StateKey.Position(_source), _destination);

            return state;
        }

        public override bool ArePreconditionsMet(StateOffset state) {
            int actionPoints = state.Get(StateKey.AP(_source), _source.ActionPoints);
            Point position = state.Get(StateKey.Position(_source), _source.Position);

            int predictedCost = Point.TaxicabDistance(position, _destination);

            return actionPoints > predictedCost;
        }

        public override void Run() {
            _walk.Fire();
        }

        public override void Setup() {
            // Find all positions where we could effectively attack the enemy using our melee
            List<KeyValuePair<CardinalDirection, Point>> possibleDestinations = new List<KeyValuePair<CardinalDirection, Point>>();
            for (int i = 0; i < (int)CardinalDirection.Count; i++) {
                CardinalDirection direction = (CardinalDirection)i;

                foreach (Point aoeOffset in _melee.GetAreaOfEffect(direction)) {
                    // For each point in the area of effect, see where we would need to be
                    Point strikePosition = _target.Position - aoeOffset;
                    if (_source.Board.InBounds(strikePosition) && // Make sure new standing position is on the Board
                        _source.Board.GetTile(strikePosition) != Board.WALL_TILE && // and that it's not inside a wall
                        _source.Board.PawnsAt(strikePosition).FirstOrDefault(pawn => pawn.IsCollidable && pawn.IsSolid) == null && // and not inside another solid Pawn
                        _melee.CanAttackFrom(strikePosition, _target.Position)) { // and there's nothing in the way

                        possibleDestinations.Add(new KeyValuePair<CardinalDirection, Point>(direction, strikePosition));
                    }
                }
            }

            if (possibleDestinations.Count > 0) {
                // Pick the closest of the possible positions
                KeyValuePair<CardinalDirection, Point> first = possibleDestinations.OrderBy(point => Point.TaxicabDistance(_source.Position, point.Value))
                                                                                   .First();
                _direction = first.Key;
                _destination = first.Value;

                _walk.SetDestination(_destination);
            }
        }
    }
}
