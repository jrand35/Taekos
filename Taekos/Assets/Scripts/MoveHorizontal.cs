using UnityEngine;
using System.Collections;

/// <summary>
/// Script for moving a single tile left and right like a moving platform
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class MoveHorizontal : MonoBehaviour
{

    public Transform rightMarker;
    public Transform leftMarker;
    public float speed = 3f;

    /// <summary>
    /// Start the Run coroutine
    /// </summary>
    void Start()
    {
        StartCoroutine(Run());
    }

    /// <summary>
    /// Move the platform left and right between 2 points
    /// </summary>
    /// <returns></returns>
    IEnumerator Run()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0f);
        Vector3 right = rightMarker.transform.position;
        Vector3 left = leftMarker.transform.position;
        while (gameObject != null)
        {
            if (transform.position.x > right.x)
            {
                transform.position = new Vector3(right.x, transform.position.y, 0f);
                speed = -speed;
                GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0f);
            }
            if (transform.position.x < left.x)
            {
                transform.position = new Vector3(left.x, transform.position.y, 0f);
                speed = -speed;
                GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0f);
            }
            yield return 0;
        }
    }
}
