using UnityEngine;
using LostGen;

public class BlockMeshTest : MonoBehaviour {
	public BlockMesh BlockMesh;

	// Use this for initialization
	void Start () {
		Debug.Log(BlockMesh);

		BlockMesh.Resize(new Point(4, 3, 4));

		int[,,] pyramid = new int[,,] {
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
					BlockMesh.SetBlock(point, pyramid[y, z ,x]);
				}
			}
		}
		
		// BlockMesh.Build();
		StartCoroutine(BlockMesh.Build());
	}
	
	void BuildMesh() {
		BlockMesh.Resize();
		
		for (int x = 0; x < BlockMesh.Size.X; x++) {
			for (int y = 0; y < BlockMesh.Size.Y; y++) {
				for (int z = 0; z < BlockMesh.Size.Z; z++) {

					//if ((y % 2 + x) % 2 == 0 && ((y % 2 + z) + 1) % 2 == 0) {
					// if (x > BlockMesh.Size.X / 2) {
						BlockMesh.SetBlock(new Point(x, y, z), 1);
					//}
				}
			}
		}

		BlockMesh.Build();
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			BuildMesh();
		}
	}
}
