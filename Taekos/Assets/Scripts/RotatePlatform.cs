//Platform freezes if angular velocity is 0

using UnityEngine;
using System.Collections;

/// <summary>
/// Rotate and move certain platforms in the testing scene, not seen in the final build
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class RotatePlatform : MonoBehaviour {
	public float angularVelocity;
	public float moveDistance;
	public float moveSpeed;
	private float movePosition;
	private Vector2 startPosition;

	/// <summary>
	/// Get a reference to the rigidbody and set its angular velocity
	/// </summary>
	void Start () {
		movePosition = 0f;
		startPosition = GetComponent<Rigidbody2D>().position;
		GetComponent<Rigidbody2D>().angularVelocity = angularVelocity;//= new Vector2 (2f, 0);
	}
	
	/// <summary>
	/// Rotate and move the platform
	/// </summary>
	void Update () {
		movePosition += moveSpeed;
		while (movePosition > (2f * Mathf.PI)) {
			movePosition -= (2f * Mathf.PI);
		}
		float newY = startPosition.y + (moveDistance * Mathf.Sin (movePosition));
		transform.position = new Vector2 (GetComponent<Rigidbody2D>().position.x, newY);
	}
}
