using UnityEngine;
using System.Collections;

public class Python : MonoBehaviour {

    public LayerMask whatIsGround;
    public Transform edge;
    public Transform front;
    public int facing = 1;
    private float xspeed = 2;

	// Use this for initialization
	void Start () {
        rigidbody2D.velocity = new Vector2(facing * xspeed, 0f);
        if (facing == -1)
            ReverseDirection();
	}

    void Update()
    {
        if (!Physics2D.OverlapPoint(edge.position, whatIsGround) || Physics2D.OverlapPoint(front.position, whatIsGround))
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

    void EnemyDeath()
    {
        Destroy(gameObject);
    }
}
