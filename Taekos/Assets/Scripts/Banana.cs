using UnityEngine;
using System.Collections;

/// <summary>
/// Banana that Taekos uses as a weapon
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class Banana : MonoBehaviour {

    public delegate void DestroyHandler(int add);       ///< Event handler for when the banana is destroyed
    public static event DestroyHandler destroyBanana;   ///< Event for when the banana is destroyed
    public LayerMask whatIsGround;                      ///< Layer mask for collision detection with walls
    public Transform top;                               ///< Collision detection at the top of the banana
    public Transform front;                             ///< Collision detection at the front of the banana
    public Transform bottom;                            ///< Collision detection at the bottom of the banana
    public Transform back;                              ///< Collision detection at the back of the banana
    public Color hit1;                                  ///< Color of the banana after hitting an enemy once
    public Color hit2;                                  ///< Color of the banana after hitting an enemy twice
    private SpriteRenderer spr;                         ///< Reference to the banana's sprite renderer
    private int facing;                                 ///< 1 for facing right, -1 for facing left
    private int catchDelay = 8;                         ///< Delay in frames before Taekos can catch the Banana
    private int life;                                   ///< How many hits the banana can take (3)
    private float move = 5f;                            ///< Vertical movement when holding the up or down arrow key
    private float initialvel;                           ///< Starting velocity
    private float maxvel;                               ///< Maximum velocity
    private float vel;                                  ///< Current velocity
    private float dvel;                                 ///< Horizontal acceleration
    private float zrot;                                 ///< Current rotation
    private float dzrot = 10f;                          ///< Rotation speed
    private bool returning;                             ///< True if the banana is moving back toward Taekos after being thrown

    void Awake()
    {
        life = 3;
        zrot = 0f;
        returning = false;
    }

    void Start()
    {
        spr = GetComponentInChildren<SpriteRenderer>();
        StartCoroutine(Delay());
        vel = GetComponent<Rigidbody2D>().velocity.x;
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
	
    /// <summary>
    /// Move the banana after being thrown, adjust speed,
    /// move vertical if up or down arrow is being held,
    /// Don't move the banana up or down if there is a wall above or below it
    /// </summary>
	void FixedUpdate () {
        vel += (dvel * Time.deltaTime);
        GetComponent<Rigidbody2D>().velocity = new Vector2(vel, 0f);

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
                GetComponent<Rigidbody2D>().velocity = new Vector2(vel, 0f);
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
            GetComponent<Rigidbody2D>().velocity = new Vector2(vel, 0f);
        }
        if (maxvel < 0 && vel < maxvel)
        {
            vel = maxvel;
            GetComponent<Rigidbody2D>().velocity = new Vector2(vel, 0f);
        }
	}

    /// <summary>
    /// Rotate the banana on the Z axis
    /// </summary>
    void Update()
    {
        zrot += dzrot;// *Time.deltaTime;
        spr.transform.rotation = Quaternion.Euler(0f, 0f, zrot);
    }

    /// <summary>
    /// Hit enemies and reverse the banana's direction
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemies")
        {
            other.SendMessage("DamageEnemy", facing);
            vel = -vel;
            GetComponent<Rigidbody2D>().velocity = new Vector2(vel, 0f);
            transform.position = new Vector3(transform.position.x + vel * Time.deltaTime, transform.position.y, transform.position.z);
            GetComponent<AudioSource>().Play();

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
    
    /// <summary>
    /// Taekos catches the banana
    /// </summary>
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

    /// <summary>
    /// Destroy the banana if it goes off screen
    /// </summary>
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Banana Boundary")
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Send a message to Taekos,
    /// After the banana is destroyed, Taekos can throw bananas again
    /// </summary>
    void OnDestroy()
    {
        destroyBanana(-1);
    }

    /// <summary>
    /// Temporarily prevent Taekos from catching the banana when he first throws it
    /// </summary>
    IEnumerator Delay()
    {
        int time = catchDelay;
        for (int i = 0; i < time; i++){
            catchDelay--;
            yield return 0;
        }
    }
}
