using System.Collections.Generic;
using UnityEngine;
using LostGen.Model;
using LostGen.Display;

public class BoardTest : MonoBehaviour
{   
    public Point BoardSize;
    [SerializeField]private PawnViewManager _pawnManager;
    [SerializeField]private PlayerPawnController _playerController;
    [SerializeField]private int _boardTypes = 1;
    private BoardRef _boardRef;
    private Pawn _pawn;

    #region MonoBehaviour
    private void Awake()
    {
        _boardRef = GetComponent<BoardRef>();
        _boardRef.Board = new Board(new StandardBlockManager(BoardSize), new BucketPawnManager(BoardSize, 8));

        List<BoardBlock> blocks = new List<BoardBlock>();
        int y = 0;
        for (int x = 0; x < _boardRef.Board.Blocks.Size.X; x++)
        {
            for (y = 0; y < _boardRef.Board.Blocks.Size.Y / 2; y++)
            {
                for (int z = 0; z < _boardRef.Board.Blocks.Size.Z; z++)
                {
                    blocks.Add
                    (
                        new BoardBlock()
                        {
                            Point = new Point(x, y, z),
                            IsSolid = true,
                            IsOpaque = true,
                            IsDiggable = true,
                            BlockType = 1
                        }
                    );
                }
            }

            y = _boardRef.Board.Blocks.Size.Y / 2;
            for (int z = 0; z < _boardRef.Board.Blocks.Size.Z; z++)
            {
                byte blockType = (byte)Mathf.RoundToInt(Random.value * _boardTypes);
                blocks.Add
                (
                    new BoardBlock()
                    {
                        Point = new Point(x, y, z),
                        IsSolid = blockType != 0,
                        IsOpaque = blockType != 0,
                        IsDiggable = blockType != 0,
                        BlockType = blockType
                    }
                );
            }
        }

        _boardRef.Board.Blocks.Set(blocks);
    }

    private void Start() {
        Pawn combatant1 = MakeCombatant(new Pawn("Test Combatant 1", _boardRef.Board, _boardRef.Board.Blocks.Size / 2 + Point.Right), 5);
        combatant1.AddComponent(new LongWalkSkill() { CanWalkDiagonally = true });
        
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
        
        for (int i = 0; i < 5; i++) { pawn.AddComponent(new WalkSkill()); }

        for (int i = 0; i < 5; i++) { pawn.AddComponent(new MeleeAttackSkill(2, new Point[] { Point.Right, Point.Right * 2})); }

        pawn.AddComponent(new Timeline());

        return pawn;
    }
}