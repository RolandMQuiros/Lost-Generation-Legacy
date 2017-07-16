using UnityEngine;
using System;

[Serializable]
public class AutoTile {
    private const int _TILE_COUNT = 16;

    [SerializeField]private Sprite _sprite;
    [SerializeField]private int _padding;
    public bool AreNormalsReversed = false;
    private Rect[] _tiles = new Rect[_TILE_COUNT];
    
    public Rect GetRect(int edges) {
        return _tiles[edges];
    }

    public void SetupTiles()
    {   
        if (_sprite != null)
        {
            // Get bounds of the sprite in texture units
            Rect bounds = new Rect(
                _sprite.rect.x / _sprite.texture.width,
                _sprite.rect.y / _sprite.texture.height,
                _sprite.rect.size.x / _sprite.texture.width,
                _sprite.rect.size.y / _sprite.texture.height
            );

            float cellWidth = bounds.width / _TILE_COUNT;
            float cellHeight = bounds.height;
            float paddingX = (float)_padding / _sprite.texture.width;
            float paddingY = (float)_padding / _sprite.texture.height;
            float tileWidth = cellWidth - (paddingX * 2);
            float tileHeight = cellHeight - (paddingX * 2);

            // Get full tiles
            for (int i = 0; i < _TILE_COUNT; i++)
            {
                Rect tile = new Rect(
                    bounds.x + (cellWidth * i) + paddingX,
                    bounds.y + paddingY,
                    tileWidth,
                    tileHeight
                );

                _tiles[i] = tile;
            }
        }
    }
}