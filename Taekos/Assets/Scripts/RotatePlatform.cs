//Platform freezes if angular velocity is 0

using UnityEngine;
using System.Collections;

public class RotatePlatform : MonoBehaviour {
	public float angularVelocity;
	public float moveDistance;
	public float moveSpeed;
	private float movePosition;
	private Vector2 startPosition;

	// Use this for initialization
	void Start () {
		movePosition = 0f;
		startPosition = GetComponent<Rigidbody2D>().position;
		GetComponent<Rigidbody2D>().angularVelocity = angularVelocity;//= new Vector2 (2f, 0);
	}
	
	// Update is called once per frame
	void Update () {
		movePosition += moveSpeed;
		while (movePosition > (2f * Mathf.PI)) {
			movePosition -= (2f * Mathf.PI);
		}
		float newY = startPosition.y + (moveDistance * Mathf.Sin (movePosition));
		transform.position = new Vector2 (GetComponent<Rigidbody2D>().position.x, newY);
	}

	void FixedUpdate(){
	}
}
