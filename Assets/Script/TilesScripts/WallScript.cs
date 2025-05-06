using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Events;
using static MouseScript;

public class WallScript : MonoBehaviour, Selectable
{
    public Sprite selectedImage;
    public Rigidbody2D rb;
    private int hitPoint = 1, maxHitPoint = 1;
    public int x, y;
    public BlockClass block;
    public int side = 0;
    public bool isBurning = false;

    [SerializeField] private bool _mineable = false;
    [SerializeField] private bool _passable = false;
    [SerializeField] private bool _isPlant = false;
    [SerializeField] private bool _isTree = false;

    public bool isDoor = false;
    [SerializeField] private bool _isDoorOpened = false;
    public bool isDoorOpened
    {
        get
        {
            if (isDoor == false)
                _isDoorOpened = false;
            return _isDoorOpened;
        }
        set
        {
            if (isDoor == true)
                _isDoorOpened = value;
        }
    }
    public bool isTree
    {
        get => _isTree;
    }
    public bool isPlant
    {
        get
        {
            if (_isTree)
                return true;
            return _isPlant;
        }
    }
    public bool mineable
    {
        get => _mineable;
        //set => _mineable = value;
    }
    public bool passable
    {
        get => _passable;
        //set => _passable = value;
    }

    private bool MODE = true;
    public bool selected = false;
    private List<MoveToMouse> moveableObjects = MoveToMouse.moveableObjects;
    private GameObject selectedSprite;
    public class SelectionBlockEvent : UnityEvent { }
    public class DamageEvent : UnityEvent { }

    public SelectionBlockEvent selectEvent;
    public DamageEvent damageEvent;

    void OnApplicationQuit()
    {
        MODE = false;
    }
    private void OnDestroy()
    {
        if (MODE == true && EditorApplication.isPlaying)
        {
            if (block.hasBorder)
                FindObjectOfType<WallGraphicsUpdater>().wallChanged.Invoke(x, y, true, gameObject);
            if (block.isPlant)
                GetComponent<PlantScript>().KillPlant();
            TerrainGenerator.floors[x, y, 0].SetActive(true);
            TerrainGenerator.walls[x, y] = null;
        }
    }
    private void OnValidate()
    {
        selectedImage = Resources.Load<Sprite>("Sprites/Tiles/Orders/selected");
    }
    void Start()
    {
        selectedImage = Resources.Load<Sprite>("Sprites/Tiles/Orders/selected");
        rb = GetComponent<Rigidbody2D>();
    }
    public void Select()
    {
        //print("selected");
        selected = true;
        selectedSprite.SetActive(true);
    }
    public void Deselect()
    {
        //print("deselected");
        if (selected == true)
        {
            selected = false;
            selectEvent.RemoveListener(Deselect);
            selectedSprite.SetActive(false);
        }
    }
    public void SetMaxLife(int value)
    {
        maxHitPoint = value;
        hitPoint = maxHitPoint;
    }
    public void SetBlock()
    {
        _mineable = block.mineable;
        selectedImage = Resources.Load<Sprite>("Sprites/Tiles/Orders/selected");
        _passable = block.passable;
        _isTree = block.isTree;
        _isPlant = block.isPlant;

        selectedSprite = new GameObject();
        selectedSprite.AddComponent<SpriteRenderer>();
        selectedSprite.GetComponent<SpriteRenderer>().sprite = selectedImage;
        selectedSprite.name = "Selected_Sprite";
        selectedSprite.transform.parent = transform;
        selectedSprite.transform.position = gameObject.transform.position;
        selectedSprite.GetComponent<SpriteRenderer>().sortingLayerName = "Air";
        selectedSprite.GetComponent<SpriteRenderer>().sortingOrder = 5;
        selectedSprite.SetActive(false);

        selectEvent = new SelectionBlockEvent();
        selectEvent.AddListener(Deselect);
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
    public void SetPlant(bool set)
    {
        if (set)
        {

        }
        else
        {

        }
    }
    public void reduceHP(int value)
    {
        if (hitPoint - value > maxHitPoint)
            hitPoint = maxHitPoint;
        else
            hitPoint -= value;
        
        print(hitPoint);

        if (hitPoint < 0)
        {
            Death();
        }
    }
    public void Death()
    {

        Destroy(gameObject);
    }
    void Update()
    {
        
    }
    private void OnMouseDown()
    {
        if (!MouseOverUI())
        {
            MouseScript.selectedList.Add(gameObject);
            Select();
        }
    }
}