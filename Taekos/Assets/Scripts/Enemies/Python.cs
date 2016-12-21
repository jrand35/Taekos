using UnityEngine;
using System.Collections;

public class Python : MonoBehaviour {

    public LayerMask whatIsGround;
    public Transform edge;
    public Transform front;
    public int facing = 1;
    private Animator anim;
    private float xspeed = 2;
    private bool dead;
    private Collider2D[] childrenColliders;

    void Awake()
    {
        dead = false;
    }

	// Use this for initialization
	void Start () {
        childrenColliders = GetComponentsInChildren<Collider2D>();
        anim = GetComponent<Animator>();
        GetComponent<Rigidbody2D>().velocity = new Vector2(facing * xspeed, 0f);
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
        anim.SetBool("Dead", dead);
    }

    void ReverseDirection()
    {
        if (dead)
            return;
        GetComponent<Rigidbody2D>().velocity = new Vector2(facing * xspeed, 0f);
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }

    void EnemyDeath()
    {
        StopAllCoroutines();
        dead = true;
        foreach (Collider2D c in childrenColliders)
        {
            c.enabled = false;
        }
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = new Vector2(-5f * facing, 10f);
        GetComponent<Rigidbody2D>().gravityScale = 1f;
        GetComponent<Rigidbody2D>().isKinematic = false;
        Destroy(gameObject, 2f);
    }
}
