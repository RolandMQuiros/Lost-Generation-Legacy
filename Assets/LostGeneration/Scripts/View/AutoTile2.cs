using System;
using UnityEngine;

[Serializable]
public class AutoTile2 { 
    private const int _MINITILE_COUNT = 5;
    private const int _TILESET_WIDTH = 2;
    private const int _TILESET_HEIGHT = 3;
    private const int _QUADRANT_COUNT = 4;
    private const int _MASK = 7;

    private static readonly Vector2[,] _QUADRANT_OFFSETS = {
        // Upper-right quadrant
        {
            new Vector2(3f, 3f),
            new Vector2(3f, 1f),
            new Vector2(1f, 3f),
            new Vector2(1f, 1f),
            new Vector2(3f, 5f)
        },
        // Lower-right quadrant
        {
            new Vector2(3f, 1f),
            new Vector2(1f, 0f),
            new Vector2(3f, 2f),
            new Vector2(1f, 2f),
            new Vector2(3f, 4f)
        },
        // Lower-left quadrant
        {
            new Vector2(0f, 0f),
            new Vector2(0f, 2f),
            new Vector2(2f, 0f),
            new Vector2(2f, 2f),
            new Vector2(2f, 4f)
        },
        // Upper-left quadrant
        {
            new Vector2(0f, 2f),
            new Vector2(2f, 2f),
            new Vector2(0f, 1f),
            new Vector2(2f, 1f),
            new Vector2(2f, 5f)
        }
    };

    [SerializeField]private Sprite _sprite;
    [SerializeField]private int _padding;

    private Rect[,] _quadrants = new Rect[_QUADRANT_COUNT, _MINITILE_COUNT];

    public void Init() {
        if (_sprite == null) {
            throw new NullReferenceException("No Sprite was defined for this AutoTile");
        }
        
        Rect spriteRect = new Rect(
            _sprite.rect.x / _sprite.texture.width,
            _sprite.rect.y / _sprite.texture.height,
            _sprite.rect.size.x / _sprite.texture.width,
            _sprite.rect.size.y / _sprite.texture.height
        );

        // Vector2 padding = new Vector2(
        //     (float)padding / _sprite.texture.Width,
        //     (float)padding / _sprite.texture.Height
        // ); 

        Rect tileRect = new Rect(
            0f, 0f,
            spriteRect.width / 3f,
            spriteRect.height / 4f
        );

        Rect slice = new Rect(
            0f, 0f,
            tileRect.width / 2f,
            tileRect.height /2f
        );

        // Build _quadrants
        for (int q = 0; q < _QUADRANT_COUNT; q++) {
            for (int m = 0; m < _MINITILE_COUNT; m++) {
                Vector2 offset = _QUADRANT_OFFSETS[q, m];
                _quadrants[q, m] = new Rect(
                    slice.width * offset.x,
                    slice.height * offset.y,
                    slice.width,
                    slice.height
                );
            }
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
            byte sides = (byte)((sidesAndCorners >> 1) | (sidesAndCorners & 1));
            byte miniTileIndex = sides;

            // If both sides are occupied by like tiles, check the corner
            if (sides == 3 /* 0b011 */) {
                // Corner is occupied
                if ((sidesAndCorners & 2) /* 0b0010 */ == 1) {
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