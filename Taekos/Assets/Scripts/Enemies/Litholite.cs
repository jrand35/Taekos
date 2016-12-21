using UnityEngine;
using System.Collections;

public class Litholite : MonoBehaviour
{
    public GameObject shot;
    public Transform shotPos;
    public LayerMask whatIsGround;
    public Transform edge;
    public Transform front;
    public SpriteRenderer blinkSprite;
    public bool move;
    public int facing = 1;
    public int startDelay;
    private Animator anim;
    private int postBlink = 10;
    private int blinkFrames = 5;
    private int waitFrames = 70;
    private float xspeed = 2;
    private float shootSpeed = 12f;
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
            GetComponent<Rigidbody2D>().velocity = new Vector2(facing * xspeed, 0f);
        if (facing == -1)
            ReverseDirection();
    }

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
