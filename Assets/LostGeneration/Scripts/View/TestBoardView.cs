using UnityEngine;
using LostGen;

public class TestBoardView : MonoBehaviour {
	public GameObject QuadPrefab;
	public GameObject BlockPrefab;
    public GameObject PawnPrefab;

    private Transform _testPawnXform;

	private Board _board;
    private Pawn _testPawn;
    public bool _isMoving = false;
    private Vector3 _start, _end;
    private float _time = 0f;

    public float Speed = 20f;
    public Vector2 DebugPosition;
    public int DebugActionCount;
    public float DebugHorizontal, DebugVertical;

	public void Awake() {
		int[,] tiles = {
			{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
			{0, 1, 1, 1, 1, 1, 1, 1, 1, 0},
			{0, 1, 1, 0, 0, 0, 0, 0, 1, 0},
			{0, 0, 1, 0, 0, 0, 0, 0, 1, 0},
			{0, 0, 1, 0, 0, 1, 1, 0, 1, 0},
			{0, 0, 1, 1, 1, 1, 1, 1, 1, 0},
			{0, 0, 0, 0, 0, 1, 0, 0, 1, 0},
			{0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
		};

		_board = new Board(tiles);

        _testPawn = new Pawn("erho", _board, Point.One, null, true, true);
        _board.AddPawn(_testPawn);

        _testPawn.Messages += OnPawnMove;
	}

    public void Start() {
        for (int j = 0; j < _board.Height; j++) {
            for (int i = 0; i < _board.Width; i++) {
                int tile = _board.GetTile(i, j);
                Vector3 position = new Vector3((float)i, 0f, (float)-j);
                GameObject instance = null;

                switch (tile) {
                    case Board.FLOOR_TILE:
                        if (QuadPrefab != null)
                            instance = GameObject.Instantiate(QuadPrefab, position, Quaternion.identity) as GameObject;
                        break;
                    case Board.WALL_TILE:
                        if (BlockPrefab != null)
                            instance = GameObject.Instantiate(BlockPrefab, position, Quaternion.identity) as GameObject;
                        break;
                }

                if (instance != null)
                    instance.transform.parent = gameObject.transform;
            }
        }

        GameObject testPawnObj = GameObject.Instantiate(PawnPrefab, new Vector3((float)_testPawn.Position.X, 0f, (float)-_testPawn.Position.Y), Quaternion.identity) as GameObject;
        _testPawnXform = testPawnObj.transform;
        Camera.main.transform.parent = _testPawnXform;
    }

    void Update() {
        if (!_isMoving) {
            float vert = Input.GetAxis("HardVertical");
            float horz = Input.GetAxis("HardHorizontal");

            Point offset = Point.Zero;
            if (vert > 0f) {
                offset += Point.Up;
            } else if (vert < 0f) {
                offset += Point.Down;
            }

            if (horz > 0f) {
                offset += Point.Right;
            } else if (horz < 0f) {
                offset += Point.Left;
            }

            if (!offset.Equals(Point.Zero)) {
                MoveAction move = new MoveAction(_testPawn, _testPawn.Position + offset, true);
                _testPawn.PushAction(move);
                _board.Step();
            }

            DebugHorizontal = horz;
            DebugVertical = vert;
        } else {
            _time += Speed * Time.deltaTime;
            if (_time > 1f) {
                _isMoving = false;
                _time = 0f;
                //_testPawnXform.position = _end;
            } else {
                _testPawnXform.position = Vector3.Slerp(_start, _end, _time);
            }
        }

        DebugPosition.Set((float)_testPawn.Position.X, (float)_testPawn.Position.Y);
        DebugActionCount = _testPawn.ActionCount;
    }

    private void OnPawnMove(object sender, MessageArgs message) {
        MoveAction.Message move = message as MoveAction.Message;
        if (move != null) {
            _start = new Vector3((float)move.From.X, 0f, (float)-move.From.Y);
            _end = new Vector3((float)move.To.X, 0f, (float)-move.To.Y);
            _isMoving = true;
        }
    }
}