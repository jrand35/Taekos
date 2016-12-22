using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// The title screen controller,
/// Controls the main menu, options menu, and instructions menu
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class TitleScreen : MonoBehaviour {

	public GameObject cloudSprite;	        ///< Cloud prefab
    public GameObject mainMenu;             ///< Main menu group
    public GameObject mainMenuButtons;      ///< Main menu buttons group
    public GameObject optionsMenu;          ///< Options menu group
    public GameObject optionsMenuButtons;   ///< Options menu buttons group
    public SpriteRenderer skyScreen;        ///< The background sky, changes depending on the time of day
    public Sprite dayScreen;                ///< Sprite for playing the game during the day
    public Sprite nightScreen;              ///< Sprite for playing the game during the night
    public RectTransform InstructionsMenu;  ///< RectTransform for moving the instructions menu in and out of the screen
    public Image screenFade;                ///< A large, rectangular black sprite for fading the screen in and out
    public Image blinkingButton;            ///< Behind the start button, becomes visible when the player clicks "Start"
    public Image lifeCounter;               ///< Number sprite for the number of lives in the options menu
    public Sprite[] lifeNumbers;            ///< Number sprites, 0-9
    public Sprite blink1;                   ///< Blinking sprite 1
    public Sprite blink2;                   ///< Blinking sprite 2
    public Button startButton;
    public Button optionsButton;
    public Button backButton;
    public Button InstructionsBackButton;
	public Transform cloudSpawn;            ///< Spawning position for the clouds
	public Transform top;                   ///< Top of the cloud spawning area
	public Transform bottom;                ///< Bottom of the cloud spawning area
    public int fadeTime = 45;               ///< Time in frames to fade the screen out to the first level
    private int startingNumberOfLives;      ///< Set to Settings.numberOfLives
    private int startingNumberOfContinues;  ///< Set to Settings.numberOfContinues
    private int musicVolume;                ///< Set to Settings.MusicVolume
    private int soundVolume;                ///< Set to Settings.SoundVolume
    private int menu;                       ///< Index of the currently active menu, 0 = Main menu, 1 = Options menu
    private GameObject gameStartSound;
    private GameObject buttonPressSound;
    private SpriteRenderer spriteRenderer;
    private bool onMainMenu;                ///< True if currently on the main menu
    private bool onOptionsMenu;             ///< True if currently on the options menu
	private bool dayTime;                   ///< Whether it is currently day or night
    //private bool buttonClicked;
    private bool onInstructions;            ///< True if currently on the instructions menu
    private float dMenuX = 53f;             ///< The speed to move the screen when the user changes the menu
    private float menuDistance = 800;       ///< The total distance to move the screen when changing menus
    private const int MENU_MAIN = 0;        ///< The main menu index
    private const int MENU_OPTIONS = 1;     ///< The options menu index
    private Vector3 instructionsPos;        ///< The position for the instructions menu

    /// <summary>
    /// Set the main menu values to the values in the Settings static class
    /// </summary>
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

	/// <summary>
    /// Start the game in the main menu
    /// </summary>
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
        //buttonClicked = false;
        UpdateLifeCounter(0, false);
        instructionsPos = InstructionsMenu.localPosition;
        InstructionsBackButton.enabled = false;
	}

    /// <summary>
    /// Subscribe to events, when the left and right life buttons are clicked
    /// </summary>
    void OnEnable()
    {
        LeftLifeButtonController.leftLifeButtonClicked += UpdateLifeCounter;
        RightLifeButtonController.rightLifeButtonClicked += UpdateLifeCounter;
    }

    /// <summary>
    /// Unsubscribe to events
    /// </summary>
    void OnDisable()
    {
        LeftLifeButtonController.leftLifeButtonClicked -= UpdateLifeCounter;
        RightLifeButtonController.rightLifeButtonClicked -= UpdateLifeCounter;
    }
	
	/// <summary>
    /// Read input from the keyboard to navigate the menu
    /// </summary>
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

    /// <summary>
    /// Create the clouds
    /// </summary>
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

    /// <summary>
    /// Called by the instructions menu button
    /// </summary>
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

    /// <summary>
    /// Called by the back button on the instructions menu button
    /// </summary>
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

    /// <summary>
    /// Called by the start button
    /// </summary>
    /// <returns></returns>
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
        SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// Move the screen like a transition when changing menus
    /// </summary>
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

    /// <summary>
    /// Enable the currently active menu and disable all inactive menus
    /// </summary>
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

    /// <summary>
    /// Call the MoveMenu coroutine
    /// </summary>
    public void ChangeMenu(int setMenu)
    {
        buttonPressSound.GetComponent<AudioSource>().Play();
        menu = setMenu;
        StartCoroutine(MoveMenu());
    }

    /// <summary>
    /// Update the life counter UI
    /// </summary>
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