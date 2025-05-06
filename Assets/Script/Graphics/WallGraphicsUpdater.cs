using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static TerrainGenerator;

public interface Selectable
{
    public void Deselect();
    public void Select();
}

public class WallGraphicsUpdater : MonoBehaviour
{
    public GameObject Wall;
    public TerrainGenerator WallScript;
    public GameObject[] BlocksList;
    public List<GameObject> BorderList;

    public Texture2D BorderTexture;
    public Texture2D InacessibleLightTexture;
    public Texture2D LightningTexture;
    public Texture2D WallBlockLightTexture;

    public Sprite[] borderSprites = new Sprite[46];
    public Sprite darknessSprite;

    public GameObject[,] border;
    public GameObject[,] darkness;
    public GameObject DestroyerObject;
    public GameObject DarknessObj;

    public UnityEvent wallChangedSignal;

    public int mapHeight, mapWidth;

    private float counter;
    public float timer = 10f;

    void Start()
    {
        
    }
    private void OnValidate()
    {

    }
    void Update()
    {
        counter += Time.deltaTime;
        if (counter > timer)
        {
            print("update.");
            counter = 0;
            CheckBorderMap();
        }
    }
    [System.Serializable]
    public class BlockChangedEvent : UnityEvent<int, int, bool, GameObject> { }

    public BlockChangedEvent wallChanged;

    public void TilesIlumination()
    {
        WallBlockLightTexture = TextureFunction.GetTexture(BorderTexture);
        for (int x = 0; x < WallBlockLightTexture.width; x++)
        {
            for (int y = 0; y < WallBlockLightTexture.height; y++)
            {
                if (WallBlockLightTexture.GetPixel(x, y) != Color.white && WallBlockLightTexture.GetPixel(x, y) != Color.black)
                {
                    WallBlockLightTexture.SetPixel(x, y, Color.white);
                }
            }
        }
        WallBlockLightTexture.Apply();
        TextureFunction.InverseWhiteBlack(WallBlockLightTexture);
        LightningTexture = TextureFunction.GetTexture(WallBlockLightTexture);
        InacessibleLightTexture = TextureFunction.GetTexture(BorderTexture);
        TextureFunction.OtherColorsToTwo(InacessibleLightTexture, false);
        TextureFunction.InverseWhiteBlack(InacessibleLightTexture);
        //TextureFunction.MeioEntre(1440, 960);
        for (int x = 0; x < WallBlockLightTexture.width; x++)
        {
            for (int y = 0; y < WallBlockLightTexture.height; y++)
            {
                if (InacessibleLightTexture.GetPixel(x, y) == Color.white)
                    continue;
                else
                {
                    GameObject darkSprite = new("Darkness");
                    darkSprite.AddComponent<SpriteRenderer>();
                    darkSprite.GetComponent<SpriteRenderer>().sprite = darknessSprite;
                    darkSprite.GetComponent<SpriteRenderer>().sortingLayerName = "Ground";
                    darkSprite.GetComponent<SpriteRenderer>().sortingOrder = -1;
                    darkSprite.transform.parent = DarknessObj.transform;
                    darkSprite.transform.position = walls[x, y].transform.position;
                    darkness[x, y] = darkSprite;
                }
            }
        }
    }
    public void CheckBorderMap()
    {
        for (int x = 0; x < BorderTexture.width; x++)
        {
            for (int y = 0; y < BorderTexture.height; y++)
            {
                if (walls[x,y] == null && border[x,y] != null) 
                {
                    DestroyImmediate(border[x, y]);
                }
            }
        }
    }
    public void RunBorder()
    {
        WallScript = Wall.GetComponent<TerrainGenerator>();
        mapWidth = TerrainGenerator.mapWidth;
        mapHeight = TerrainGenerator.mapHeight;
        BorderTexture = FindObjectOfType<TerrainGenerator>().ReturnBorderTexture();

        InacessibleLightTexture = new Texture2D(mapWidth, mapHeight);
        LightningTexture = new Texture2D(mapWidth, mapHeight);
        WallBlockLightTexture = new Texture2D(mapWidth, mapHeight);

        wallChanged ??= new BlockChangedEvent();

        darkness = new GameObject[mapWidth, mapHeight];
        darknessSprite = Resources.Load<Sprite>("Sprites/Tiles/Black");

        wallChanged.AddListener(UpdateBorderTextures);
        Wall.GetComponent<TerrainGenerator>();
        TilesIlumination();
        BorderList = FindObjectOfType<TerrainGenerator>().ReturnBorder();
        border = new GameObject[BorderTexture.width, BorderTexture.height];
        DrawBorderTextures();
    }
    public void UpdateBorderTextures(int ix, int iy, bool destroyed, GameObject wallSignal)
    {
        void RemoveDarkness(int x, int y)
        {
            if (!CheckArrayLimits(x, y) && darkness[x, y] != null)
            {
                DestroyImmediate(darkness[x, y].gameObject);
                darkness[x, y] = null;
            }
        }

        void ChangeTileAt(int x, int y)
        {
            bool EsqSup() => CheckArrayLimits(x - 1, y + 1) ? false : walls[x - 1, y + 1] != null ? walls[x - 1, y + 1].GetComponent<WallScript>().block.mineable ? false : true : true;
            bool Sup() => CheckArrayLimits(x, y + 1) ? false : walls[x, y + 1] != null ? walls[x, y + 1].GetComponent<WallScript>().block.mineable ? false : true : true;
            bool DirSup() => CheckArrayLimits(x + 1, y + 1) ? false : walls[x + 1, y + 1] != null ? walls[x + 1, y + 1].GetComponent<WallScript>().block.mineable ? false : true : true;
            bool Esq() => CheckArrayLimits(x - 1, y) ? false : walls[x - 1, y] != null ? walls[x - 1, y].GetComponent<WallScript>().block.mineable ? false : true : true;
            bool Dir() => CheckArrayLimits(x + 1, y) ? false : walls[x + 1, y] != null ? walls[x + 1, y].GetComponent<WallScript>().block.mineable ? false : true : true;
            bool EsqInf() => CheckArrayLimits(x - 1, y - 1) ? false : walls[x - 1, y - 1] != null ? walls[x - 1, y - 1].GetComponent<WallScript>().block.mineable ? false : true : true;
            bool Inf() => CheckArrayLimits(x, y - 1) ? false : walls[x, y - 1] != null ? walls[x, y - 1].GetComponent<WallScript>().block.mineable ? false : true : true;
            bool DirInf() => CheckArrayLimits(x + 1, y - 1) ? false : walls[x + 1, y - 1] != null ? walls[x + 1, y - 1].GetComponent<WallScript>().block.mineable ? false : true : true;
            //4 sides
            if (Sup() && Inf() && Dir() && Esq())
                PlaceTile(borderSprites[15], x, y);
            //4 points
            else if (EsqSup() && DirSup() && EsqInf() && DirInf() && !Sup() && !Inf() && !Dir() && !Esq())
                PlaceTile(borderSprites[0], x, y);
            //3 sides
            else if (Inf() && Dir() && Esq())
                PlaceTile(borderSprites[16], x, y);
            else if (Sup() && Dir() && Esq())
                PlaceTile(borderSprites[17], x, y);
            else if (Sup() && Inf() && Esq())
                PlaceTile(borderSprites[23], x, y);
            else if (Sup() && Inf() && Dir())
                PlaceTile(borderSprites[24], x, y);
            //1 point 2 sides
            else if (Sup() && Dir() && EsqInf())
                PlaceTile(borderSprites[30], x, y);
            else if (Sup() && Esq() && DirInf())
                PlaceTile(borderSprites[31], x, y);
            else if (Inf() && Esq() && DirSup())
                PlaceTile(borderSprites[32], x, y);
            else if (Inf() && Dir() && EsqSup())
                PlaceTile(borderSprites[33], x, y);
            //2 sides
            else if (Sup() && Esq())
                PlaceTile(borderSprites[18], x, y);
            else if (Inf() && Esq())
                PlaceTile(borderSprites[19], x, y);
            else if (Sup() && Dir())
                PlaceTile(borderSprites[20], x, y);
            else if (Inf() && Dir())
                PlaceTile(borderSprites[21], x, y);
            else if (Inf() && Sup())
                PlaceTile(borderSprites[25], x, y);
            else if (Esq() && Dir())
                PlaceTile(borderSprites[22], x, y);
            //2 points 1 side
            else if (((EsqSup() && Sup()) || Sup() || (DirSup() && Sup())) && DirInf() && EsqInf())//sup
            { PlaceTile(borderSprites[34], x, y); print("There's a T in X: " + x + " Y: " + y); }
            else if (((EsqInf() && Inf()) || Inf() || (DirInf() && Inf())) && DirSup() && EsqSup())//inf
            { PlaceTile(borderSprites[39], x, y); print("There's a T in X: " + x + " Y: " + y); }
            else if (((EsqInf() && Esq()) || Esq() || (EsqSup() && Esq())) && DirSup() && DirInf())//esq
            { PlaceTile(borderSprites[45], x, y); print("There's a T in X: " + x + " Y: " + y); }
            else if (((DirInf() && Dir()) || Dir() || (DirSup() && Dir())) && EsqSup() && EsqInf())//dir
            { PlaceTile(borderSprites[42], x, y); print("There's a T in X: " + x + " Y: " + y); }
            //1 point 1 side
            else if (((EsqSup() && Sup()) || Sup() || (DirSup() && Sup())) && DirInf())//sup
                PlaceTile(borderSprites[36], x, y);
            else if (((EsqSup() && Sup()) || Sup() || (DirSup() && Sup())) && EsqInf())//sup
                PlaceTile(borderSprites[35], x, y);
            else if (((EsqInf() && Inf()) || Inf() || (DirInf() && Inf())) && DirSup())//inf
                PlaceTile(borderSprites[41], x, y);
            else if (((EsqInf() && Inf()) || Inf() || (DirInf() && Inf())) && EsqSup())//inf
                PlaceTile(borderSprites[40], x, y);
            else if (((EsqInf() && Esq()) || Esq() || (EsqSup() && Esq())) && DirSup())//esq
                PlaceTile(borderSprites[38], x, y);
            else if (((EsqInf() && Esq()) || Esq() || (EsqSup() && Esq())) && DirInf())//esq
                PlaceTile(borderSprites[37], x, y);
            else if (((DirInf() && Dir()) || Dir() || (DirSup() && Dir())) && EsqSup())//dir
                PlaceTile(borderSprites[43], x, y);
            else if (((DirInf() && Dir()) || Dir() || (DirSup() && Dir())) && EsqInf())//dir
                PlaceTile(borderSprites[44], x, y);
            //1 side
            else if ((EsqSup() && Sup()) || Sup() || (DirSup() && Sup()))
                PlaceTile(borderSprites[26], x, y);
            else if ((EsqInf() && Inf()) || Inf() || (DirInf() && Inf()))
                PlaceTile(borderSprites[27], x, y);
            else if ((EsqInf() && Esq()) || Esq() || (EsqSup() && Esq()))
                PlaceTile(borderSprites[28], x, y);
            else if ((DirInf() && Dir()) || Dir() || (DirSup() && Dir()))
                PlaceTile(borderSprites[29], x, y);
            //3 points
            else if (DirSup() && EsqInf() && DirInf())
                PlaceTile(borderSprites[1], x, y);
            else if (EsqSup() && EsqInf() && DirInf())
                PlaceTile(borderSprites[2], x, y);
            else if (EsqSup() && DirSup() && EsqInf())
                PlaceTile(borderSprites[3], x, y);
            else if (EsqSup() && DirSup() && DirInf())
                PlaceTile(borderSprites[4], x, y);
            //2 points
            else if (EsqInf() && DirInf())
                PlaceTile(borderSprites[5], x, y);
            else if (EsqSup() && EsqInf())
                PlaceTile(borderSprites[6], x, y);
            else if (DirSup() && DirInf())
                PlaceTile(borderSprites[7], x, y);
            else if (EsqSup() && DirSup())
                PlaceTile(borderSprites[8], x, y);
            else if (EsqSup() && DirInf())
                PlaceTile(borderSprites[13], x, y);
            else if (EsqInf() && DirSup())
                PlaceTile(borderSprites[14], x, y);
            //1 point
            else if (DirInf())
                PlaceTile(borderSprites[9], x, y);
            else if (EsqSup())
                PlaceTile(borderSprites[10], x, y);
            else if (EsqInf())
                PlaceTile(borderSprites[11], x, y);
            else if (DirSup())
                PlaceTile(borderSprites[12], x, y);
        }
        void DeleteOldBorder(int x, int y)
        {
            if (CheckIfInsideArrayLimits(x, y))
                if (border[x, y] != null)
                {
                    DestroyImmediate(border[x, y].gameObject);
                    border[x, y] = null;
                }
        }
        if (destroyed)
        {
            walls[ix,iy] = null;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    DeleteOldBorder(ix + i, iy + j);
                    RemoveDarkness(ix + i, iy + j);
                }
            }
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    int checkX = ix + i, checkY = iy + j;

                    if (checkX >= 0 && checkX < walls.GetLength(0) && checkY >= 0 && checkY < walls.GetLength(1))
                    {
                        if (walls[ix + i, iy + j] != null)
                            ChangeTileAt(ix + i, iy + j);
                    }
                }
            }
        }
        else
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    DeleteOldBorder(ix + i, iy + j);
                }
            }
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    int checkX = ix + i, checkY = iy + j;

                    if (checkX >= 0 && checkX < walls.GetLength(0) && checkY >= 0 && checkY < walls.GetLength(1))
                    {
                        if (walls[ix + i, iy + j] != null)
                            ChangeTileAt(ix + i, iy + j);
                    }
                }
            }
        }
    }
    public void DrawBorderTextures()
    {
        int x, y;
        bool EsqSup() => BorderTexture.GetPixel(x - 1, y + 1) == Color.black;
        bool Sup() => BorderTexture.GetPixel(x, y + 1) == Color.black;
        bool DirSup() => BorderTexture.GetPixel(x + 1, y + 1) == Color.black;
        bool Esq() => BorderTexture.GetPixel(x - 1, y) == Color.black;
        bool Dir() => BorderTexture.GetPixel(x + 1, y) == Color.black;
        bool EsqInf() => BorderTexture.GetPixel(x - 1, y - 1) == Color.black;
        bool Inf() => BorderTexture.GetPixel(x, y - 1) == Color.black;
        bool DirInf() => BorderTexture.GetPixel(x + 1, y - 1) == Color.black;

        for (x = 0; x < BorderTexture.width; x++)
        {
            for (y = 0; y < BorderTexture.height; y++)
            {
                if(BorderTexture.GetPixel(x,y) == Color.green)
                {
                    //4 sides
                    if (Sup() && Inf() && Dir() && Esq())
                        PlaceTile(borderSprites[15], x, y);
                    //4 points
                    else if (EsqSup() && DirSup() && EsqInf() && DirInf() && !Sup() && !Inf() && !Dir() && !Esq())
                        PlaceTile(borderSprites[0], x, y);
                    //3 sides
                    else if (Inf() && Dir() && Esq())
                        PlaceTile(borderSprites[16], x, y);
                    else if (Sup() && Dir() && Esq())
                        PlaceTile(borderSprites[17], x, y);
                    else if (Sup() && Inf() && Esq())
                        PlaceTile(borderSprites[23], x, y);
                    else if (Sup() && Inf() && Dir())
                        PlaceTile(borderSprites[24], x, y);
                    //1 point 2 sides
                    else if (Sup() && Dir() && EsqInf())
                        PlaceTile(borderSprites[30], x, y);
                    else if (Sup() && Esq() && DirInf())
                        PlaceTile(borderSprites[31], x, y);
                    else if (Inf() && Esq() && DirSup())
                        PlaceTile(borderSprites[32], x, y);
                    else if (Inf() && Dir() && EsqSup())
                        PlaceTile(borderSprites[33], x, y);
                    //2 sides
                    else if (Sup() && Esq())
                        PlaceTile(borderSprites[18], x, y);
                    else if (Inf() && Esq())
                        PlaceTile(borderSprites[19], x, y);
                    else if (Sup() && Dir())
                        PlaceTile(borderSprites[20], x, y);
                    else if (Inf() && Dir())
                        PlaceTile(borderSprites[21], x, y);
                    else if (Inf() && Sup())
                        PlaceTile(borderSprites[25], x, y);
                    else if (Esq() && Dir())
                        PlaceTile(borderSprites[22], x, y);
                    //2 points 1 side
                    else if (((EsqSup() && Sup()) || Sup() || (DirSup() && Sup())) && DirInf() && EsqInf())//sup
                        { PlaceTile(borderSprites[34], x, y); print("There's a T in X: " + x + " Y: " + y); }
                    else if (((EsqInf() && Inf()) || Inf() || (DirInf() && Inf())) && DirSup() && EsqSup())//inf
                        { PlaceTile(borderSprites[39], x, y); print("There's a T in X: " + x + " Y: " + y); }
                    else if (((EsqInf() && Esq()) || Esq() || (EsqSup() && Esq())) && DirSup() && DirInf())//esq
                        { PlaceTile(borderSprites[45], x, y); print("There's a T in X: " + x + " Y: " + y); }
                    else if (((DirInf() && Dir()) || Dir() || (DirSup() && Dir())) && EsqSup() && EsqInf())//dir
                        { PlaceTile(borderSprites[42], x, y); print("There's a T in X: " + x + " Y: " + y); }
                    //1 point 1 side
                    else if (((EsqSup() && Sup()) || Sup() || (DirSup() && Sup())) && DirInf())//sup
                        PlaceTile(borderSprites[36], x, y);
                    else if (((EsqSup() && Sup()) || Sup() || (DirSup() && Sup())) && EsqInf())//sup
                        PlaceTile(borderSprites[35], x, y);
                    else if (((EsqInf() && Inf()) || Inf() || (DirInf() && Inf())) && DirSup())//inf
                        PlaceTile(borderSprites[41], x, y);
                    else if (((EsqInf() && Inf()) || Inf() || (DirInf() && Inf())) && EsqSup())//inf
                        PlaceTile(borderSprites[40], x, y);
                    else if (((EsqInf() && Esq()) || Esq() || (EsqSup() && Esq())) && DirSup())//esq
                        PlaceTile(borderSprites[38], x, y);
                    else if (((EsqInf() && Esq()) || Esq() || (EsqSup() && Esq())) && DirInf())//esq
                        PlaceTile(borderSprites[37], x, y);
                    else if (((DirInf() && Dir()) || Dir() || (DirSup() && Dir())) && EsqSup())//dir
                        PlaceTile(borderSprites[43], x, y);
                    else if (((DirInf() && Dir()) || Dir() || (DirSup() && Dir())) && EsqInf())//dir
                        PlaceTile(borderSprites[44], x, y);
                    //1 side
                    else if ((EsqSup() && Sup()) || Sup() || (DirSup() && Sup()))
                        PlaceTile(borderSprites[26], x, y);
                    else if ((EsqInf() && Inf()) || Inf() || (DirInf() && Inf()))
                        PlaceTile(borderSprites[27], x, y);
                    else if ((EsqInf() && Esq()) || Esq() || (EsqSup() && Esq()))
                        PlaceTile(borderSprites[28], x, y);
                    else if ((DirInf() && Dir()) || Dir() || (DirSup() && Dir()))
                        PlaceTile(borderSprites[29], x, y);
                    //3 points
                    else if (DirSup() && EsqInf() && DirInf())
                        PlaceTile(borderSprites[1], x, y);
                    else if (EsqSup() && EsqInf() && DirInf())
                        PlaceTile(borderSprites[2], x, y);
                    else if (EsqSup() && DirSup() && EsqInf())
                        PlaceTile(borderSprites[3], x, y);
                    else if (EsqSup() && DirSup() && DirInf())
                        PlaceTile(borderSprites[4], x, y);
                    //2 points
                    else if (EsqInf() && DirInf())
                        PlaceTile(borderSprites[5], x, y);
                    else if (EsqSup() && EsqInf())
                        PlaceTile(borderSprites[6], x, y);
                    else if (DirSup() && DirInf())
                        PlaceTile(borderSprites[7], x, y);
                    else if (EsqSup() && DirSup())
                        PlaceTile(borderSprites[8], x, y);
                    else if (EsqSup() && DirInf())
                        PlaceTile(borderSprites[13], x, y);
                    else if (EsqInf() && DirSup())
                        PlaceTile(borderSprites[14], x, y);
                    //1 point
                    else if (DirInf())
                        PlaceTile(borderSprites[9], x, y);
                    else if (EsqSup())
                        PlaceTile(borderSprites[10], x, y);
                    else if (EsqInf())
                        PlaceTile(borderSprites[11], x, y);
                    else if (DirSup())
                        PlaceTile(borderSprites[12], x, y);
                }
            }
        }
    }
    public void PlaceTile(Sprite tileSprite, float x, float y)
    {
        GameObject newTile = new();
        newTile.transform.parent = gameObject.transform;
        newTile.AddComponent<SpriteRenderer>();
        newTile.GetComponent<SpriteRenderer>().sprite = tileSprite;
        newTile.GetComponent<SpriteRenderer>().sortingLayerName = "Wall";
        newTile.GetComponent<SpriteRenderer>().sortingOrder = 5;
        newTile.name = "BorderWallSprite";
        newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);
        newTile.layer = 7;
        border[Mathf.FloorToInt(x), Mathf.FloorToInt(y)] = newTile;
    }
}