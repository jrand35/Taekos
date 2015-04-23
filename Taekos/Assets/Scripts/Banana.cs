using UnityEngine;
using System.Collections;

public class Banana : MonoBehaviour {

    public delegate void DestroyHandler(int add);
    public static event DestroyHandler destroyBanana;
    public LayerMask whatIsGround;
    public Transform top;
    public Transform front;
    public Transform bottom;
    public Transform back;
    public Color hit1;
    public Color hit2;
    private SpriteRenderer spr;
    private int facing;
    private int catchDelay = 8;
    private int life;
    private float move = 5f;
    private float initialvel;
    private float maxvel;
    private float vel;
    private float dvel;
    private float zrot;
    private float dzrot = 10f;
    private bool returning;

    void Awake()
    {
        life = 3;
        zrot = 0f;
        returning = false;
    }

	// Use this for initialization
    void Start()
    {
        spr = GetComponentInChildren<SpriteRenderer>();
        StartCoroutine(Delay());
        vel = rigidbody2D.velocity.x;
        initialvel = vel;
        dvel = -vel * 1.5f;
        maxvel = -initialvel;
        if (vel > 0)
        {
            facing = 1;
        }
        else
        {
            //dzrot = -dzrot;
            facing = -1;
        }
        transform.localScale = new Vector3(facing, 1f, 1f);
	}
	
	void FixedUpdate () {
        vel += (dvel * Time.deltaTime);
        rigidbody2D.velocity = new Vector2(vel, 0f);

        bool topTouching = Physics2D.OverlapPoint(top.position, whatIsGround);
        bool bottomTouching = Physics2D.OverlapPoint(bottom.position, whatIsGround);

        if (Input.GetAxisRaw("Vertical") == 1f)
        {
            if(!topTouching)
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + move * Time.deltaTime, transform.localPosition.z);
        }
        else if (Input.GetAxisRaw("Vertical") == -1f)
        {
            if (!bottomTouching)
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - move * Time.deltaTime, transform.localPosition.z);
        }
        if (Physics2D.OverlapPoint(back.position, whatIsGround))
        {
            Destroy(gameObject);
        }
        while (Physics2D.OverlapPoint(front.position, whatIsGround))
        {
            if ((facing == 1 && vel > 0) || (facing == -1 && vel < 0))
            {
                vel = 0f;
                rigidbody2D.velocity = new Vector2(vel, 0f);
            }
            transform.position = new Vector3(transform.position.x - facing*0.05f, transform.position.y, transform.position.z);
        }
        if ((initialvel > 0 && vel < 0) || (initialvel < 0 && vel > 0))
        {
            returning = true;
        }
        else
        {
            returning = false;
        }
        if (maxvel > 0 && vel > maxvel)
        {
            vel = maxvel;
            rigidbody2D.velocity = new Vector2(vel, 0f);
        }
        if (maxvel < 0 && vel < maxvel)
        {
            vel = maxvel;
            rigidbody2D.velocity = new Vector2(vel, 0f);
        }
	}

    void Update()
    {
        zrot += dzrot;// *Time.deltaTime;
        spr.transform.rotation = Quaternion.Euler(0f, 0f, zrot);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemies")
        {
            other.SendMessage("DamageEnemy", facing);
            vel = -vel;
            rigidbody2D.velocity = new Vector2(vel, 0f);
            transform.position = new Vector3(transform.position.x + vel * Time.deltaTime, transform.position.y, transform.position.z);
            audio.Play();

            life--;
            if (life == 2)
            {
                spr.color = hit1;
            }
            else if (life == 1)
            {
                spr.color = hit2;
            }
            else if (life == 0)
            {
                Destroy(gameObject);
            }
        }
    }

    //Taekos catches the banana
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (catchDelay == 0)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Banana Boundary")
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        destroyBanana(-1);
    }

    IEnumerator Delay()
    {
        int time = catchDelay;
        for (int i = 0; i < time; i++){
            catchDelay--;
            yield return 0;
        }
    }
}
