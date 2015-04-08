using UnityEngine;
using System.Collections;

public class PickUpPowerups : MonoBehaviour {

    public delegate void HealthHandler(int health);
    public static event HealthHandler addHealth;
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
        else if (other.gameObject.tag == "Health items" && other.gameObject.renderer.enabled)
        {
            //Play item pickup sound
            //Disable the object's renderer so it cannot be seen by the camera
            //Wait until the sound has finished playing, then destroy the item
            other.gameObject.audio.Play();
            other.gameObject.renderer.enabled = false;
            if (other.transform.childCount != 0)
            {
                other.transform.GetChild(0).renderer.enabled = false;
            }
            Destroy(other.gameObject, 2f);
            addHealth(1);
        }
	}
}
