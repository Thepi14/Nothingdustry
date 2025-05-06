using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Teste")]
    public float InputAcceleration = 1;
    
    public float maxRotationSpeed = 100;

    public float velocityDrag = 1;

    private Vector2 direction;

    public float maxSpeed;
    public float Acceleration;

    private float zRotationVelocity;

    public Rigidbody2D rb;

    private void Update()
    {
        //x,y
        direction = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
        direction = direction.normalized;
    }

    private void FixedUpdate()
    {
        Move();
        Rotate();
        print(rb.velocity.normalized.magnitude);
    }
    private void Move()
    {
        //x
        if (rb.velocity.x < maxSpeed * direction.x && direction.x > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x + Acceleration, rb.velocity.y);
        }
        else if (rb.velocity.x > 0 && direction.x == 0)//
        {
            rb.velocity = new Vector2(rb.velocity.x - Acceleration, rb.velocity.y);
        }
        //-x
        if (rb.velocity.x > maxSpeed * direction.x && direction.x < 0)
        {
            rb.velocity = new Vector2(rb.velocity.x - Acceleration, rb.velocity.y);
        }
        else if (rb.velocity.x < 0 && direction.x == 0)//
        {
            rb.velocity = new Vector2(rb.velocity.x + Acceleration, rb.velocity.y);
        }
        //y
        if (rb.velocity.y < maxSpeed * direction.y && direction.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + Acceleration);
        }
        else if (rb.velocity.y > 0 && direction.y == 0)//
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - Acceleration);
        }
        //-y
        if (rb.velocity.y > maxSpeed * direction.y && direction.y < 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - Acceleration);
        }
        else if (rb.velocity.y < 0 && direction.y == 0)//
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + Acceleration);
        }
        //0
        if (direction.x == 0 && ((rb.velocity.x > 0 && rb.velocity.x <= Acceleration) || (rb.velocity.x < 0 && rb.velocity.x >= -Acceleration)))
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        if (direction.y == 0 && ((rb.velocity.y > 0 && rb.velocity.y <= Acceleration) || (rb.velocity.y < 0 && rb.velocity.y >= -Acceleration)))
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
    }
    private void Rotate()
    {
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0 && rb.velocity.magnitude < maxSpeed / 7 /*|| Vector3.Dot(transform.forward, rb.velocity)*/)
        {

        }
        else
        {
            Vector2 v = rb.velocity;
            float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}