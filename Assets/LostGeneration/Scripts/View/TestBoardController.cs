using UnityEngine;
using System.Collections;
using LostGen;

public class TestBoardController : MonoBehaviour {
    private Board _board;
    private BoardView _boardView;

    private TestCharacterManager _characters = new TestCharacterManager();

    private Combatant _combatant;
    private WalkSkill _walk;

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

        _boardView = GetComponent<BoardView>();
        _boardView.Characters = _characters;
	}

    public void Start() {
        _boardView.AttachBoard(_board);

        Character chara = _characters.GetCharacter(1);
        _combatant = chara.CreateCombatant(_board, new Point(1, 14));
        _walk = _combatant.GetSkill<WalkSkill>();

        _board.AddPawn(_combatant);
    }
	
	// Update is called once per frame
    public void Update () {
	    if (Input.GetKeyDown(KeyCode.UpArrow)) {
            _walk.SetDestination(_combatant.Position + Point.Up);
            Debug.Log("Destination: " + _walk.Destination);
        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            _walk.SetDestination(_combatant.Position + Point.Right);
            Debug.Log("Destination: " + _walk.Destination);
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            _walk.SetDestination(_combatant.Position + Point.Down);
            Debug.Log("Destination: " + _walk.Destination);
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            _walk.SetDestination(_combatant.Position + Point.Left);
            Debug.Log("Destination: " + _walk.Destination);
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            _walk.Fire();
            _boardView.Step();
            Debug.Log("Move, dammit");
        }
    }
}
