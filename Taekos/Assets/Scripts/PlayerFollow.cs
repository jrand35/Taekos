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
    private bool followPlayer;

    void OnEnable()
    {
        Controller.FollowPlayer += EnableFollowPlayer;
    }

    void OnDisable()
    {
        Controller.FollowPlayer -= EnableFollowPlayer;
    }

	void Start(){
		cameraStart = transform.position;
		backgroundStart = background.position;
        followPlayer = true;
	}

	// Update is called once per frame
	void Update () {
        if (!followPlayer) return;
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

    void EnableFollowPlayer(bool enable)
    {
        followPlayer = enable;
    }
}
