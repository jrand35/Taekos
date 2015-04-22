using UnityEngine;
using System.Collections;

public class Banana : MonoBehaviour {

    private float vel;
    //private float dvel = -15f;
    private float dvel;
    private int facing;
    private float zrot;
    private float dzrot = 10f;

    void Awake()
    {
        zrot = 0f;
    }

	// Use this for initialization
	void Start () {
        vel = rigidbody2D.velocity.x;
        dvel = -vel;
        if (vel > 0)
        {
            facing = 1;
        }
        else
        {
            facing = -1;
        }
	}
	
	void FixedUpdate () {
        vel += (dvel * Time.deltaTime);
        rigidbody2D.velocity = new Vector2(vel, 0f);
	}

    void Update()
    {
        zrot += dzrot;// *Time.deltaTime;
        transform.rotation = Quaternion.Euler(0f, 0f, zrot);
    }

    //Taekos catches the banana
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (rigidbody2D.velocity.x < 0 && other.transform.position.x < transform.position.x ||
                rigidbody2D.velocity.x > 0 && other.transform.position.x > transform.position.x)
            Destroy(gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Banana Boundary")
        {
            Destroy(gameObject);
        }
    }
}
