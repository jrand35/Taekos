using UnityEngine;
using System.Collections;

/// <summary>
/// Script for moving a single tile up and down like a moving platform
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class MoveVertical : MonoBehaviour {

    public Transform topMarker;
    public Transform bottomMarker;
    public float speed = 3f;

	/// <summary>
	/// Start the Run coroutine
	/// </summary>
	void Start () {
        StartCoroutine(Run());
	}

    /// <summary>
    /// Move the platform up and down between 2 points
    /// </summary>
    IEnumerator Run()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0f, speed);
        Vector3 top = topMarker.transform.position;
        Vector3 bottom = bottomMarker.transform.position;
        while (gameObject != null)
        {
            if (transform.position.y > top.y)
            {
                transform.position = new Vector3(transform.position.x, top.y, 0f);
                speed = -speed;
                GetComponent<Rigidbody2D>().velocity = new Vector2(0f, speed);
            }
            if (transform.position.y < bottom.y)
            {
                transform.position = new Vector3(transform.position.x, bottom.y, 0f);
                speed = -speed;
                GetComponent<Rigidbody2D>().velocity = new Vector2(0f, speed);
            }
            yield return 0;
        }
    }
}
