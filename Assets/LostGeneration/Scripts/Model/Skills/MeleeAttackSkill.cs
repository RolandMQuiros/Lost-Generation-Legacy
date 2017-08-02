using System;
using System.Linq;
using System.Collections.Generic;

namespace LostGen {
    public class MeleeAttackSkill : DirectionalSkill {
        private List<Point> _areaOfEffect;
        private Dictionary<CardinalDirection, Point[]> _transforms = new Dictionary<CardinalDirection, Point[]>();
        private HashSet<Point> _fullAreaOfEffect = new HashSet<Point>();
        private int _actionPoints = 0;

        public bool PierceSolids { get; set; }
        public bool PierceWalls { get; set; }
        public override int ActionPoints{ get { return _actionPoints; } }

        /// <summary>
        /// Contruct a new MeleeAttackSkill.
        /// </summary>
        /// <param name="attacker">Reference to Attacking Combatant</param>
        /// <param name="areaOfEffect">
        /// Collection of Point offsets indicating which tiles around the attacker are affected by the attack.
        /// These offsets are rotated based on this skill's Direction attribute, and are defined based on the
        /// attacker facing east.
        /// </param>
        public MeleeAttackSkill(Pawn attacker, int actionPoints, IEnumerable<Point> areaOfEffect = null)
        : base(attacker, "Melee Attack", "Attack an adjacent space") {
            
            _actionPoints = actionPoints;

            if (areaOfEffect == null) {
                _areaOfEffect = new List<Point>();
            } else {
                _areaOfEffect = new List<Point>(areaOfEffect);
            }

            for (CardinalDirection d = CardinalDirection.East; d < CardinalDirection.Count; d++) {
                _transforms[d] = new Point[_areaOfEffect.Count];
            }

            for (int i = 0; i < _areaOfEffect.Count; i++) {
                Point east = _areaOfEffect[i];
                Point south = new Point(-east.Z,east.Y, -east.X);
                Point west = new Point(-east.X, east.Y, -east.Z);
                Point north = new Point(east.Z, east.Y, east.X);

                _transforms[CardinalDirection.East][i] = east;
                _transforms[CardinalDirection.South][i] = south;
                _transforms[CardinalDirection.West][i] = west;
                _transforms[CardinalDirection.North][i] = north;

                _fullAreaOfEffect.Add(east);
                _fullAreaOfEffect.Add(south);
                _fullAreaOfEffect.Add(west);
                _fullAreaOfEffect.Add(north);
            }
        }

        public override IEnumerable<Point> GetAreaOfEffect() {
            return _transforms[Direction].Select(point => point + Pawn.Position);
        }

        public bool InAreaOfEffect(Point target) {
            bool found = false;

            if (_fullAreaOfEffect.Contains(target - Pawn.Position)) {
                for (int i = 0; !found && i < _transforms[Direction].Length; i++) {
                    Point aoePoint = _transforms[Direction][i];
                    if (aoePoint.Equals(Pawn.Position - target)) {
                        if (PierceWalls && PierceSolids) {
                            found = true;
                        } else {
                            found = Pawn.Board.LineCast(Pawn.Position, aoePoint, null, PierceWalls, PierceSolids);
                        }
                    }
                }
            }

            return found;
        }

        public bool InFullAreaOfEffect(Point point) {
            Point offset = point - Pawn.Position;
            return _fullAreaOfEffect.Contains(offset);
        }

        public bool CanAttackFrom(Point from, Point point) {
            bool canAttack = false;
            bool inFullAOE = _fullAreaOfEffect.Contains(point - from);

            if (Pawn.Board.Blocks.InBounds(from)) {
                if (PierceWalls && PierceSolids) {
                    canAttack = inFullAOE;
                } else if (inFullAOE) {
                    canAttack = !Pawn.Board.LineCast(from, point, null, PierceWalls, PierceSolids);
                }
            }

            return canAttack;
        }

        public override PawnAction Fire() {
            AttackAction attack = new AttackAction(Pawn, _transforms[Direction]);
            return attack;
        }
    }
}
