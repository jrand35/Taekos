using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class Lifebar : MonoBehaviour {
	public Image lifebar;
	public Sprite[] sprites;
    public Text livesRemaining;
	private RectTransform rec;

    void OnEnable()
    {
        Controller.UpdateLifebar += UpdateLifebar;
    }

    void OnDisable()
    {
        Controller.UpdateLifebar -= UpdateLifebar;
    }

	void Start(){
		rec = lifebar.transform.GetComponent<RectTransform> ();
	}

	void UpdateLifebar(int currentLife, int lives){
		Sprite newSprite = sprites[0];

        switch (currentLife)
        {
            case 0:
                newSprite = sprites[4];	//0% (dead)
                break;

            case 1:
                newSprite = sprites[3];	//25%
                break;

            case 2:
                newSprite = sprites[2];	//50%
                break;

            case 3:
                newSprite = sprites[1];	//75%
                break;

            case 4:
                newSprite = sprites[0]; //100%
                break;
        }
		lifebar.sprite = newSprite;

        livesRemaining.text = "x " + lives;
	}
}