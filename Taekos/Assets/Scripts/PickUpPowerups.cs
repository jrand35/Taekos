using UnityEngine;
using System.Collections;

public class PickUpPowerups : MonoBehaviour {
	
	public ScoreTextController scoreTextController;
	public GameObject bonusPoints;
	private float height = 1f;
	private Controller controller;

	void Start(){

		controller = GetComponentInParent<Controller> ();
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Powerups") {
			Destroy (other.gameObject);
			audio.Play ();

			PowerupPointValue pointValue = other.gameObject.GetComponent<PowerupPointValue>();
			if (pointValue != null){
				Vector3 pos = new Vector3(transform.position.x, transform.position.y + height, 0f);
				Quaternion rot = this.transform.rotation;
				GameObject points = (GameObject)Instantiate (bonusPoints, pos, rot);
				BonusPoints bonusPointsScript = points.GetComponent<BonusPoints>();
				bonusPointsScript.SetPoints(pointValue.getPoints ());
				scoreTextController.UpdateScore (pointValue.getPoints ());
				controller.StartCoroutine (controller.MultiplyJumpSpeed(1.3f, 10f));
			}
		}
	}
}
