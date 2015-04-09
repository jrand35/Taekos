using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class Lifebar : MonoBehaviour {
	public Image lifebar;       //White image
    public Image lifebarMask;   //Lifebar image
	public Sprite[] sprites;
    public Sprite[] maskSprites;
    public Text livesRemaining;
    private float blinkTime = 0.1f;
    private Color normalColor;
    private Color redColor;     //When taking damage
    private Color greenColor;   //When restoring health
	private RectTransform rec;

    void Awake()
    {
        normalColor = new Color(1f, 1f, 1f, 1f);
        redColor = new Color(1f, 0f, 0f, 0.5f);     //May need to fix
        greenColor = new Color(0f, 1f, 0f, 0.5f);   //May need to fix
    }

    void OnEnable()
    {
        Controller.UpdateLifebar += UpdateLifebar;
    }

    void OnDisable()
    {
        Controller.UpdateLifebar -= UpdateLifebar;
    }

	void Start(){
        rec = lifebar.transform.GetComponent<RectTransform>();
        //lifebarMask.color = new Color(1f, 1f, 0f, 0f);
	}

    //effectType
    //0: No effect
    //1: Blink red
    //2: Blink green
    void UpdateLifebar(int currentLife, int lives, int effectType)
    {
        lifebarMask.color = normalColor;
        Sprite newMaskSprite = maskSprites[0];
        Sprite newSprite = sprites[0];

        switch (currentLife)
        {
            case 0:
                newMaskSprite = maskSprites[4];	//0% (dead)
                newSprite = sprites[4];
                break;

            case 1:
                newMaskSprite = maskSprites[3];	//25%
                newSprite = sprites[3];
                break;

            case 2:
                newMaskSprite = maskSprites[2];	//50%
                newSprite = sprites[2];
                break;

            case 3:
                newMaskSprite = maskSprites[1];	//75%
                newSprite = sprites[1];
                break;

            case 4:
                newMaskSprite = maskSprites[0]; //100%
                newSprite = sprites[0];
                break;
        }
        lifebarMask.sprite = newMaskSprite;
        lifebar.sprite = newSprite;
        StartCoroutine(Blink(effectType));

        livesRemaining.text = "x " + lives;
	}

    //col
    //1: Red
    //2: Green
    IEnumerator Blink(int col)
    {
        if (col == 1)
        {
            lifebarMask.color = redColor;
        }
        else if (col == 2)
        {
            lifebarMask.color = greenColor;
        }
        yield return new WaitForSeconds(blinkTime);
        lifebarMask.color = normalColor;
    }
}