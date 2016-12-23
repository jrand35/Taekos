using UnityEngine;
using System.Collections;

/// <summary>
/// Piranha enemy class
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class Piranha : MonoBehaviour {
    
    public Sprite frame1;               ///< For moving upward
    public Sprite frame2;               ///< For moving downward
    public int delay = 180;             ///< Delay for jumping up again
    public float jumpSpeed = 25;
    private Vector3 startPosition;
    private SpriteRenderer spr;
    private bool dead;
	
    /// <summary>
    /// Initialize variables and start Run coroutine
    /// </summary>
	void Start () {
        dead = false;
        startPosition = transform.position;
        spr = GetComponent<SpriteRenderer>();
        StartCoroutine(Run());
	}

    /// <summary>
    /// Jump up and down and update the sprite for rising or falling
    /// </summary>
    IEnumerator Run()
    {
        while (true)
        {
            transform.position = startPosition;
            GetComponent<Rigidbody2D>().velocity = new Vector2(0f, jumpSpeed);
            for (int i = 0; i < delay; i++)
            {
                if (GetComponent<Rigidbody2D>().velocity.y > 0)
                {
                    //Rising
                    spr.sprite = frame1;
                }
                else
                {
                    //Falling
                    spr.sprite = frame2;
                }
                yield return 0;
            }
        }
    }

    /// <summary>
    /// Jump away, turn off collision detection, and destroy the GameObject after 2 seconds
    /// </summary>
    void EnemyDeath()
    {
        StopAllCoroutines();
        dead = true;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = new Vector2(-5f, 10f);
        GetComponent<Rigidbody2D>().gravityScale = 1f;
        GetComponent<Rigidbody2D>().isKinematic = false;
        Destroy(gameObject, 2f);
    }
}
