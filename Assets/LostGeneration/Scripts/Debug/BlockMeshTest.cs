using UnityEngine;
using LostGen;

public class BlockMeshTest : MonoBehaviour {
	public BlockMesh BlockMesh;

	// Use this for initialization
	void Start () {
		Debug.Log(BlockMesh);
		for (int x = 0; x < BlockMesh.Size.X; x++) {
			for (int y = 0; y < BlockMesh.Size.Y; y++) {
				for (int z = 0; z < BlockMesh.Size.Z; z++) {

					if (x < BlockMesh.Size.X / 2) {
						BlockMesh.SetBlock(new Point(x, y, z), 1);
					}

				}
			}
		}

		BlockMesh.Build();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
