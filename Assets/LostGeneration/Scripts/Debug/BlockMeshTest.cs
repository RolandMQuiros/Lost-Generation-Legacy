using UnityEngine;
using LostGen;

public class BlockMeshTest : MonoBehaviour {
	[SerializeField]private BlockMesh _blockMesh;

	// Use this for initialization
	void Start () {
		Debug.Log(_blockMesh);

		_blockMesh.Resize(new Point(4, 3, 4));

		byte[,,] pyramid = new byte[,,] {
			{
				{ 1, 1, 1, 1 },
				{ 1, 1, 1, 1 },
				{ 1, 1, 1, 1 },
				{ 1, 1, 1, 0 }
			},
			{
				{ 0, 0, 0, 0 },
				{ 0, 1, 1, 0 },
				{ 0, 1, 1, 0 },
				{ 0, 0, 0, 0 }
			},
			{
				{ 0, 0, 0, 0 },
				{ 0, 0, 1, 0 },
				{ 0, 0, 0, 0 },
				{ 0, 0, 0, 0 }
			}
		};

		for (int x = 0; x < 4; x++) {
			for (int y = 0; y < 3; y++) {
				for (int z = 0; z < 4; z++) {
					Point point = new Point(x, y, z);
					_blockMesh.SetBlock(point, pyramid[y, z ,x]);
				}
			}
		}
		
		_blockMesh.Build();
	}
	
	void BuildMesh() {		
		for (int x = 0; x < _blockMesh.Size.X; x++) {
			for (int y = 0; y < _blockMesh.Size.Y; y++) {
				for (int z = 0; z < _blockMesh.Size.Z; z++) {

					//if ((y % 2 + x) % 2 == 0 && ((y % 2 + z) + 1) % 2 == 0) {
					// if (x > BlockMesh.Size.X / 2) {
						_blockMesh.SetBlock(new Point(x, y, z), 1);
					//}
				}
			}
		}

		_blockMesh.Build();
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			BuildMesh();
		}
	}
}
