using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TerrainGenerator;

public class Pathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 12;

    public static Pathfinding Instance { get; private set; }

    private Grid<PathNode> grid;
    private List<PathNode> openList;
    private HashSet<PathNode> closedList;

    public Pathfinding(int width, int height)
    {
        grid = new Grid<PathNode>(width, height, (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
    }
    public List<PathNode> FindPath(int startx, int starty, int endx, int endy)
    {
        PathNode startNode = grid.GetGridObject(startx, starty);
        PathNode endNode = grid.GetGridObject(endx, endy);

        SetGridWalkableNodes();

        if (endNode != null)
            if (endNode.isWalkable == false)
            {
                List<List<PathNode>> paths = new List<List<PathNode>>();

                if (grid.CheckIfInsideGrid(endx - 1, endy))
                    if (grid.GetGridObject(endx - 1, endy).isWalkable)
                        if (FindPathB(startx, starty, endx - 1, endy) != null)
                            paths.Add(FindPathB(startx, starty, endx - 1, endy));

                if (grid.CheckIfInsideGrid(endx + 1, endy))
                    if (grid.GetGridObject(endx + 1, endy).isWalkable)
                        if (FindPathB(startx, starty, endx + 1, endy) != null)
                            paths.Add(FindPathB(startx, starty, endx + 1, endy));

                if (grid.CheckIfInsideGrid(endx, endy - 1))
                    if (grid.GetGridObject(endx, endy - 1).isWalkable)
                        if (FindPathB(startx, starty, endx, endy - 1) != null)
                            paths.Add(FindPathB(startx, starty, endx, endy - 1));

                if (grid.CheckIfInsideGrid(endx, endy + 1))
                    if (grid.GetGridObject(endx, endy + 1).isWalkable)
                        if (FindPathB(startx, starty, endx, endy + 1) != null)
                            paths.Add(FindPathB(startx, starty, endx, endy + 1));

                bool allPathIsNull = true;
                foreach (var path in paths)
                {
                    if (path != null)
                    {
                        allPathIsNull = false;
                        break;
                    }
                }
                if (allPathIsNull)
                    goto exitNotWalkablePathDefinition;

                List<PathNode> finalPath = paths[0];

                foreach (var path in paths)
                {
                    if (path != null)
                        if (path.Count < finalPath.Count && path.Count > 1)
                        {
                            finalPath = path;
                        }
                }
                return finalPath;
            }
        exitNotWalkablePathDefinition:

        openList = new List<PathNode> { startNode };
        closedList = new HashSet<PathNode>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }
        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                Debug.Log("PathFound");
                return CalculatePath(endNode);
            }
            /*Debug.Log(currentNode.x + " " + currentNode.y);
            GameObject truer = new GameObject("Verif");
            truer.transform.position = new Vector3(currentNode.x + 0.5f, currentNode.y + 0.5f, -2);
            truer.AddComponent<SpriteRenderer>();
            truer.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/Tiles/Orders/wrong") as Sprite;
            truer.GetComponent<SpriteRenderer>().sortingLayerName = "Air";*/
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                //Debug.Log(neighbourNode);
                if (closedList.Contains(neighbourNode)) continue;
                if (!neighbourNode.isWalkable)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                if (currentNode != null && neighbourNode != null) { }
                else
                    continue;
                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                        openList.Add(neighbourNode);
                }
            }
        }

        return null;
    }
    public List<PathNode> FindPathB(int startx, int starty, int endx, int endy)
    {
        PathNode startNode = grid.GetGridObject(startx, starty);
        PathNode endNode = grid.GetGridObject(endx, endy);

        SetGridWalkableNodes();
        openList = new List<PathNode> { startNode };
        closedList = new HashSet<PathNode>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }
        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                Debug.Log("PathFound");
                return CalculatePath(endNode);
            }
            /*Debug.Log(currentNode.x + " " + currentNode.y);
            GameObject truer = new GameObject("Verif");
            truer.transform.position = new Vector3(currentNode.x + 0.5f, currentNode.y + 0.5f, -2);
            truer.AddComponent<SpriteRenderer>();
            truer.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/Tiles/Orders/wrong") as Sprite;
            truer.GetComponent<SpriteRenderer>().sortingLayerName = "Air";*/
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                //Debug.Log(neighbourNode);
                if (closedList.Contains(neighbourNode)) continue;
                if (!neighbourNode.isWalkable)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                if (currentNode != null && neighbourNode != null) { }
                else
                    continue;
                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                        openList.Add(neighbourNode);
                }
            }
        }

        return null;
    }
    public Grid<PathNode> GetGrid()
    {
        return grid;
    }
    public PathNode GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }
    public void SetWalkable(int x, int y, bool value)
    {
        grid.GetGridObject(x, y).SetWalkableState(value);
    }
    public void SetGridWalkableNodes()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (walls[x, y] == null)
                {
                    SetWalkable(x, y, true);
                }
                else if (walls[x, y] != null && walls[x, y].GetComponent<WallScript>().passable == true)
                {
                    SetWalkable(x, y, true);
                }
                else
                    SetWalkable(x, y, false);
            }
        }
    }
    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();
        /*for (int x = -1; x < 1; x++)
        {
            for (int y = -1; y < 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                //Debug.DrawLine(new Vector3 (pathNode.x + x, pathNode.y + y), new Vector3(pathNode.x + x, pathNode.y + y + 0.5f), Color.green, 5f, false);
                if (grid.CheckIfInsideGrid(currentNode.x + x, currentNode.y + y))
                    neighbourList.Add(grid.GetGridObject(currentNode.x + x, currentNode.y + y));
            }
        }*/
        if (currentNode.x - 1 >= 0)
        {
            // Left
            neighbourList.Add(grid.GetGridObject(currentNode.x - 1, currentNode.y));
            if (currentNode.y - 1 >= 0 && 
                grid.GetGridObject(currentNode.x, currentNode.y - 1).isWalkable && 
                grid.GetGridObject(currentNode.x - 1, currentNode.y).isWalkable) neighbourList.Add(grid.GetGridObject(currentNode.x - 1, currentNode.y - 1));
            if (currentNode.y + 1 < grid.GetHeight() && 
                grid.GetGridObject(currentNode.x, currentNode.y + 1).isWalkable && 
                grid.GetGridObject(currentNode.x - 1, currentNode.y).isWalkable) neighbourList.Add(grid.GetGridObject(currentNode.x - 1, currentNode.y + 1));
        }

        if (currentNode.x + 1 < grid.GetWidth()) {
            // Right
            neighbourList.Add(grid.GetGridObject(currentNode.x + 1, currentNode.y));
            if (currentNode.y - 1 >= 0 &&
                grid.GetGridObject(currentNode.x, currentNode.y - 1).isWalkable &&
                grid.GetGridObject(currentNode.x + 1, currentNode.y).isWalkable) neighbourList.Add(grid.GetGridObject(currentNode.x + 1, currentNode.y - 1));
            if (currentNode.y + 1 < grid.GetHeight() &&
                grid.GetGridObject(currentNode.x, currentNode.y + 1).isWalkable &&
                grid.GetGridObject(currentNode.x + 1, currentNode.y).isWalkable) neighbourList.Add(grid.GetGridObject(currentNode.x + 1, currentNode.y + 1));
        }
        // Down
        if (currentNode.y - 1 >= 0) neighbourList.Add(grid.GetGridObject(currentNode.x, currentNode.y - 1));
        // Up
        if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(grid.GetGridObject(currentNode.x, currentNode.y + 1));
        return neighbourList;
    }
    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        if (a != null && b != null)
        {
            int xDistance = Mathf.Abs(a.x - b.x);
            int yDistance = Mathf.Abs(a.y - b.y);
            int remaining = Mathf.Abs(xDistance - yDistance);
            return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST + remaining;
        }
        return 0;
    }
    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList) 
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i ++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }
    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        PathNode currentNode = endNode;
        int x = currentNode.cameFromNode.x - currentNode.x, prevX = currentNode.cameFromNode.x - currentNode.x,
            y = currentNode.cameFromNode.y - currentNode.y, prevY = currentNode.cameFromNode.y - currentNode.y,
            counter = 0;
        path.Add(endNode);
        while (currentNode.cameFromNode != null)
        {
            if (x != prevX || y != prevY)
            {
                path.Add(currentNode.cameFromNode);
            }
            else if (counter >= 0)
            {
                path.Add(currentNode.cameFromNode);
                counter = 0;
            }
            else counter++;
            prevX = x;
            prevY = y;
            currentNode = currentNode.cameFromNode;
            if (currentNode.cameFromNode == null) break;
            x = currentNode.cameFromNode.x - currentNode.x;
            y = currentNode.cameFromNode.y - currentNode.y;
            //Debug.Log(prevX + " x " + x + " --- " + prevY + " y " + y);
        }
        path.Reverse();
        return path;
    }
}
