using UnityEngine;
using System.Collections;

/// <summary>
/// Move an entire group of tiles left and right like a moving wall
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class MoveChildrenHorizontal : MonoBehaviour
{
    public Transform rightMarker;
    public Transform leftMarker;
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
    /// Move the tiles left and right between 2 points
    /// </summary>
    /// <returns></returns>
    IEnumerator Run()
    {
        foreach (Rigidbody2D rgbd in childrenRigidbody)
        {
            rgbd.velocity = new Vector2(speed, 0f);
        }
        Vector3 right = rightMarker.transform.position;
        Vector3 left = leftMarker.transform.position;
        while (gameObject != null)
        {
            if (childTransform.position.x > right.x)
            {
                foreach (Transform t in childrenTransform)
                {
                    t.position = new Vector3(right.x, t.position.y, 0f);
                }
                speed = -speed;
                foreach (Rigidbody2D rgbd in childrenRigidbody)
                {
                    rgbd.velocity = new Vector2(speed, 0f);
                }
            }
            if (childTransform.position.x < left.x)
            {
                foreach (Transform t in childrenTransform)
                {
                    t.position = new Vector3(left.x, t.position.y, 0f);
                }
                speed = -speed;
                foreach (Rigidbody2D rgbd in childrenRigidbody)
                {
                    rgbd.velocity = new Vector2(speed, 0f);
                }
            }
            yield return 0;
        }
    }
}
