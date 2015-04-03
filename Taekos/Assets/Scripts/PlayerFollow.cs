using UnityEngine;
using System.Collections;

public class PlayerFollow : MonoBehaviour {

	public Transform playerTransform;
	public Transform upLeftBound;
	public Transform downRightBound;
	public Transform background;
	public float ratio = 0.8f;	//1: Background stays with camera, 0: Background does not move
	private Vector3 cameraStart;
	private Vector3 backgroundStart;

	void Start(){
		cameraStart = transform.position;
		backgroundStart = background.position;
	}

	// Update is called once per frame
	void Update () {
		Vector3 newPosition = transform.position;
		newPosition.x = playerTransform.position.x;
		newPosition.y = playerTransform.position.y;
		newPosition.x = Mathf.Clamp (newPosition.x, upLeftBound.position.x, downRightBound.position.x);
		newPosition.y = Mathf.Clamp (newPosition.y, downRightBound.position.y, upLeftBound.position.y);
		transform.position = newPosition;

		Vector3 dCameraPos = transform.position - cameraStart;
		Vector3 newBackgroundPos = backgroundStart + new Vector3 (dCameraPos.x * ratio, dCameraPos.y * ratio, dCameraPos.z);
		background.position = newBackgroundPos;
	}
}
