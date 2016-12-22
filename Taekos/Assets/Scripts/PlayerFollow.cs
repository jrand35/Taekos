using UnityEngine;
using System.Collections;

/// <summary>
/// Attached to the main camera, allows it to follow the player with a parallax background
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class PlayerFollow : MonoBehaviour {

	public Transform playerTransform;
	public Transform upLeftBound;       ///< Top left boundary of the screen
	public Transform downRightBound;    ///< Bottom right boundary
	public Transform background;        ///< Parallax background
	public float ratio = 0.8f;	        ///< 1: Background stays with camera, 0: Background does not move
	private Vector3 cameraStart;        ///< Camera's starting position
	private Vector3 backgroundStart;    ///< Background's starting position
    private bool followPlayer;          ///< True, allows the camera to follow the player

    /// <summary>
    /// Subscribe to events
    /// </summary>
    void OnEnable()
    {
        Controller.FollowPlayer += EnableFollowPlayer;
    }

    /// <summary>
    /// Unsubscribe to events
    /// </summary>
    void OnDisable()
    {
        Controller.FollowPlayer -= EnableFollowPlayer;
    }

	void Start(){
		cameraStart = transform.position;
		backgroundStart = background.position;
        followPlayer = true;
	}

	/// <summary>
	/// Update the Camera and background parallax
	/// </summary>
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

    /// <summary>
    /// Enable the camera to follow the player
    /// </summary>
    void EnableFollowPlayer(bool enable)
    {
        followPlayer = enable;
    }
}
