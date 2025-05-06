using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantScript : MonoBehaviour
{
    public FloorScript floorScript;
    public WallScript wallScript;

    public BlockClass blockTree;
    public FloorClass groundPlant;

    public bool isWall = false;
    public bool isFloor = false;

    public float lifeTime = 0;
    public int maxLifeTime;

    private bool _plantIsSet = false;
    public bool plantIsSet
    {
        get { return _plantIsSet; }
        private set { _plantIsSet = value; }
    }

    public SpriteRenderer spriteRenderer;

    void Start()
    {

    }
    void Update()
    {
        if (plantIsSet)
        {
            lifeTime += Time.deltaTime;
            if (lifeTime <= maxLifeTime / 3)
            {
                spriteRenderer.sprite = isWall ? blockTree.GrowPhases[0] : groundPlant.GrowPhases[0];
            }
            else if (lifeTime <= (maxLifeTime / 3) * 2)
            {
                spriteRenderer.sprite = isWall ? blockTree.GrowPhases[1] : groundPlant.GrowPhases[1];
            }
            else
            {
                spriteRenderer.sprite = isWall ? blockTree.GrowPhases[2] : groundPlant.GrowPhases[2];
            }
            if (lifeTime >= maxLifeTime)
            {
                KillPlant();
            }
            if (isFloor == true)
            {

            }
            else if (isWall == true)
            {

            }
        }
    }
    public void KillPlant()
    {

        Destroy(gameObject);
    }
    public void SetPlant()
    {
        plantIsSet = false;
        floorScript = GetComponent<FloorScript>();
        wallScript = GetComponent<WallScript>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        isFloor = floorScript != null;
        isWall = wallScript != null;

        if (floorScript == null && wallScript == null)
        {
            isFloor = false;
            isWall = false;
            Destroy(this);
            return;
        }
        else if (isFloor && isWall)
        {
            Destroy(this);
            return;
        }
        if (isFloor == true)
        {
            groundPlant = floorScript.floor;
            SetGroundPlant();
        }
        else if (isWall == true)
        {
            blockTree = wallScript.block;
            SetTree();
        }
        plantIsSet = true;
    }
    public void SetGroundPlant()
    {
        maxLifeTime = groundPlant.maxLifeTime;
    }
    public void SetTree()
    {
        maxLifeTime = blockTree.maxLifeTime;
    }
}
