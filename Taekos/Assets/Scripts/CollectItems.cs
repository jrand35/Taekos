using UnityEngine;
using System.Collections;

/// <summary>
/// A box collider that allows Taekos to collect feathers and mangos, a child of Taekos's GameObject
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class CollectItems : MonoBehaviour {

    public delegate void ScoreHandler(long score);      ///< Event handler for changes to the score
    public static event ScoreHandler addScore;          ///< Send a message to ScoreTextController when Taekos collects a powerup (not used in the final build)
    public delegate void HealthHandler(int health);     ///< Event handler for collecting a health powerup
    public static event HealthHandler addHealth;        ///< Send a message to Controller when Taekos collects a mango 
    public delegate void FeathersHandler(int feathers); ///< Event handler for collecting feathers
    public static event FeathersHandler addFeathers;    ///< Send a message to GameController when Taekos collects a feather
	public GameObject bonusPoints;                      ///< A Prefab of a "100" text sprite, appears when Taekos collects a powerup (not used in the final build)
    public AudioSource audio1;                          ///< Sound effect for collecting powerups
    public AudioSource audio2;                          ///< Sound effect for collecting mangos and feathers
	private float height = 1f;                          ///< Offset y position when instantiating a powerup prefab
	private Controller controller;                      ///< Reference to the Controller object

    /// <summary>
    /// Get a reference to the Controller
    /// </summary>
	void Start(){

		controller = GetComponentInParent<Controller> ();
	}

    /// <summary>
    /// When Taekos collects a powerup, adjust his maximum jump height,
    /// When he collects a feather, update the GameController and feather counter,
    /// When he collects a mango (health item), update the Controller and add a point to his life bar
    /// </summary>
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
