using UnityEngine;
using System.Collections;

/// <summary>
/// Litholite (robotic crab) enemy,
/// Moves left and right, or not at all, and shoots lasers at Taekos when he is in the line of sight,
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class Litholite : MonoBehaviour
{
    public GameObject shot;             ///< Laser prefab
    public Transform shotPos;
    public LayerMask whatIsGround;      ///< For wall detection
    public Transform edge;              ///< Detection for turning around at the edge of a platform
    public Transform front;             ///< Detection for turning around in front of a wall
    public SpriteRenderer blinkSprite;  ///< Allow the Litholite to blink blue just before shooting
    public bool move;                   ///< Enable the Litholite to move or not
    public int facing = 1;              ///< 1 for right, -1 for left
    public int startDelay;              ///< Delay in frames before the Litholite starts shooting
    private Animator anim;
    private int postBlink = 10;         ///< Delay in frames after the Litholite blinks to start shooting
    private int blinkFrames = 5;        ///< Duration of the blink
    private int waitFrames = 70;        ///< Delay between shots
    private float xspeed = 2;
    private float shootSpeed = 12f;
    private bool attacking;
    private bool dead;
    private IEnumerator shooting;

    /// <summary>
    /// Create a reference to the Shooting coroutine
    /// </summary>
    void Awake()
    {
        shooting = Shooting();
        attacking = false;
        dead = false;
    }

    /// <summary>
    /// Start moving and possibly turn around
    /// </summary>
    void Start()
    {
        anim = GetComponent<Animator>();
        if (move)
            GetComponent<Rigidbody2D>().velocity = new Vector2(facing * xspeed, 0f);
        if (facing == -1)
            ReverseDirection();
    }

    /// <summary>
    /// Detect Taekos and shoot at him
    /// </summary>
    void Update()
    {
        if (!attacking)
        {
            StopCoroutine("Shooting");
            if (blinkSprite != null)
            {
                blinkSprite.enabled = false;
            }
        }
        if (move && (!Physics2D.OverlapPoint(edge.position, whatIsGround) || Physics2D.OverlapPoint(front.position, whatIsGround)))
        {
            facing = -facing;
            ReverseDirection();
        }
        anim.SetBool("Attacking", attacking);
        anim.SetBool("Dead", dead);
    }

    /// <summary>
    /// Shoot lasers at Taekos with delays between each shot
    /// </summary>
    IEnumerator Shooting()
    {
        while (true)
        {
            for (int i = 0; i < startDelay; i++)
            {
                yield return 0;
            }
            for (int i = 0; i < blinkFrames; i++)
            {
                blinkSprite.enabled = true;
                yield return 0;
            }
            blinkSprite.enabled = false;
            for (int i = 0; i < postBlink; i++)
            {
                yield return 0;
            }
            GameObject bullet = Instantiate(shot, shotPos.position, Quaternion.identity) as GameObject;
            bullet.transform.localScale = new Vector3(0.5f * facing, 0.5f, 1f);
            bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(shootSpeed * facing, 0f);
            GetComponent<AudioSource>().Play();
            Destroy(bullet, 2f);
            for (int i = 0; i < waitFrames - blinkFrames - postBlink - startDelay; i++)
            {
                yield return 0;
            }
        }
    }

    /// <summary>
    /// Message sent by DetectPlayer object when Taekos comes in the line of sight,
    /// Allow the Litholite to start shooting and start the Shooting coroutine
    /// </summary>
    void Attack(bool attack)
    {
        //Only call once if attack state is changing
        if (attacking == attack || dead)
            return;
        attacking = attack;
        if (attack)
        {
            StartCoroutine("Shooting");
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        else
        {
            StopCoroutine("Shooting");
            blinkSprite.enabled = false;
            if (move)
                GetComponent<Rigidbody2D>().velocity = new Vector2(facing * xspeed, 0f);
        }
    }

    /// <summary>
    /// Turn around if there is an edge or wall in front of the Litholite
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
    /// Fly off screen, shut off collision detection, and destroy the Litholite after 2 seconds
    /// </summary>
    void EnemyDeath()
    {
        Destroy(blinkSprite.gameObject);
        StopAllCoroutines();
        dead = true;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = new Vector2(-5f * facing, 10f);
        GetComponent<Rigidbody2D>().gravityScale = 1f;
        GetComponent<Rigidbody2D>().isKinematic = false;
        Destroy(gameObject, 2f);
    }
}
