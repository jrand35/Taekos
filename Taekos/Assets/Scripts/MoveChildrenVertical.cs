using UnityEngine;
using System.Collections;

/// <summary>
/// Move an entire group of tiles up and down like a moving wall
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class MoveChildrenVertical : MonoBehaviour
{
    public Transform upMarker;
    public Transform downMarker;
    public float speed = 3f;
    private Rigidbody2D[] childrenRigidbody;
    private Transform[] childrenTransform;
    private Transform childTransform;

    /// <summary>
    /// Get references to children and start Run coroutine
    /// </summary>
    void Start()
    {
        childrenRigidbody = GetComponentsInChildren<Rigidbody2D>();
        childrenTransform = GetComponentsInChildren<Transform>();
        childTransform = transform.GetChild(0).transform;
        StartCoroutine(Run());
    }

    /// <summary>
    /// Move the tiles up and down between 2 points
    /// </summary>
    IEnumerator Run()
    {
        foreach (Rigidbody2D rgbd in childrenRigidbody)
        {
            rgbd.velocity = new Vector2(0f, speed);
        }
        Vector3 up = upMarker.transform.position;
        Vector3 down = downMarker.transform.position;
        while (gameObject != null)
        {
            if (childTransform.position.y > up.y)
            {
                foreach (Transform t in childrenTransform)
                {
                    t.position = new Vector3(t.position.x, up.y, 0f);
                }
                speed = -speed;
                foreach (Rigidbody2D rgbd in childrenRigidbody)
                {
                    rgbd.velocity = new Vector2(0f, speed);
                }
            }
            if (childTransform.position.y < down.y)
            {
                foreach (Transform t in childrenTransform)
                {
                    t.position = new Vector3(t.position.x, down.y, 0f);
                }
                speed = -speed;
                foreach (Rigidbody2D rgbd in childrenRigidbody)
                {
                    rgbd.velocity = new Vector2(0f, speed);
                }
            }
            yield return 0;
        }
    }
}
