using UnityEngine;
using System.Collections;

public class MoveChildrenVertical : MonoBehaviour
{
    public Transform upMarker;
    public Transform downMarker;
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
