using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "newtileclass", menuName = "Floor Class")]
public class FloorClass : ScriptableObject
{
    [Header("Properties")]
    public string floorName;
    public Sprite floorSprite;
    public int floorLife = 100;
    public bool isDestructible = false;
    public int priorityRender = 0;
    public int level = 0;
    public bool canGenerateInStone = true;
    public float movementSpeedMultiplier = 1f;
    public bool infamable = false;
    public bool randomizeSpriteSide = false;

    [Header("Plant Properties")]
    public bool isPlant = false;
    public Sprite[] GrowPhases;
    public int maxLifeTime = 10;
    public float baseGrowthSpeedMultiplier = 1f;
    public float temperatureGrowthSpeedMultiplier = 1f;
    public float bestTemperaturePoint = 0.5f;
    public float maxTemperatureGrowthSpeedMultiplier = 1f;
    public float minTemperature = 0f;
    public float maxTemperature = 1f;
    public FloorClass[] floorsItCanGenerateOn;
}
