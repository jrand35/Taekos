using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleScreen : MonoBehaviour {

	public GameObject cloudSprite;	//Cloud prefab
    public Image screenFade;
    public Image blinkingButton;
    public Sprite blink1;
    public Sprite blink2;
    public Button startButton;
	public Transform cloudSpawn;
	public Transform top;
	public Transform bottom;
    public int fadeTime = 45;
    private GameObject gameStartSound;
    private SpriteRenderer spriteRenderer;
	private bool dayTime;
    private bool buttonClicked;

	// Use this for initialization
	void Start () {
        blinkingButton.active = false;
        gameStartSound = transform.GetChild(0).gameObject;
        screenFade.color = new Color(1f, 1f, 1f, 0f);
		StartCoroutine (Run ());
		var time = System.DateTime.Now;
		dayTime = (time.Hour >= 6 && time.Hour < 18);
        buttonClicked = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator Run(){
		while (true) {
			float cloudDelay = Random.value * 0.7f;
			yield return new WaitForSeconds (cloudDelay);

			Color cloudColor;
			if (dayTime){
				cloudColor = new Color(1f, 1f, 1f, 1f);
			}
			else{
				cloudColor = new Color(0.4f, 0.4f, 0.4f, 4f);
			}
			Vector3 cloudPos = cloudSpawn.transform.position;
			float topY = top.transform.position.y;
			float bottomY = bottom.transform.position.y;
			float range = topY - bottomY;
			cloudPos.y = bottomY + (range * Random.value);
			float hSpeed = (topY - cloudPos.y + 6f) / (- 3f);
			float scale = 0.5f + ((topY - cloudPos.y) / 8);
			GameObject newCloud = Instantiate (cloudSprite, cloudPos, new Quaternion (0f, 0f, 0f, 0f)) as GameObject;
			SpriteRenderer spriteRenderer = newCloud.GetComponent<SpriteRenderer>();
			newCloud.rigidbody2D.velocity = new Vector2 (hSpeed, 0f);
			spriteRenderer.color = cloudColor;
			spriteRenderer.sortingOrder = 100 - (int)(cloudPos.y * 10);
			newCloud.transform.localScale = new Vector3(scale, scale, 1f);
			Destroy (newCloud, 12f);
		}
	}

    public IEnumerator LoadFirstLevel()
    {
        gameStartSound.audio.Play();
        screenFade.rectTransform.SetAsLastSibling();    //Move the image in front of the button and title screen
        float alpha = 0f;
        float dAlpha = 1f / (float)fadeTime;
        float dVolume = audio.volume / (float)fadeTime;

        //Remove button, show blinking button
        startButton.active = false;
        blinkingButton.active = true;

        for (int i = 0; i < fadeTime; i++)
        {
            audio.volume -= dVolume;
            alpha += dAlpha;
            screenFade.color = new Color(1f, 1f, 1f, alpha);

            //Button blink
            if (i % 4 == 0 || i % 4 == 1)
            {
                blinkingButton.sprite = blink1;
            }
            else
            {
                blinkingButton.sprite = blink2;
            }
            yield return 0;
        }
        Application.LoadLevel("Main");
    }
}
