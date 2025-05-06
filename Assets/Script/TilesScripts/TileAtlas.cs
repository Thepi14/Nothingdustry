using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "TileAtlas", menuName = "Tile Atlas")]
public class TileAtlas : ScriptableObject
{
    [Header("Blocks")]
    public BlockClass[] Wall;
    public BlockClass[] Ores;
    [Header("Floor")]
    public FloorClass[] Floors;
    [Header("Plants")]
    public FloorClass[] GroundPlants;
    public BlockClass[] Trees;
}
