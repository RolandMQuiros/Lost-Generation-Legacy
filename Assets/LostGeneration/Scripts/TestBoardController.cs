using UnityEngine;
using System.Collections;
using LostGen;


public class TestBoardController : MonoBehaviour {
    private Board _board;
    private BoardController _controller;
    private Combatant _combatant;
    private WalkSkill _walk;

    private LostGen.CharacterController _enemyAI;

    // Use this for initialization
    public void Awake () {
        _board = new Board(new int[,] {
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            {0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            {0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            {0, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            {0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            {0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            {0, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        });

        _controller = GetComponent<BoardController>();
        _controller.Board = _board;
	}

    public void Start() {
        Character chara = _controller.Characters.GetCharacter(1);
        _combatant = chara.CreateCombatant(_board, new Point(1, 14));
        _combatant.Team = new Team(1, 1, 0, 2);
        _walk = _combatant.GetSkill<WalkSkill>();

        chara = _controller.Characters.GetCharacter(2);

        Combatant enemy = chara.CreateCombatant(_board, new Point(18, 1));
        enemy.Team = new Team(2, 2, 0, 1);
        _enemyAI = new LostGen.CharacterController(enemy);

        enemy.AddPawnToView(_combatant);

        _board.AddPawn(_combatant);
        _board.AddPawn(enemy);
    }
	
	// Update is called once per frame
    public void Update () {
        bool move = false;
	    /*if (Input.GetKeyDown(KeyCode.UpArrow)) {
            _walk.SetDestination(_combatant.Position + Point.Up);
            move = true;
        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            _walk.SetDestination(_combatant.Position + Point.Right);
            move = true;
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            _walk.SetDestination(_combatant.Position + Point.Down);
            move = true;
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            _walk.SetDestination(_combatant.Position + Point.Left);
            move = true;
        }*/

        if (Input.GetKeyDown(KeyCode.Space)) {
            _walk.Fire();
            if (!_controller.Step()) {
                _enemyAI.BeginTurn();
            }
        }
    }
}
