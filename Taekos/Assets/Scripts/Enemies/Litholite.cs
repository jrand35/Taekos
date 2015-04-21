using UnityEngine;
using System.Collections;

public class Litholite : MonoBehaviour
{

    public LayerMask whatIsGround;
    public Transform edge;
    public Transform front;
    public bool move;
    public int facing = 1;
    private float xspeed = 2;

    // Use this for initialization
    void Start()
    {
        if (move)
            rigidbody2D.velocity = new Vector2(facing * xspeed, 0f);
        if (facing == -1)
            ReverseDirection();
    }

    void Update()
    {
        if (move && (!Physics2D.OverlapPoint(edge.position, whatIsGround) || Physics2D.OverlapPoint(front.position, whatIsGround)))
        {
            facing = -facing;
            ReverseDirection();
        }
    }

    void ReverseDirection()
    {
        rigidbody2D.velocity = new Vector2(facing * xspeed, 0f);
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }
}
