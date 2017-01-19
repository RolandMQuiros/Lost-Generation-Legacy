using UnityEngine;
using LostGen;

/// <summary>
/// Unity Inspector interface for the Board.  Allows us to edit the board through Unity either directly
/// or by some procgen utility.  More importantly, it lets us spread references to the Board through the
/// Editor, instead of having to create classes specifically to assign the reference to components.
/// </summary>
public class BoardData : MonoBehaviour {
    public Board Board; 

    private void Awake() {
        int[,,] grid = new int[,,] {
            {
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1 }
            },
            {
                { 1, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 1 }
            }
        };

        Board = new Board(new Point(grid.GetLength(2), grid.GetLength(0), grid.GetLength(1)));

        for (int y = 0; y < grid.GetLength(0); y++) {
            for (int z = 0; z < grid.GetLength(1); z++) {
                for (int x = 0; x < grid.GetLength(2); x++) {
                    Point point = new Point(x, y, z);
                    if (grid[y,z,x] == 1) {
                        Board.SetBlock(new BoardBlock() { IsSolid = true, IsOpaque = true }, point);
                    } else {
                        Board.SetBlock(new BoardBlock() { IsSolid = false, IsOpaque = false }, point);
                    }
                }
            }
        }
    }
}