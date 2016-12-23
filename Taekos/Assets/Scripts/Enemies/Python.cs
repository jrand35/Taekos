using UnityEngine;
using System.Collections;

/// <summary>
/// Python enemy script
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class Python : MonoBehaviour {

    public LayerMask whatIsGround;      ///< Layer mask for ground detection
    public Transform edge;              ///< Edge detection for turning around
    public Transform front;             ///< front wall detection for turning around
    public int facing = 1;              ///< Facing, 1 for right, -1 for left
    private Animator anim;
    private float xspeed = 2;           ///< Horizontal movement speed
    private bool dead;
    private Collider2D[] childrenColliders;

    /// <summary>
    /// Initialize variables
    /// </summary>
    void Awake()
    {
        dead = false;
    }

	/// <summary>
	/// Initialize more variables, turn around if facing = -1
	/// </summary>
	void Start () {
        childrenColliders = GetComponentsInChildren<Collider2D>();
        anim = GetComponent<Animator>();
        GetComponent<Rigidbody2D>().velocity = new Vector2(facing * xspeed, 0f);
        if (facing == -1)
            ReverseDirection();
	}

    /// <summary>
    /// Edge detection
    /// </summary>
    void Update()
    {
        if (!Physics2D.OverlapPoint(edge.position, whatIsGround) || Physics2D.OverlapPoint(front.position, whatIsGround))
        {
            facing = -facing;
            ReverseDirection();
        }
        anim.SetBool("Dead", dead);
    }

    /// <summary>
    /// Edge detection
    /// </summary>
    void ReverseDirection()
    {
        if (dead)
            return;
        GetComponent<Rigidbody2D>().velocity = new Vector2(facing * xspeed, 0f);
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }

    /// <summary>
    /// Death method,
    /// Fly off screen, turn off collision detection and destroy the GameObject after 2 seconds
    /// </summary>
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
