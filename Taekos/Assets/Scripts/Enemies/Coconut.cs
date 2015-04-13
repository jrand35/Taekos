using UnityEngine;
using System.Collections;

public class Coconut : MonoBehaviour {
    private bool dropped = false;
    private int shakeTime = 20;
    private float respawnTime = 3f;
    private float deleteTime = 2f;
    private float dRad = 1.2f;
    private float ampl = 0.15f;
    private Transform parentTransform;
    private Rigidbody2D parentRigidbody;

	// Use this for initialization
	void Start () {
        parentTransform = transform.parent.transform;
        parentRigidbody = transform.parent.rigidbody2D;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !dropped)
        {
            dropped = true;
            StartCoroutine(Drop());
        }
    }

    IEnumerator Drop()
    {
        Vector3 initialPos = parentTransform.position;
        //Initial shake
        float x = parentTransform.position.x;
        float rad = 0f;
        for (int i = 0; i < shakeTime; i++)
        {
            rad += dRad;
            if (rad > 2 * Mathf.PI){
                rad -= (2 * Mathf.PI);
            }
            float newX = x + (ampl * Mathf.Sin(rad));
            parentTransform.position = new Vector3(newX, parentTransform.position.y, 0f);
            yield return 0;
        }
        //Gravity
        parentRigidbody.gravityScale = 1f;
        Destroy(transform.parent.gameObject, deleteTime);

        //yield return new WaitForSeconds(respawnTime);

        //Spawn new coconut?
    }
}
