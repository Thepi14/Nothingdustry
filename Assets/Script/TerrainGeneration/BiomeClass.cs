using UnityEngine;

[System.Serializable]
public class BiomeClass
{
    [Header("Settings", order = 1)]
    public string biomeName;

    public Color biomeCol;

    [Range(0, 100)]
    public int temperatureMin, temperatureMax;
    [Range(0, 100)]
    public int rainMin, rainMax;

    public TileAtlas tileAtlas;

    [Header("Wall Settings", order = 1)]
    [Range(0, 1)]
    public float caveFrequency = 0.05f;
    [Range(0, 1)]
    public float caveSize = 0.5f;
    [Range(0, 0.1f)]
    public float caveScattering = 0.5f;

    [Header("Floor Settings", order = 2)]
    [Range(0, 1)]
    public float caveFloorFrequency = 0.05f;
    [Range(0, 1)]
    public float caveFloorSize = 0.6f;
    [Range(0, 0.1f)]
    public float caveFloorScattering = 0.5f;

    public Texture2D caveNoiseTexture;
    public Texture2D borderWallTexture;
    [Header("Ore Settings", order = 3)]
    public OreClass[] ores;
    public GroundPlantClass[] groundPlants;
    public TreeClass[] trees;
}
