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
        for (int x = 0; x < _boardRef.Board.Size.X; x++)
        {
            int y;
            for (y = 0; y < _boardRef.Board.Size.Y / 2; y++)
            {
                for (int z = 0; z < _boardRef.Board.Size.Z; z++)
                {
                    blocks.Add
                    (
                        new BoardBlock()
                        {
                            Point = new Point(x, y, z),
                            BlockType = 1
                        }
                    );
                }
            }

            y = _boardRef.Board.Size.Y / 2;
            for (int z = 0; z < _boardRef.Board.Size.Z; z++)
            {
                blocks.Add
                (
                    new BoardBlock()
                    {
                        Point = new Point(x, y, z),
                        BlockType = (byte)Mathf.RoundToInt(Random.value * _boardTypes)
                    }
                );
            }
        }
        _boardRef.Board.SetBlocks(blocks);
    }

    private void Start()
    {
        _pawn = new Pawn("Test Combatant", _boardRef.Board, _boardRef.Board.Size / 2);
        Combatant combatant = (Combatant)_pawn.AddComponent
        (
            new Combatant()
            {
                BaseStats = new Stats()
                {
                    Health = 10,
                    Attack = 6,
                    Magic = 4,
                    Agility = 7,
                    Stamina = 5
                },
                Health = 10
            }
        );

        
        SkillSet skillSet = new SkillSet();
        _pawn.AddComponent(skillSet);
        for (int i = 0; i < 20; i++)
            skillSet.AddSkill(new WalkSkill(_pawn));

        _boardRef.Board.AddPawn(_pawn);

        _playerController.AddCombatant(combatant);
        Debug.Log(_playerController.CycleForward());
    }

    private void Update()
    {   
        Point offset = new Point();
        if (Input.GetKeyDown(KeyCode.W)) {
            offset = Point.Forward;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            offset = Point.Left;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            offset = Point.Backward;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            offset = Point.Right;
        }

        if (offset != Point.Zero)
        {
            _pawn.PushAction(new MoveAction(_pawn, _pawn.Position, _pawn.Position + offset, true));
            _boardRef.Step();
            _pawnManager.DistributeMessages();
        }
    }
    #endregion MonoBehaviour
}