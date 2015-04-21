using UnityEngine;
using System.Collections;

public class Litholite : MonoBehaviour
{

    public LayerMask whatIsGround;
    public Transform edge;
    public Transform front;
    public bool move;
    public int facing = 1;
    private Animator anim;
    private float xspeed = 2;
    private bool attacking;
    private bool dead;
    private IEnumerator shooting;

    void Awake()
    {
        shooting = Shooting();
        attacking = false;
        dead = false;
    }

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        if (move)
            rigidbody2D.velocity = new Vector2(facing * xspeed, 0f);
        if (facing == -1)
            ReverseDirection();
    }

    void Update()
    {
        if (!attacking)
        {
            StopCoroutine("Shooting");
        }
        if (move && (!Physics2D.OverlapPoint(edge.position, whatIsGround) || Physics2D.OverlapPoint(front.position, whatIsGround)))
        {
            facing = -facing;
            ReverseDirection();
        }
        anim.SetBool("Attacking", attacking);
        anim.SetBool("Dead", dead);
    }

    IEnumerator Shooting()
    {
        while (true)
        {
            for (int i = 0; i < 60; i++)
            {
                yield return 0;
            }
            audio.Play();
        }
    }

    void Attack(bool attack)
    {
        attacking = attack;
        if (attack)
        {
            //shooting = Shooting();
            StartCoroutine("Shooting");
            rigidbody2D.velocity = Vector2.zero;
        }
        else
        {
            StopCoroutine("Shooting");
            rigidbody2D.velocity = new Vector2(facing * xspeed, 0f);
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
