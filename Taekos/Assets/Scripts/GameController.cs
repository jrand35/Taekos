using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// The game controller. Keeps track of the time, number of lives, feathers, whether the game is over, and whether or not to unlock the door
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class GameController : MonoBehaviour {

    public delegate void FeatherHandler(int numFeathers, int maxFeathers);      ///< Event handler for UpdateFeatherCounter
    public static event FeatherHandler UpdateFeatherCounter;                    ///< Called in Lifebar.cs to update the number of collected and required feathers on the UI
    public delegate void DoorHandler(int numFeathers, int requiredFeathers);    ///< Event handler for UpdateDoor
    public static event DoorHandler UpdateDoor;                                 ///< Called in Door.cs, to possibly unlock the door
	public ScoreTextController scoreTextController;                             ///< Public reference to the game's score UI controller
    public Image screenFade;                                                    ///< A large black rectangle used to fade the screen in during the start of the level and out during a game over
    public Text timerText;                                                      ///< UI that tracks how long the player has been playing the level
    public int fadeTime = 45;                                                   ///< Duration in frames for how long the screen fades in or out
    public int totalFeathers;                                                   ///< Total number of feathers in the level
    public int requiredFeathers;                                                ///< Number of feathers required to collect to beat the level (8 required, 10 total)
    private float timer;                                                        ///< The time passed since the level started
    private int numFeathers;                                                    ///< The number of feathers Taekos has collected
    private int livesRemaining;                                                 ///< The number of lives Taekos has left
    private bool isGameOver;                                                    ///< Whether or not Taekos just lost his last life
    
    /// <summary>
    /// Initialize variables
    /// </summary>
    void Awake()
    {
        timer = 0f;
        numFeathers = 0;
        livesRemaining = Settings.NumberOfLives;
        isGameOver = false;
    }

    /// <summary>
    /// Subscribe to events
    /// </summary>
    void OnEnable()
    {
        Controller.getCurrentLives += getLives;
        Controller.getGameOver += getGameOver;
        Controller.AddLives += AddLives;
        CollectItems.addFeathers += AddFeathers;
        HitBox.completeLevel += CompleteLevel;
    }

    /// <summary>
    /// Unsubscribe to events
    /// </summary>
    void OnDisable()
    {
        Controller.getCurrentLives += getLives;
        Controller.getGameOver -= getGameOver;
        Controller.AddLives -= AddLives;
        CollectItems.addFeathers -= AddFeathers;
        HitBox.completeLevel -= CompleteLevel;
    }

	/// <summary>
    /// Fade the screen in and initialize the UI
    /// </summary>
	void Start () {
        StartCoroutine(ScreenFade());
		scoreTextController.UpdateScore (0);
        UpdateFeatherCounter(numFeathers, totalFeathers);
        UpdateDoor(numFeathers, requiredFeathers);
	}
	
	void Update () {
        float cseconds = timer - (int)timer;
        cseconds = Mathf.Round(cseconds * 100f) / 100f;
        cseconds *= 100f;
        if (cseconds >= 100f)
        {
            cseconds -= 100f;
        }
        string cseconds_s = cseconds.ToString();
        if (cseconds < 10f)
        {
            cseconds_s = "0" + cseconds_s;
        }

        float seconds = (int)timer % 60;
        string seconds_s = seconds.ToString();
        if (seconds < 10f)
        {
            seconds_s = "0" + seconds_s;
        }
        float minutes = (int) timer / 60;
        string minutes_s = minutes.ToString();
        if (minutes < 10f)
        {
            minutes_s = "0" + minutes_s;
        }
        timer += Time.deltaTime;
        timerText.text = "Time: " + minutes_s + "\"" + seconds_s + "\"" + cseconds_s;
	}

    /// <summary>
    /// Return the number of lives remaining
    /// </summary>
    int getLives()
    {
        return livesRemaining;
    }

    /// <summary>
    /// Returns whether or not Taekos has lost all lives
    /// </summary>
    bool getGameOver()
    {
        return isGameOver;
    }

    /// <summary>
    /// Called by CollectItems.addFeathers event,
    /// Update the feathers UI
    /// </summary>
    void AddFeathers(int add)
    {
        numFeathers += add;
        UpdateFeatherCounter(numFeathers, totalFeathers);
        UpdateDoor(numFeathers, requiredFeathers);
    }

    /// <summary>
    /// Called by Controller.AddLives event,
    /// Update the life counter whenever Taekos loses a life
    /// </summary>
    /// <param name="add"></param>
    void AddLives(int add)
    {
        livesRemaining += add;
        if (livesRemaining < 0)
        {
            livesRemaining = 0;
            isGameOver = true;
            StartCoroutine(GameOver());
        }
    }

    /// <summary>
    /// Coroutine called by AddLives if Taekos gets hit and runs out of lives
    /// </summary>
    /// <returns></returns>
    IEnumerator GameOver()
    {
        screenFade.enabled = true;
        float alpha = 0f;
        float dAlpha = 1f / (float)fadeTime;
        for (int i = 0; i < fadeTime; i++)
        {
            alpha += dAlpha;
            screenFade.color = new Color(1f, 1f, 1f, alpha);
            yield return 0;
        }
        SceneManager.LoadScene("GameOver");
    }

    /// <summary>
    /// Load the results screen when Taekos gets to the door
    /// </summary>
    void CompleteLevel()
    {
        if (numFeathers >= requiredFeathers)
        {
            Settings.Results.Score = scoreTextController.getScore();
            Settings.Results.Feathers = numFeathers;
            Settings.Results.Time = (int)(timer * 100);
            SceneManager.LoadScene("ResultsScreen");
        }
    }

    /// <summary>
    /// Coroutine to fade the screen in when starting the level
    /// </summary>
    public IEnumerator ScreenFade()
    {
        screenFade.rectTransform.position = new Vector3(400f, 300f, 0);
        float alpha = 1f;
        float dAlpha = 1f / (float)fadeTime;
        for (int i = 0; i < fadeTime; i++)
        {
            if (isGameOver)
            {
                break;
            }
            alpha -= dAlpha;
            screenFade.color = new Color(1f, 1f, 1f, alpha);
            yield return 0;
        }
        if (!isGameOver)
        screenFade.enabled = false;
    }
}
