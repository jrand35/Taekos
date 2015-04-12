using UnityEngine;
using System.Collections;

public class Piranha : MonoBehaviour {
    
    public Sprite frame1;
    public Sprite frame2;
    public int delay = 180;
    public float jumpSpeed = 25;
    private Vector3 startPosition;
    private SpriteRenderer spr;
	// Use this for initialization
	void Start () {
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
	
	// Update is called once per frame
	void Update () {
	}
}
