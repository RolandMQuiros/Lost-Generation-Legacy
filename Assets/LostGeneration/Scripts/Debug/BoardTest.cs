using System.Collections.Generic;
using UnityEngine;
using LostGen.Model;
using LostGen.Display;

public class BoardTest : MonoBehaviour {
    public Point BoardSize;
    [SerializeField]private PawnViewManager _pawnManager;
    [SerializeField]private PlayerPawnController _playerController;
    [SerializeField]private int _boardTypes = 1;
    [SerializeField]private int _seed = 0;
    private BoardRef _boardRef;
    private Pawn _pawn;

    #region MonoBehaviour
    private void Awake() {
        _boardRef = GetComponent<BoardRef>();
        _boardRef.Board = new Board(new StandardBlockManager(BoardSize), new BucketPawnManager(BoardSize, 8));

        List<BoardBlock> blocks = new List<BoardBlock>();
        System.Random rand = new System.Random(_seed);
        Point upper = _boardRef.Board.Blocks.Size;

        Point.ForEachXYZ(Point.Zero, upper, null, null,
            //(Point Point) => { rand = new System.Random(_seed); },
            (Point point) => {
                if (point.Y < upper.Y / 2) {
                    blocks.Add (
                        new BoardBlock() {
                            Point = point,
                            IsSolid = true,
                            IsOpaque = true,
                            IsDiggable = true,
                            BlockType = 1
                        }
                    );
                }
                // else {
                //     int r = rand.Next() % _boardTypes;
                //     byte blockType = (byte)r;
                //     blocks.Add (
                //         new BoardBlock() {
                //             Point = point,
                //             IsSolid = blockType != 0,
                //             IsOpaque = blockType != 0,
                //             IsDiggable = blockType != 0,
                //             BlockType = blockType
                //         }
                //     );
                // }
            }
        );

        _boardRef.Board.Blocks.Set(blocks);
    }

    private void Start() {
        Pawn combatant1 = MakeCombatant(new Pawn("Test Combatant 1", _boardRef.Board, _boardRef.Board.Blocks.Size / 2 + Point.Right), 5);
        combatant1.AddComponent(new LongWalkSkill() { CanWalkDiagonally = false });
        
        Pawn combatant2 = MakeCombatant(new Pawn("Test Combatant 2", _boardRef.Board, _boardRef.Board.Blocks.Size / 2 + Point.Left), 6);
        combatant2.AddComponent(new LongWalkSkill() { CanWalkDiagonally = false });
        
        _boardRef.Board.Pawns.Add(combatant1);
        _boardRef.Board.Pawns.Add(combatant2);

        _playerController.AddPawn(combatant1);
        _playerController.AddPawn(combatant2);
        _playerController.CycleForward();
    }

    private void Update() {   
        // if (Input.GetKeyDown(KeyCode.Space)) {
        //     _boardRef.Step();
        //     _pawnManager.DistributeMessages();   
        // }   
    }
    #endregion MonoBehaviour

    private Pawn MakeCombatant(Pawn pawn, int agility) {
        pawn.IsSolid = true;
        pawn.AddComponent(new Health(10));
        pawn.AddComponent(new PawnStats(
            new Stats() {
                Health = 10,
                Attack = 6,
                Magic = 4,
                Agility = agility,
                Stamina = 5
            }
        ));
        pawn.AddComponent(new ActionPoints(5));
        pawn.AddComponent(new Combatant());
        pawn.AddComponent(new MeleeAttackSkill(2, new Point[] { Point.Right, Point.Right * 2}));
        pawn.AddComponent(new Timeline());

        return pawn;
    }
}