using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "newblockclass", menuName = "Block Class")]
public class BlockClass : ScriptableObject
{
    [Header("Properties")]
    public string blockName;
    public Sprite blockSprite;
    public int blockLife = 100;
    public int blockWidth = 1;
    public int blockHeight = 1;
    public bool hasBorder = false;
    public bool randomizeSpriteSide = false;

    [SerializeField] private int _workToBuild = 0;

    public int workToBuild
    {
        get
        {
            if (mineable == true)
                _workToBuild = -1;

            return _workToBuild;
        }
    }

    [SerializeField] private float _speedMultiplier = 1;
    public float speedMultiplier
    {
        get 
        {
            if (passable == false)
                _speedMultiplier = 0;

            return _speedMultiplier; 
        }
        set
        {
            if (passable == false)
                _speedMultiplier = 0;

            _speedMultiplier = value; 
        }
    }

    public bool rotatable = false;
    public bool passable = false;
    public bool isDoor = false;
    public bool infamable = false;
    public bool mineable = false;

    [Header("Plant Properties")]
    public bool isTree = false;
    [SerializeField] private bool _isPlant = false;
    public bool isPlant
    {
        get
        {
            if (isTree == true)
            {
                _isPlant = true;
            }
            return _isPlant;
        }
        set
        {
            if (isTree == true)
                _isPlant = true;
            else
                _isPlant = value;
        }
    }
    public Sprite[] GrowPhases;
    public int maxLifeTime = 10;
    public float baseGrowthSpeedMultiplier = 1f;
    public float minTemperature = 0f;
    public float maxTemperature = 1f;
    public float minMoisture = 0.1f;
    public float maxMoisture = 2f;
    public FloorClass[] floorsItCanGenerateOn;
}
