using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class Lifebar : MonoBehaviour {
	public Image lifebar;
	public Sprite[] sprites;
    public Text livesRemaining;
	private RectTransform rec;

	void Start(){
		rec = lifebar.transform.GetComponent<RectTransform> ();
	}

	public void UpdateLifebar(int currentLife, int maxLife, int lives){
		float ratio = ((float)currentLife) / ((float)maxLife);
		Sprite newSprite = sprites[0];
		if (ratio > 0.75f) {
			newSprite = sprites[0];	//100%
		}
		else if (ratio > 0.5f) {
			newSprite = sprites[1];	//75%
		}
		else if (ratio > 0.25f) {
			newSprite = sprites[2];	//50%
		}
		else if (ratio > 0f) {
			newSprite = sprites[3];	//25%
		}
        else
        {
            newSprite = sprites[4]; //0% (dead)
        }
		lifebar.sprite = newSprite;

        livesRemaining.text = "x " + lives;
	}
}