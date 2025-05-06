using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TerrainGenerator;
using static MouseScript;

public class UnitBehavior : MonoBehaviour
{
    public float speed = 1f;
    public float rotationSpeed = 0.5f;
    public bool asEndedPath = true;
    public bool unitStarted = false;

    public float timer = 1f;
    private float counter;

    public Rigidbody2D rb;
    public Animator animator;

    public List<PathNode> path;

    private Pathfinding pathfinding;

    private Vector3 target;
    public bool selected;
    private Vector2 movementDirection;
    private float angle;
    private Quaternion toRotation;
    public int pathIndex = 0;

    public void StartUnit()
    {
        pathfinding = new Pathfinding(mapWidth, mapHeight);
        Debug.Log(mapWidth + " " + mapHeight);
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        asEndedPath = true;
        unitStarted = true;
    }
    void Update()
    {
        if (unitStarted)
        {
            if (asEndedPath == true)
                counter += Time.deltaTime;

            if (counter >= timer)
            {
                counter = 0;
                bool encounteredPath = false;
                while (encounteredPath == false)
                {
                    pathfinding.SetGridWalkableNodes();
                    //Vector3 mouseWorldPosition = MouseToGrid();

                    Random.InitState(Random.Range(10000, 10000));
                    target = new Vector3((int)transform.position.x + (int)UnityEngine.Random.Range(-10, 10) + 0.5f, (int)transform.position.y + (int)UnityEngine.Random.Range(-10, 10) + 0.5f, 0);
                    pathfinding.GetGrid().GetXY(target, out int x, out int y);

                    //path = pathfinding.FindPath((int)transform.position.x, (int)transform.position.y, x, y);

                    target = MouseToGrid();

                    path = pathfinding.FindPath((int)transform.position.x, (int)transform.position.y, (int)target.x, (int)target.y);

                    if (path != null)
                    {
                        for (int i = 0; i < path.Count; i++)
                        {
                            if (i + 1 == path.Count)
                                break;
                            Debug.DrawLine(new Vector2(path[i].x + 0.5f, path[i].y + 0.5f), new Vector2(path[i + 1].x + 0.5f, path[i + 1].y + 0.5f), Color.green, 300f, false);
                        }
                        encounteredPath = true;
                    }
                    encounteredPath = true;
                }
                pathIndex = 0;
                asEndedPath = false;
                //path.Reverse();
            }

            if (asEndedPath == false)
            {
                if (path == null)
                {

                    asEndedPath = true;
                    rb.velocity = Vector2.zero;
                }
                if (pathIndex == path.Count)
                {
                    asEndedPath = true;
                    rb.velocity = Vector2.zero;
                }
                if (Vector2.Distance(transform.position, new Vector2(path[pathIndex].x + 0.5f, path[pathIndex].y + 0.5f)) <= 0.05f && !asEndedPath)
                {
                    pathIndex++;
                }
                if (pathIndex == path.Count)
                {
                    asEndedPath = true;
                    rb.velocity = Vector2.zero;
                }
                if (!asEndedPath)
                {
                    target = new Vector2(path[pathIndex].x + 0.5f, path[pathIndex].y + 0.5f);
                    Rotate();
                    if (transform.rotation.z >= toRotation.z - 0.01f && transform.rotation.z <= toRotation.z + 0.01f)
                    {
                        rb.velocity = transform.up * speed;
                    }
                    else rb.velocity = Vector2.zero;
                }
            }
            if (rb.velocity != Vector2.zero || !(transform.rotation.z >= toRotation.z - 0.01f && transform.rotation.z <= toRotation.z + 0.01f))
                animator.speed = 1;
            else
                animator.speed = 0;
        }
    }
    private void Rotate()
    {
        //angle = Mathf.Atan2(velocity.y, velocity.x);
        angle = Mathf.Atan2(target.y - gameObject.transform.position.y, target.x - gameObject.transform.position.x);
        movementDirection = new Vector2((float)Mathf.Cos(angle), (float)Mathf.Sin(angle));
        toRotation = Quaternion.LookRotation(Vector3.forward, movementDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
    }
}
