using UnityEngine;
using System.Collections;

public class MoveVertical : MonoBehaviour {

    public Transform topMarker;
    public Transform bottomMarker;
    public float speed = 3f;

	// Use this for initialization
	void Start () {
        StartCoroutine(Run());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator Run()
    {
        rigidbody2D.velocity = new Vector2(0f, speed);
        Vector3 top = topMarker.transform.position;
        Vector3 bottom = bottomMarker.transform.position;
        while (gameObject != null)
        {
            if (transform.position.y > top.y)
            {
                transform.position = new Vector3(transform.position.x, top.y, 0f);
                speed = -speed;
                rigidbody2D.velocity = new Vector2(0f, speed);
            }
            if (transform.position.y < bottom.y)
            {
                transform.position = new Vector3(transform.position.x, bottom.y, 0f);
                speed = -speed;
                rigidbody2D.velocity = new Vector2(0f, speed);
            }
            yield return 0;
        }
    }
}
