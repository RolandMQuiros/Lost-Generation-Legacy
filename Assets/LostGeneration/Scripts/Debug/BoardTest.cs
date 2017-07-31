using System.Collections.Generic;
using UnityEngine;
using LostGen;
public class BoardTest : MonoBehaviour
{   
    public Point BoardSize;
    [SerializeField]private PawnManager _pawnManager;
    [SerializeField]private PlayerCombatantController _playerController;
    [SerializeField]private int _boardTypes = 1;
    private BoardRef _boardRef;
    private Pawn _pawn;

    #region MonoBehaviour
    private void Awake()
    {
        _boardRef = GetComponent<BoardRef>();
        _boardRef.Board = new Board(new Point(BoardSize.X, BoardSize.Y, BoardSize.Z));

        List<BoardBlock> blocks = new List<BoardBlock>();
        int y = 0;
        for (int x = 0; x < _boardRef.Board.Size.X; x++)
        {
            for (y = 0; y < _boardRef.Board.Size.Y / 2; y++)
            {
                for (int z = 0; z < _boardRef.Board.Size.Z; z++)
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

            y = _boardRef.Board.Size.Y / 2;
            for (int z = 0; z < _boardRef.Board.Size.Z; z++)
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

        _boardRef.Board.SetBlocks(blocks);
    }

    private void Start() {
        Pawn combatant1 = MakeCombatant(new Pawn("Test Combatant 1", _boardRef.Board, _boardRef.Board.Size / 2 + Point.Right));
        Pawn combatant2 = MakeCombatant(new Pawn("Test Combatant 2", _boardRef.Board, _boardRef.Board.Size / 2 + Point.Left));

        _boardRef.Board.AddPawn(combatant1);
        _boardRef.Board.AddPawn(combatant2);

        _playerController.AddCombatant(combatant1.GetComponent<Combatant>());
        _playerController.AddCombatant(combatant2.GetComponent<Combatant>());
        _playerController.CycleForward();
    }

    private void Update() {   
        if (Input.GetKeyDown(KeyCode.Space)) {
            _boardRef.Step();
            _pawnManager.DistributeMessages();   
        }   
    }
    #endregion MonoBehaviour

    private Pawn MakeCombatant(Pawn pawn) {
        pawn.IsSolid = true;

        pawn.AddComponent(new Health(10));
        pawn.AddComponent(new PawnStats(
            new Stats() {
                Health = 10,
                Attack = 6,
                Magic = 4,
                Agility = 7,
                Stamina = 5
            }
        ));
        pawn.AddComponent(new ActionPoints(5));

        Combatant combatant = (Combatant)pawn.AddComponent(new Combatant());
        
        SkillSet skillSet = new SkillSet();
        pawn.AddComponent(skillSet);
        for (int i = 0; i < 5; i++) {
            skillSet.AddSkill(new WalkSkill(pawn));
        }

        for (int i = 0; i < 5; i++) {
            skillSet.AddSkill(new MeleeAttackSkill(pawn, new Point[] { Point.Right, Point.Right * 2}));
        }

        pawn.AddComponent(new Timeline());

        return pawn;
    }
}