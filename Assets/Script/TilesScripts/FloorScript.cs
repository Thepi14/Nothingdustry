using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;
using static WallScript;

public class FloorScript : MonoBehaviour
{
    public FloorClass floor;
    public int side = 0;
    private bool MODE = true;
    public int x, y;
    public Sprite selectedImage;
    private GameObject selectedSprite;
    [SerializeField] private bool _isPlant = false;

    
    public bool isBurning = false;
    public bool isPlant
    {
        get
        {
            return _isPlant;
        }
    }


    void Start()
    {
        
    }
    void Update()
    {

    }
    void OnApplicationQuit()
    {
        MODE = false;
    }
    private void OnDestroy()
    {
        if (MODE == true && EditorApplication.isPlaying)
        {
            if (floor.isPlant)
                GetComponent<PlantScript>().KillPlant();
            TerrainGenerator.floors[x, y, 0].SetActive(true);
            TerrainGenerator.walls[x, y] = null;
        }
    }
    public void SetBlock()
    {
        selectedImage = Resources.Load<Sprite>("Sprites/Tiles/Orders/selected");
        _isPlant = floor.isPlant;

        selectedSprite = new GameObject();
        selectedSprite.AddComponent<SpriteRenderer>();
        selectedSprite.GetComponent<SpriteRenderer>().sprite = selectedImage;
        selectedSprite.name = "Selected_Sprite";
        selectedSprite.transform.parent = transform;
        selectedSprite.transform.position = gameObject.transform.position;
        selectedSprite.GetComponent<SpriteRenderer>().sortingLayerName = "Air";
        selectedSprite.GetComponent<SpriteRenderer>().sortingOrder = 5;
        selectedSprite.SetActive(false);

        //selectEvent = new SelectionBlockEvent();
        //selectEvent.AddListener(Deselect);
        x = Mathf.FloorToInt(gameObject.transform.position.x - 0.5f);
        y = Mathf.FloorToInt(gameObject.transform.position.y - 0.5f);
        //TerrainGenerator.floors[x, y, 0].SetActive(false);
        for (int i = 1; i < TerrainGenerator.floors.GetLength(2); i++)
        {
            if (TerrainGenerator.walls[x, y] != null && TerrainGenerator.floors[x, y, i] != null)
            {
                DestroyImmediate(TerrainGenerator.floors[x, y, 1]);
            }
        }


    }
    private void OnMouseDown()
    {

    }
    public void SetFloor()
    {

    }
}
