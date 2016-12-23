using UnityEngine;
using System.Collections;

/// <summary>
/// Coconut obstacle,
/// A coconut hangs in a palm tree and drops on Taekos when he moves directly beneath it
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class Coconut : MonoBehaviour {
    private bool dropped = false;
    private int shakeTime = 20;
    private float deleteTime = 2f;          ///< Time in seconds after dropping the coconut to delete it
    private float dRad = 1.2f;              ///< Angle to be converted into horizontal distance with a Sin function
    private float ampl = 0.15f;             ///< Amplitude for the coconut's shake
    private Transform parentTransform;
    private Rigidbody2D parentRigidbody;

	/// <summary>
	/// Get the parent's transform and rigidbody
	/// </summary>
	void Start () {
        parentTransform = transform.parent.transform;
        parentRigidbody = transform.parent.GetComponent<Rigidbody2D>();
	}

    /// <summary>
    /// Detect Taekos then drop on his head with the Drop coroutine
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !dropped)
        {
            dropped = true;
            StartCoroutine(Drop());
        }
    }

    /// <summary>
    /// Fall on Taekos's head, but shake a little to warn him first
    /// </summary>
    /// <returns></returns>
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
    }
}
