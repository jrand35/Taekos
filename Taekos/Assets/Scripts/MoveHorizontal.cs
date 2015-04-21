using UnityEngine;
using System.Collections;

public class MoveHorizontal : MonoBehaviour
{

    public Transform rightMarker;
    public Transform leftMarker;
    public float speed = 3f;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(Run());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Run()
    {
        rigidbody2D.velocity = new Vector2(speed, 0f);
        Vector3 right = rightMarker.transform.position;
        Vector3 left = leftMarker.transform.position;
        while (gameObject != null)
        {
            if (transform.position.x > right.x)
            {
                transform.position = new Vector3(right.x, transform.position.y, 0f);
                speed = -speed;
                rigidbody2D.velocity = new Vector2(speed, 0f);
            }
            if (transform.position.x < left.x)
            {
                transform.position = new Vector3(left.x, transform.position.y, 0f);
                speed = -speed;
                rigidbody2D.velocity = new Vector2(speed, 0f);
            }
            yield return 0;
        }
    }
}
