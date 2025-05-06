using System;
using System.Collections.Generic;
using UnityEngine;
using static TextureFunction;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Tile Atlas", order = 0)]
    [Range(100000, 999999)]
    public int seed;
    private int lastSeed, badSeed;
    private Texture2D badApple;
    private Texture2D trueBadApple;
    [SerializeField] private bool badAppleEnabled = false;
    public Grid grid;
    
    [Header("Biomes", order = 1)]
    public BiomeClass[] biomes;
    public float biomeFrequency;
    public Gradient biomeGradient;
    public Gradient temperatureGradient;
    public Gradient rainGradient;

    public int temperature;
    public int rain;

    public Texture2D biomeMap;
    public Texture2D temperatureMap;
    public Texture2D rainMap;

    [Header("Configurations", order = 2)]
    public int worldHeight;
    public int worldWidth;
    public static int mapHeight = 0;
    public static int mapWidth = 0;
    public GameObject Floor;
    [Range(0, 1)]
    public float caveFrequency = 0.05f;
    [Range(0, 1)]
    public float caveSize = 0.5f;
    [Range(0, 1)]
    public float caveScattering = 0.5f;
    public Texture2D caveNoiseTexture;
    public Texture2D borderLineTexture;
    public GameObject Player;
    public TileAtlas tileAtlas;

    [Header("Ore Settings", order = 3)]
    //public OreClass[] ores;

    [Header("World Elements", order = 4)]
    public List<GameObject> borderBlocks;
    public static GameObject[,] walls;
    public static GameObject[,,] floors;
    private Color[] biomeCols;

    private void Start()
    {
        trueBadApple = Resources.Load<Texture2D>("Sprites/BadApple");
        badApple = new Texture2D(trueBadApple.width, trueBadApple.height);
        badApple = GetTexture(trueBadApple);
        PolarizeGray(badApple, 0.5f);
        InverseWhiteBlack(badApple);

        if (badAppleEnabled)
        {
            Debug.Log("Bad Apple enabled");
            worldHeight = 150;
            worldWidth = 150;
            mapWidth = 150;
            mapHeight = 150;
            badSeed = 666568;
            UnityEngine.Random.InitState(badSeed);
        }
        else
        {
            UnityEngine.Random.InitState(seed);
        }
        //GenerateNoiseTexture(seed, biomeFrequency,  temperatureMap)

        mapWidth = worldWidth;
        mapHeight = worldHeight;

        biomeCols = new Color[biomes.Length];
        walls = new GameObject[mapWidth, mapHeight];
        floors = new GameObject[mapWidth, mapHeight, 5];

        temperatureMap = new Texture2D(mapWidth, mapHeight);
        rainMap = new Texture2D(mapWidth, mapHeight);

        caveNoiseTexture = new Texture2D(mapWidth, mapHeight);
        borderLineTexture = new Texture2D(mapWidth, mapHeight);

        for (int e = 0; e < biomes.Length; e++)
        {
            biomes[e].biomeCol.a = 1;
            biomeCols[e] = biomes[e].biomeCol;
        }
        /*for (int e = 0; e < ores.Length; e++)
        {
            ores[e].spreadTexture = new Texture2D(mapWidth, mapHeight);
            GenerateNoiseTexture(seed + e, ores[e].rarity, ores[e].size, ores[e].scattering, ores[e].spreadTexture);
        }*/

        DrawTextures();
        borderLineTexture = GetTexture(caveNoiseTexture);
        DrawBorderTexture(borderLineTexture, caveNoiseTexture);
        GenerateFloor();
        //start all gameobjects with tag "Unit"
        StartUnits();
    }
    public void StartUnits()
    {
        GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
        for (int i = 0;  i < units.Length; i++)
        {
            units[i].GetComponent<UnitBehavior>().StartUnit();
        }
    }
    private void OnValidate()
    {
        
    }
    public BiomeClass GetCurrentBiome(int x, int y)
    {        
        if(System.Array.IndexOf(biomeCols, biomeMap.GetPixel(x, y)) > 0)
            return biomes[System.Array.IndexOf(biomeCols, biomeMap.GetPixel(x, y))];

        return biomes[0];
    }
    public BiomeClass GetCurrentBiomeB(int x, int y)
    {
        for (int i = 0; i < biomes.Length; i++)
        {
            if (biomeMap.GetPixel(x, y) == biomes[i].biomeCol)
                return biomes[i];
        }
        return biomes[0];
    }
    public List<GameObject> ReturnBorder()
    {
        return borderBlocks;
    }
    public Texture2D ReturnBorderTexture()
    {
        return borderLineTexture;
    }
    public GameObject PixelToRealObject(float x, float y)
    {
        Collider2D[] List = Physics2D.OverlapCircleAll(new Vector2(x + 0.5f, y + 0.5f), 1f);
        return List[0].gameObject;
    }
    public static bool CheckArrayLimits(int ex, int ey) => ex >= walls.GetLength(0) || ey >= walls.GetLength(1) || ex < 0 || ey < 0;
    public static bool CheckIfInsideArrayLimits(int ex, int ey) => ex >= 0 && ey >= 0 && ex < walls.GetLength(0) && ey < walls.GetLength(1);

    void RemoveDarkness(int x, int y)
    {
        if (!CheckArrayLimits(x, y) && walls[x, y] != null)
        {
            walls[x, y].GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
    public void BiomesNoisesToPrincipal()
    {
        for (int x = 0; x < biomeMap.width; x++)//L
        {
            for (int y = 0; y < biomeMap.height; y++)
            {
                caveNoiseTexture.SetPixel(x, y, GetCurrentBiomeB(x, y).caveNoiseTexture.GetPixel(x, y));
            }
        }
        caveNoiseTexture.Apply();
    }
    public void DrawBiomeTexture()
    {
        for (int x = 0; x < biomeMap.width; x++)
        {
            for (int y = 0; y < biomeMap.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * biomeFrequency, (y + seed) * biomeFrequency);
                Color col1 = temperatureGradient.Evaluate(v);
                temperatureMap.SetPixel(x, y, col1);
                v = Mathf.PerlinNoise((x - seed) * biomeFrequency, (y - seed) * biomeFrequency);
                Color col2 = rainGradient.Evaluate(v);
                rainMap.SetPixel(x, y, col2);
            }
        }
        temperatureMap.Apply();
        rainMap.Apply();
        for (int x = 0; x < biomeMap.width; x++)
        {
            for (int y = 0; y < biomeMap.height; y++)
            {
                //
                //biomeMap.SetPixel(x, y, biomeCols[0]);
                for (int i = 0; i < biomes.Length; i++)
                {
                    int temp = Mathf.FloorToInt(temperatureMap.GetPixel(x, y).r * 100);
                    int rain = Mathf.FloorToInt(rainMap.GetPixel(x, y).r * 100);
                    Debug.Log(temp + " " + rain);
                    if (temp >= biomes[i].temperatureMin && 
                        temp <= biomes[i].temperatureMax && 
                        rain >= biomes[i].rainMin && 
                        rain <= biomes[i].rainMax)
                    {
                        biomeMap.SetPixel(x, y, biomes[i].biomeCol);
                        break;
                    }
                }
            }
        }
        biomeMap.Apply();
        if (!badAppleEnabled)
            BiomesNoisesToPrincipal();
        else
            caveNoiseTexture = GetTexture(badApple);
    }
    public void DrawTextures()
    {
        biomeMap = new Texture2D(mapWidth, mapHeight);

        for (int i = 0; i < biomes.Length; i++)
        {
            biomes[i].caveNoiseTexture = new Texture2D(mapWidth, mapHeight);
            biomes[i].borderWallTexture = new Texture2D(mapWidth, mapHeight);
            //ores
            for (int o = 0; o < biomes[i].ores.Length; o++)
            {
                biomes[i].ores[o].spreadTexture = new Texture2D(mapWidth, mapHeight);
            }
            //ores
            for (int i2 = 0; i2 < biomes[i].ores.Length; i2++)
            {
                GenerateNoiseTexture(seed + i2 + 1, biomes[i].ores[i2].rarity, biomes[i].ores[i2].size, biomes[i].ores[i2].scattering, biomes[i].ores[i2].spreadTexture);
            }
            for (int o = 0; o < biomes[i].groundPlants.Length; o++)
            {
                biomes[i].groundPlants[o].spreadTexture = new Texture2D(mapWidth, mapHeight);
                GenerateNoiseTexture(seed, biomes[i].groundPlants[o].rarity, biomes[i].groundPlants[o].size, biomes[i].groundPlants[o].scattering, biomes[i].groundPlants[o].spreadTexture);
            }
            for (int o = 0; o < biomes[i].trees.Length; o++)
            {
                biomes[i].trees[o].spreadTexture = new Texture2D(mapWidth, mapHeight);
                GenerateNoiseTexture(seed, biomes[i].trees[o].rarity, biomes[i].trees[o].size, biomes[i].trees[o].scattering, biomes[i].trees[o].spreadTexture);
            }
            if (!badAppleEnabled)
            {
                GenerateNoiseTexture(seed, biomes[i].caveFrequency, biomes[i].caveSize, biomes[i].caveScattering, biomes[i].caveNoiseTexture);
                GenerateNoiseTexture(seed, biomes[i].caveFloorFrequency, biomes[i].caveFloorSize, biomes[i].caveFloorScattering, biomes[i].borderWallTexture);
            }
            else
            {
                biomes[i].caveNoiseTexture = GetTexture(badApple);
                biomes[i].borderWallTexture = GetTexture(badApple);
                ExpandWhiteArea(biomes[i].borderWallTexture);
            }
        }
        //GenerateNoiseTexture(seed, caveFrequency, caveSize, caveScattering, caveNoiseTexture);
        DrawBiomeTexture();
    }
    public void GenerateTerrain()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (walls[x, y] != null)
                    continue;
                if (caveNoiseTexture.GetPixel(x, y).r < 0.5f)
                    continue;
                BlockClass blockSprite = null;
                for (int i = 0; i < GetCurrentBiome(x, y).tileAtlas.Ores.Length; i++)
                {
                    if (GetCurrentBiome(x, y).ores[i].spreadTexture.GetPixel(x, y).r > 0.5f)
                    {
                        blockSprite = GetCurrentBiome(x, y).tileAtlas.Ores[i];
                        break;
                    }
                    else
                    {
                        blockSprite = GetCurrentBiome(x, y).tileAtlas.Wall[0];
                    }
                }
                if (blockSprite != null)
                    PlaceWall(blockSprite, x, y, gameObject);
            }
        }

        FindObjectOfType<WallGraphicsUpdater>().RunBorder();
        GenerateTrees();
    }
    public void GenerateTrees()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (walls[x, y] != null)
                    continue;
                BlockClass blockSprite = null;
                for (int i = 0; i < GetCurrentBiome(x, y).tileAtlas.Trees.Length; i++)
                {
                    if (GetCurrentBiome(x, y).tileAtlas.Trees[i].isTree == true && GetCurrentBiome(x, y).trees[i].spreadTexture.GetPixel(x, y).r > 0.5f)
                    {
                        for (int w = 0; w < GetCurrentBiome(x, y).tileAtlas.GroundPlants[i].floorsItCanGenerateOn.Length; w++)
                        {
                            if (GetCurrentBiome(x, y).tileAtlas.Trees[i].floorsItCanGenerateOn[w] == floors[x, y, 0].GetComponent<FloorScript>().floor)
                            {
                                blockSprite = GetCurrentBiome(x, y).tileAtlas.Trees[i];
                                break;
                            }
                        }
                    }
                }
                if (blockSprite != null)
                    PlaceWall(blockSprite, x, y, gameObject);
            }
        }
        GenerateFloorPlants();
    }
    public void GenerateFloorPlants()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (walls[x, y] != null)
                    continue;
                FloorClass floorSprite = null;
                //get the floorclass in the list
                for (int i = 0; i < GetCurrentBiome(x, y).tileAtlas.GroundPlants.Length; i++)
                {
                    if (GetCurrentBiome(x, y).groundPlants[i].spreadTexture.GetPixel(x, y).r > 0.5f)
                        continue;
                    //get the floors it can generate on
                    for (int w = 0; w < GetCurrentBiome(x, y).tileAtlas.GroundPlants[i].floorsItCanGenerateOn.Length; w++)
                    {
                        if (GetCurrentBiome(x, y).tileAtlas.GroundPlants[i].floorsItCanGenerateOn[w] == floors[x, y, 0].GetComponent<FloorScript>().floor)
                        {
                            floorSprite = GetCurrentBiome(x, y).tileAtlas.GroundPlants[i];
                            break;
                        }
                    }
                }
                if (floorSprite != null)
                    PlaceFloor(floorSprite, x, y, Floor);
            }
        }
    }
    public void GenerateFloor()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                FloorClass floorSprite = null;

                if(GetCurrentBiome(x, y).borderWallTexture.GetPixel(x, y) == Color.white && GetCurrentBiome(x, y).tileAtlas.Floors.Length > 1)
                    floorSprite = GetCurrentBiome(x, y).tileAtlas.Floors[1];
                else
                    floorSprite = GetCurrentBiome(x, y).tileAtlas.Floors[0];

                if (floorSprite != null)
                    PlaceFloor(floorSprite, x, y, Floor);
                else
                {
                    floorSprite = GetCurrentBiome(x, y).tileAtlas.Floors[0];
                    PlaceFloor(floorSprite, x, y, Floor);
                }

                if (GetCurrentBiome(x, y).tileAtlas.Floors.Length > 2)
                {
                    floorSprite = GetCurrentBiome(x, y).tileAtlas.Floors[2];

                    if (GetCurrentBiome(x, y).tileAtlas.Floors[2].canGenerateInStone == false && GetCurrentBiome(x, y).borderWallTexture.GetPixel(x, y) == Color.white)
                        continue;

                    if (floorSprite != null)
                    {
                        if (floorSprite.isPlant == true)
                        {
                            for (int z = 0; z < 2; z++)
                                for (int w = 0; w < floorSprite.floorsItCanGenerateOn.Length; w++)
                                    if (floors[x, y, z].GetComponent<FloorScript>().floor == floorSprite.floorsItCanGenerateOn[w])
                                        PlaceFloor(floorSprite, x, y, Floor);
                        }
                        else
                        {
                            PlaceFloor(floorSprite, x, y, Floor);
                        }
                    }
                }
            }
        }
        GenerateTerrain();
    }
    public void GenerateNoiseTexture(float seed, float frequency, float limit, float scattering, Texture2D noiseTexture)
    {
        //noiseTexture = new Texture2D(mapWidth, mapHeight);
        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency);
                if (v + UnityEngine.Random.Range(-scattering, scattering) > limit)
                    noiseTexture.SetPixel(x, y, Color.white);
                else
                    noiseTexture.SetPixel(x, y, Color.black);
            }
        }
        noiseTexture.Apply();
    }
    public void DrawBorderTexture(Texture2D noiseTexture, Texture2D comparative)
    {
        void SetBorder(int ix, int iy)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;

                    int checkX = ix + i;
                    int checkY = iy + j;

                    if (comparative.GetPixel(checkX, checkY).r > 0.5 && checkX >= 0 && checkX < walls.GetLength(0) && checkY >= 0 && checkY < walls.GetLength(1))
                    {
                        noiseTexture.SetPixel(checkX, checkY, Color.green);
                    }
                    else
                        continue;
                }
            }
        }
        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                if (comparative.GetPixel(x, y).r > 0.5)
                    continue;
                SetBorder(x, y);
            }
        }
        noiseTexture.Apply();
    }
    public void PlaceWall(BlockClass block, float x, float y, GameObject parent)
    {
        GameObject newTile = new();
        newTile.transform.parent = parent.transform;

        newTile.AddComponent<SpriteRenderer>();
        newTile.GetComponent<SpriteRenderer>().sprite = block.blockSprite;
        newTile.GetComponent<SpriteRenderer>().sortingLayerName = "Wall";
        newTile.AddComponent<BoxCollider2D>();
        newTile.GetComponent<BoxCollider2D>().size = Vector2.one;
        newTile.AddComponent<Rigidbody2D>();
        newTile.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        newTile.name = block.blockName;
        newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);
        newTile.layer = 9;

        if (block.randomizeSpriteSide)
        {
            int side = UnityEngine.Random.Range(0, 3);
            newTile.transform.rotation = new Quaternion(0, 0, side, 0);
        }

        newTile.AddComponent<WallScript>();
        newTile.GetComponent<WallScript>().block = block;
        newTile.GetComponent<WallScript>().SetBlock();
        newTile.GetComponent<WallScript>().SetMaxLife(block.blockLife);

        if (block.isPlant)
        {
            newTile.AddComponent<PlantScript>();
            int l = UnityEngine.Random.Range(0, block.maxLifeTime);
            newTile.GetComponent<PlantScript>().lifeTime = l;
            newTile.GetComponent<PlantScript>().SetPlant();
        }

        if (block.randomizeSpriteSide)
        {
            int side = UnityEngine.Random.Range(0, 3);
            newTile.transform.eulerAngles = new Vector3(0, 0, side * 90);
            newTile.GetComponent<WallScript>().side = side;
        }

        walls[Mathf.FloorToInt(x), Mathf.FloorToInt(y)] = newTile;

        if (borderLineTexture.GetPixel(Mathf.RoundToInt(x), Mathf.RoundToInt(y)) == Color.green)
        {
            borderBlocks.Add(newTile);
        }
    }
    public void PlaceFloor(FloorClass floor, float x, float y, GameObject parent)
    {
        GameObject newTile = new();
        newTile.transform.parent = parent.transform;

        newTile.AddComponent<SpriteRenderer>();
        newTile.GetComponent<SpriteRenderer>().sprite = floor.floorSprite;
        newTile.GetComponent<SpriteRenderer>().sortingLayerName = "Floor";
        newTile.GetComponent<SpriteRenderer>().sortingOrder = floor.priorityRender;
        newTile.AddComponent<BoxCollider2D>();
        newTile.GetComponent<BoxCollider2D>().size = Vector2.one;
        newTile.AddComponent<FloorScript>();
        newTile.GetComponent<FloorScript>().floor = floor;
        newTile.GetComponent<FloorScript>().SetFloor();

        if (floor.isPlant)
        {
            newTile.AddComponent<PlantScript>();
            int l = UnityEngine.Random.Range(0, floor.maxLifeTime);
            newTile.GetComponent<PlantScript>().lifeTime = l;
            newTile.GetComponent<PlantScript>().SetPlant();
        }

        if (floor.randomizeSpriteSide)
        {
            int side = UnityEngine.Random.Range(0, 3);
            newTile.transform.eulerAngles = new Vector3 (0, 0, side * 90);
            newTile.GetComponent<FloorScript>().side = side;
        }

        newTile.name = floor.floorName;
        newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);
        newTile.layer = 8;

        floors[Mathf.FloorToInt(x), Mathf.FloorToInt(y), floor.level] = newTile;
    }
}