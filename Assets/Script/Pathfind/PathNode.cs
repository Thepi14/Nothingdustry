using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private Grid<PathNode> grid;
    public int x, y;

    public int gCost, hCost, fCost;

    public bool isWalkable;
    public PathNode cameFromNode;
    public PathNode(Grid<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        isWalkable = true;
    }
    public void CalculateFCost() => fCost = gCost + hCost;
    public void SetWalkableState(bool value) => isWalkable = value;
}
