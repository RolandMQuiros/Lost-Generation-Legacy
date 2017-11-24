using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AutoTile { 
    private const int _MINITILE_COUNT = 5;
    private const int _TILESET_WIDTH = 2;
    private const int _TILESET_HEIGHT = 3;
    private const int _QUADRANT_COUNT = 4;
    private const int _MASK = 7;

    // Contains the offsets of each slice within the tile set, by quadrant.
    private static readonly Vector2[,] _QUADRANT_OFFSETS = {
        // Upper-right quadrant
        {
            new Vector2(3f, 3f),
            new Vector2(3f, 1f),
            new Vector2(1f, 3f),
            new Vector2(2f, 2f),
            new Vector2(3f, 5f)
        },
        // Lower-right quadrant
        {
            new Vector2(3f, 0f),
            new Vector2(1f, 0f),
            new Vector2(3f, 2f),
            new Vector2(2f, 1f),
            new Vector2(3f, 4f)
        },
        // Lower-left quadrant
        {
            new Vector2(0f, 0f),
            new Vector2(0f, 2f),
            new Vector2(2f, 0f),
            new Vector2(1f, 1f),
            new Vector2(2f, 4f)
        },
        // Upper-left quadrant
        {
            new Vector2(0f, 3f),
            new Vector2(2f, 3f),
            new Vector2(0f, 1f),
            new Vector2(1f, 2f),
            new Vector2(2f, 5f)
        }
    };

    [SerializeField]private Sprite _tileSet;
    [SerializeField]private int _padding;
    [SerializeField]private bool _reverseNormals;

    private Rect[,] _quadrants = new Rect[_QUADRANT_COUNT, _MINITILE_COUNT];

    public void Setup() {
        if (_tileSet == null) {
            //throw new NullReferenceException("No tileset Sprite was defined for this AutoTile");
            return;
        }
        
        Rect spriteRect = new Rect(
            _tileSet.rect.x / _tileSet.texture.width,
            _tileSet.rect.y / _tileSet.texture.height,
            _tileSet.rect.size.x / _tileSet.texture.width,
            _tileSet.rect.size.y / _tileSet.texture.height
        );

        Vector2 padding = new Vector2(
            (float)_padding / _tileSet.texture.width,
            (float)_padding / _tileSet.texture.height
        ); 

        Rect tileRect = new Rect(
            0f, 0f,
            spriteRect.width / _TILESET_WIDTH,
            spriteRect.height / _TILESET_HEIGHT
        );

        Rect slice = new Rect(
            0f, 0f,
            tileRect.width / 2f,
            tileRect.height / 2f
        );

        // Build _quadrants
        for (int q = 0; q < _QUADRANT_COUNT; q++) {
            for (int m = 0; m < _MINITILE_COUNT; m++) {
                Vector2 offset = _QUADRANT_OFFSETS[q, m];
                _quadrants[q, m] = new Rect(
                    padding.x + spriteRect.x + slice.width * offset.x,
                    padding.y + spriteRect.y + slice.height * offset.y,
                    slice.width - (padding.x * 2f),
                    slice.height - (padding.y * 2f)
                );
            }
        }
    }

    public void AddTile(Vector3 position,
                        Vector3 up,
                        Vector3 right,
                        byte adjacency,
                        List<Vector3> vertices,
                        List<int> triangles,
                        List<Vector2> uv) {
        up = 0.25f * up.normalized;
        right = 0.25f * right.normalized;
        
        Vector3[] offsets = new Vector3[] {
             up + right,
            -up + right,
            -up - right,
             up - right
        };
        Vector3[] tilePoints = new Vector3[] {
            position + up - right,
            position + up + right,
            position - up - right,
            position - up + right
        };
        int[] tileTris = new int[] { 0, 1, 2, 1, 3, 2 };
        Rect[] tileRects = GetTiles(adjacency);

        for (int q = 0; q < _QUADRANT_COUNT; q++) {
            if (_reverseNormals) {
                for (int i = tileTris.Length - 1; i >= 0; i--) {
                    triangles.Add(vertices.Count + tileTris[i]);
                }    
            } else {
                for (int i = 0; i < tileTris.Length; i++) {
                    triangles.Add(vertices.Count + tileTris[i]);
                }
            }
    
            for (int i = 0; i < tilePoints.Length; i++) {
                vertices.Add(tilePoints[i] + offsets[q]);
            }

            uv.Add(new Vector2(tileRects[q].x,    tileRects[q].yMax));
			uv.Add(new Vector2(tileRects[q].xMax, tileRects[q].yMax));
			uv.Add(new Vector2(tileRects[q].x,    tileRects[q].y));
			uv.Add(new Vector2(tileRects[q].xMax, tileRects[q].y));
        }
    }

    /// <summary>
    /// Returns four Rects, each containing the UVs for each minitile, based on the adjacency bitstring parameter.
    /// </summary>
    /// <param name="adjacency">
    /// Adjacency bitstring. Each bit, from LSB onward, represents a side or corner of the tile, starting from the top and moving clockwise.
    /// A 1 indicates a like tile is next to this one, 0 otherwise.
    /// </param>
    /// <returns></returns>
    public Rect[] GetTiles(byte adjacency) {
        Rect[] miniTiles = new Rect[_QUADRANT_COUNT];

        // Iterate through quadrants in clockwise direction, starting with upper-right
        for (int q = 0; q < _QUADRANT_COUNT; q++) {
            // Circular right-shift the relevant edges towards the LSB side, then isolate
            // first three bits
            byte sidesAndCorners = (byte)((adjacency >> (2 * q) | adjacency << (8 - 2 * q)) & _MASK);
            
            // Squash the first and third bit in the sequence into two bits
            // This removes the corner from processing, so we can use the resulting byte as
            // an array index
            byte sides = (byte)(((sidesAndCorners >> 1) & 2) | (sidesAndCorners & 1));
            byte miniTileIndex = sides;

            // If both sides are occupied by like tiles, check the corner
            if (sides == 3 /* 0b011 */) {
                // Corner is occupied
                if ((sidesAndCorners & 2) /* 0b0010 */ == 2) {
                    miniTileIndex = 3;
                // Corner is not occupied
                } else {
                    miniTileIndex = 4;
                }
            }

            // Add the minitile to our buffer
            miniTiles[q] = _quadrants[q, miniTileIndex];
        }

        return miniTiles;
    }
}