using UnityEngine;
using System.Collections;

public class Piranha : MonoBehaviour {
    
    public Sprite frame1;
    public Sprite frame2;
    public int delay = 180;
    public float jumpSpeed = 25;
    private Vector3 startPosition;
    private SpriteRenderer spr;
    private bool dead;
	// Use this for initialization
	void Start () {
        dead = false;
        startPosition = transform.position;
        spr = GetComponent<SpriteRenderer>();
        StartCoroutine(Run());
	}

    IEnumerator Run()
    {
        while (true)
        {
            transform.position = startPosition;
            rigidbody2D.velocity = new Vector2(0f, jumpSpeed);
            for (int i = 0; i < delay; i++)
            {
                if (rigidbody2D.velocity.y > 0)
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

    void EnemyDeath()
    {
        StopAllCoroutines();
        dead = true;
        collider2D.enabled = false;
        rigidbody2D.velocity = new Vector2(-5f, 10f);
        rigidbody2D.gravityScale = 1f;
        rigidbody2D.isKinematic = false;
        Destroy(gameObject, 2f);
    }
	
	// Update is called once per frame
	void Update () {
	}
}
