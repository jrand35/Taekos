using UnityEngine;
using System.Collections;

public class CollectItems : MonoBehaviour {

    public delegate void ScoreHandler(long score);
    public static event ScoreHandler addScore;
    public delegate void HealthHandler(int health);
    public static event HealthHandler addHealth;
    public delegate void FeathersHandler(int feathers);
    public static event FeathersHandler addFeathers;
	public GameObject bonusPoints;
    public AudioSource audio1;  //For powerups
    public AudioSource audio2;  //For mangos and feathers
	private float height = 1f;
	private Controller controller;

	void Start(){

		controller = GetComponentInParent<Controller> ();
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Powerups") {
			Destroy (other.gameObject);
            audio1.Play();

			PowerupPointValue pointValue = other.gameObject.GetComponent<PowerupPointValue>();
			if (pointValue != null){
				Vector3 pos = new Vector3(transform.position.x, transform.position.y + height, 0f);
				Quaternion rot = this.transform.rotation;
				GameObject points = (GameObject)Instantiate (bonusPoints, pos, rot);
				BonusPoints bonusPointsScript = points.GetComponent<BonusPoints>();
				bonusPointsScript.SetPoints(pointValue.getPoints ());
                addScore(pointValue.getPoints());
				controller.StartCoroutine (controller.MultiplyJumpSpeed(1.3f, 10f));
			}
		}
        else if (other.gameObject.tag == "Feathers")
        {
            addFeathers(1);
            audio2.Play();
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "Health items" && other.gameObject.GetComponent<Renderer>().enabled)
        {
            //Play item pickup sound
            //Disable the object's renderer so it cannot be seen by the camera
            //Wait until the sound has finished playing, then destroy the item
            audio2.Play();
            other.gameObject.GetComponent<Renderer>().enabled = false;
            if (other.transform.childCount != 0)
            {
                other.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
            }
            Destroy(other.gameObject, 2f);
            addHealth(1);
        }
	}
}
