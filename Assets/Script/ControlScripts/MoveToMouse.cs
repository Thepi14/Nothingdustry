using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;
using static TerrainGenerator;
using static MouseScript;

public class MoveToMouse : MonoBehaviour
{
    public float speed = 1f;
    public float rotationSpeed = 0.5f;
    public Vector3 velocity;
    public static List<MoveToMouse> moveableObjects = new List<MoveToMouse>();
    public Rigidbody2D rb;
    public Pathfinding pathfinding;

    private Vector3 target;
    public bool selected;
    private Vector2 movementDirection;
    private float angle;
    private Quaternion toRotation;

    private Vector3 current, previous;

    void Start()
    {
        pathfinding = new Pathfinding(mapWidth, mapHeight);
        moveableObjects.Add(this);
        target = transform.position;
    }
    private void OnValidate()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        /*velocity = (transform.position - previous) / Time.deltaTime;
        previous = transform.position;
        if (velocity.normalized.x + velocity.normalized.y <= 0.5 && transform.position.ConvertTo<Vector2>() != target.ConvertTo<Vector2>())
        {
            Rotate();
        }
        if (Input.GetMouseButtonDown(1) && selected)
        {
            target = MouseToGrid()/*Camera.main.ScreenToWorldPoint(Input.mousePosition);

            target.x += 0.5f;
            target.y += 0.5f;

        }
        if (transform.rotation == toRotation)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }*/
    }
    private void Rotate()
    {
        //angle = Mathf.Atan2(velocity.y, velocity.x);
        angle = Mathf.Atan2(target.y - gameObject.transform.position.y, target.x - gameObject.transform.position.x);
        movementDirection = new Vector2((float)Mathf.Cos(angle), (float)Mathf.Sin(angle));
        toRotation = Quaternion.LookRotation(Vector3.forward, movementDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
    }
    public void OnMouseDown()
    {
        print("selected");
        selected = true;
        gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;

        foreach (MoveToMouse obj in moveableObjects)
        {
            if (obj != this)
            {
                obj.selected = false;
                obj.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }
}
