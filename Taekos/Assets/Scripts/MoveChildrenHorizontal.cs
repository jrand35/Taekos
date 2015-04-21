using UnityEngine;
using System.Collections;

public class MoveChildrenHorizontal : MonoBehaviour
{
    public Transform rightMarker;
    public Transform leftMarker;
    public float speed = 3f;
    private Rigidbody2D[] childrenRigidbody;
    private Transform[] childrenTransform;
    private Transform childTransform;

    // Use this for initialization
    void Start()
    {
        childrenRigidbody = GetComponentsInChildren<Rigidbody2D>();
        childrenTransform = GetComponentsInChildren<Transform>();
        childTransform = transform.GetChild(0).transform;
        StartCoroutine(Run());
    }

    // Update is called once per frame
    void Update()
    {

    }

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
