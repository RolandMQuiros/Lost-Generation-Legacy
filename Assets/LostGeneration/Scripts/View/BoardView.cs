using UnityEngine;
using System;
using System.Collections.Generic;
using LostGen;

public class BoardView : MonoBehaviour {
    public BoardTheme Theme;
    public Board Board;

    private const string _TILE_CHILD_NAME = "_tileChild";
    private GameObject _tileChild;

	// Use this for initialization
	public void Awake () {
	}
	
	// Update is called once per frame
	public void Update () {
	
	}

    public void RebuildBoard() {
        if (_tileChild != null && _tileChild.transform.childCount > 0) {
            GameObject.Destroy(_tileChild);
        }

        _tileChild = new GameObject(_TILE_CHILD_NAME);
        _tileChild.transform.SetParent(transform);

        for (int y = 0; y < Board.Height; y++) {
            for (int x = 0; x < Board.Width; x++) {
                GameObject newTile;
                Vector3 position = new Vector3(-x * Theme.TileWidth, 0f, y * Theme.TileHeight);
                switch (Board.GetTile(x, y)) {
                    case Board.FLOOR_TILE:
                        newTile = GameObject.Instantiate(Theme.FloorTile);
                        newTile.transform.SetParent(_tileChild.transform);
                        newTile.transform.position = position;
                        break;
                    case Board.WALL_TILE:
                        newTile = Theme.WallTile.GetTile(Board, new Point(x, y), Board.WALL_TILE);
                        newTile.transform.SetParent(_tileChild.transform);
                        newTile.transform.position = position;
                        break;
                }
            }
        }

        //CombineMeshes();
    }

    private void CombineMeshes() {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        if (meshFilters.Length > 0) {
            List<CombineInstance> combine = new List<CombineInstance>();
            for (int i = 0; i < meshFilters.Length; i++) {
                if (meshFilters[i].gameObject != gameObject && meshFilters[i].sharedMesh != null) {
                    CombineInstance instance = new CombineInstance();
                    instance.mesh = meshFilters[i].sharedMesh;
                    instance.transform = meshFilters[i].transform.localToWorldMatrix;
                    meshFilters[i].gameObject.SetActive(false);

                    combine.Add(instance);
                }
            }

            MeshFilter filter = GetComponent<MeshFilter>();
            filter.mesh = new Mesh();
            filter.mesh.CombineMeshes(combine.ToArray(), true, true);
            gameObject.SetActive(true);
        }
    }
}
