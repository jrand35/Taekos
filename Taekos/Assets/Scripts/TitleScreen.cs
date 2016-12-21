﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleScreen : MonoBehaviour {

	public GameObject cloudSprite;	//Cloud prefab
    public GameObject mainMenu;
    public GameObject mainMenuButtons;
    public GameObject optionsMenu;
    public GameObject optionsMenuButtons;
    public SpriteRenderer skyScreen;
    public Sprite dayScreen;
    public Sprite nightScreen;
    public RectTransform InstructionsMenu;
    public Image screenFade;
    public Image blinkingButton;
    public Image lifeCounter;
    public Sprite[] lifeNumbers;
    public Sprite blink1;
    public Sprite blink2;
    public Button startButton;
    public Button optionsButton;
    public Button backButton;
    public Button InstructionsBackButton;
	public Transform cloudSpawn;
	public Transform top;
	public Transform bottom;
    public int fadeTime = 45;
    private int startingNumberOfLives;
    private int startingNumberOfContinues;
    private int musicVolume;
    private int soundVolume;
    private int menu;
    private GameObject gameStartSound;
    private GameObject buttonPressSound;
    private SpriteRenderer spriteRenderer;
    private bool onMainMenu;
    private bool onOptionsMenu;
	private bool dayTime;
    private bool buttonClicked;
    private bool onInstructions;
    private float dMenuX = 53f;
    private float menuDistance = 800;
    private const int MENU_MAIN = 0;
    private const int MENU_OPTIONS = 1;
    private Vector3 instructionsPos;

    void Awake()
    {
        onInstructions = false;
        Settings.Results.Feathers = 0;
        Settings.Results.Score = 0;
        Settings.Results.Time = 0;
        musicVolume = Settings.MusicVolume;
        soundVolume = Settings.SoundVolume;
        startingNumberOfLives = Settings.NumberOfLives;
        startingNumberOfContinues = Settings.NumberOfContinues;
    }

	// Use this for initialization
	void Start () {
        menu = MENU_MAIN;
        SetMenuActive(true, false);
        blinkingButton.enabled = false;
        gameStartSound = transform.GetChild(0).gameObject;
        buttonPressSound = transform.GetChild(1).gameObject;
        screenFade.color = new Color(1f, 1f, 1f, 0f);
		StartCoroutine (Run ());
		var time = System.DateTime.Now;
		dayTime = (time.Hour >= 6 && time.Hour < 18);
        buttonClicked = false;
        UpdateLifeCounter(0, false);
        instructionsPos = InstructionsMenu.localPosition;
        InstructionsBackButton.enabled = false;
	}

    void OnEnable()
    {
        LeftLifeButtonController.leftLifeButtonClicked += UpdateLifeCounter;
        RightLifeButtonController.rightLifeButtonClicked += UpdateLifeCounter;
    }

    void OnDisable()
    {
        LeftLifeButtonController.leftLifeButtonClicked -= UpdateLifeCounter;
        RightLifeButtonController.rightLifeButtonClicked -= UpdateLifeCounter;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(LoadFirstLevel());
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (onOptionsMenu)
            {
                ChangeMenu(0);
            }
            else if (onInstructions)
            {
                returnFromInstructions();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)){
            if (onMainMenu)
            {
                goToInstructions();
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (onMainMenu)
            {
                ChangeMenu(1);
            }
        }
	}

	IEnumerator Run(){
		while (true) {
			float cloudDelay = Random.value * 0.7f;
			yield return new WaitForSeconds (cloudDelay);

			Color cloudColor;
			if (dayTime){
				cloudColor = new Color(1f, 1f, 1f, 1f);
                skyScreen.sprite = dayScreen;
			}
			else{
                cloudColor = new Color(0.4f, 0.4f, 0.4f, 4f);
                skyScreen.sprite = nightScreen;
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
			newCloud.GetComponent<Rigidbody2D>().velocity = new Vector2 (hSpeed, 0f);
			spriteRenderer.color = cloudColor;
			spriteRenderer.sortingOrder = 100 - (int)(cloudPos.y * 10);
			newCloud.transform.localScale = new Vector3(scale, scale, 1f);
			Destroy (newCloud, 12f);
		}
	}

    public void goToInstructions()
    {
        onInstructions = true;
        buttonPressSound.GetComponent<AudioSource>().Play();
        InstructionsBackButton.enabled = true;
        SetMenuActive(false, false);
        mainMenu.transform.position = new Vector3(
            mainMenu.transform.position.x,
            mainMenu.transform.position.y - 2000f,
            0f);

        InstructionsMenu.localPosition = new Vector3(0f, 0f, 0f);
    }

    public void returnFromInstructions()
    {
        onInstructions = false;
        buttonPressSound.GetComponent<AudioSource>().Play();
        InstructionsBackButton.enabled = false;
        SetMenuActive(true, false);
        mainMenu.transform.position = new Vector3(
            mainMenu.transform.position.x,
            mainMenu.transform.position.y + 2000f,
            0f);

        InstructionsMenu.localPosition = instructionsPos;
    }

    public IEnumerator LoadFirstLevel()
    {
        gameStartSound.GetComponent<AudioSource>().Play();
        screenFade.rectTransform.SetAsLastSibling();    //Move the image in front of the button and title screen
        float alpha = 0f;
        float dAlpha = 1f / (float)fadeTime;
        float dVolume = GetComponent<AudioSource>().volume / (float)fadeTime;

        //Remove button, show blinking button
        startButton.enabled = false;
        blinkingButton.enabled = true;

        for (int i = 0; i < fadeTime; i++)
        {
            GetComponent<AudioSource>().volume -= dVolume;
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

    IEnumerator MoveMenu()
    {
        //Set to main menu
        if (menu == 0)
        {
            SetMenuActive(false, false);

            while (mainMenu.transform.localPosition.x < 0f)
            {
                Vector3 newMainPosition = mainMenu.transform.localPosition;
                Vector3 newOptionsPosition = optionsMenu.transform.localPosition;
                newMainPosition.x += dMenuX;
                newOptionsPosition.x += dMenuX;
                newMainPosition.x = Mathf.Min(0f, newMainPosition.x);
                newOptionsPosition.x = Mathf.Min(menuDistance, newOptionsPosition.x);
                mainMenu.transform.localPosition = newMainPosition;
                optionsMenu.transform.localPosition = newOptionsPosition;
                yield return 0;
            }
            SetMenuActive(true, false);
        }
        //Menus move to left
        //Set to options menu
        else if (menu == 1)
        {
            SetMenuActive(false, false);

            while (optionsMenu.transform.localPosition.x > 0f)
            {
                Vector3 newMainPosition = mainMenu.transform.localPosition;
                Vector3 newOptionsPosition = optionsMenu.transform.localPosition;
                newMainPosition.x -= dMenuX;
                newOptionsPosition.x -= dMenuX;
                newMainPosition.x = Mathf.Max(-menuDistance, newMainPosition.x);
                newOptionsPosition.x = Mathf.Max(0f, newOptionsPosition.x);
                mainMenu.transform.localPosition = newMainPosition;
                optionsMenu.transform.localPosition = newOptionsPosition;
                yield return 0;
            }
            SetMenuActive(false, true);
        }
    }

    void SetMenuActive(bool mainMenuActive, bool optionsMenuActive)
    {
        onMainMenu = mainMenuActive;
        onOptionsMenu = optionsMenuActive;
        Button[] mainButtons = mainMenuButtons.GetComponentsInChildren<Button>();
        foreach (Button b in mainButtons)
        {
            b.enabled = mainMenuActive;
        }

        Button[] optionsButtons = optionsMenuButtons.GetComponentsInChildren<Button>();
        foreach (Button b in optionsButtons){
            b.enabled = optionsMenuActive;
        }
    }

    public void ChangeMenu(int setMenu)
    {
        buttonPressSound.GetComponent<AudioSource>().Play();
        menu = setMenu;
        StartCoroutine(MoveMenu());
    }

    public void UpdateLifeCounter(int addLife, bool playSound)
    {
        Settings.NumberOfLives += addLife;
        if (Settings.NumberOfLives < 0)
        {
            Settings.NumberOfLives = 0;
        }
        if (Settings.NumberOfLives > 9)
        {
            Settings.NumberOfLives = 9;
        }

    /*    if (Settings.NumberOfLives == 0)
        {
            leftLifeButton.enabled = false;
        }
        else
        {
            leftLifeButton.enabled = true;
        }
        if (Settings.NumberOfLives == 9)
        {
            rightLifeButton.enabled = false;
        }
        else
        {
            rightLifeButton.enabled = true;
        }*/
        lifeCounter.sprite = lifeNumbers[Settings.NumberOfLives];
        if (playSound)
        {
            buttonPressSound.GetComponent<AudioSource>().Play();
        }
    }
}