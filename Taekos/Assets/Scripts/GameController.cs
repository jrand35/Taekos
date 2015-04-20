using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    public delegate void FeatherHandler(int numFeathers, int maxFeathers);
    public static event FeatherHandler UpdateFeatherCounter;
    public delegate void DoorHandler(int numFeathers, int requiredFeathers);
    public static event DoorHandler UpdateDoor;
	public ScoreTextController scoreTextController;
    public Image screenFade;
    public Text timerText;
    public int fadeTime = 45;
    public int totalFeathers = 20;
    public int requiredFeathers = 1;
    private float timer;
    private int numFeathers;
    private int livesRemaining;
    private bool isGameOver;

    void Awake()
    {
        timer = 0f;
        numFeathers = 0;
        livesRemaining = Settings.NumberOfLives;
        isGameOver = false;
    }

    void OnEnable()
    {
        Controller.getCurrentLives += getLives;
        Controller.getGameOver += getGameOver;
        Controller.AddLives += AddLives;
        CollectItems.addFeathers += AddFeathers;
        HitBox.completeLevel += CompleteLevel;
    }

    void OnDisable()
    {
        Controller.getCurrentLives += getLives;
        Controller.getGameOver -= getGameOver;
        Controller.AddLives -= AddLives;
        CollectItems.addFeathers -= AddFeathers;
        HitBox.completeLevel -= CompleteLevel;
    }

	// Use this for initialization
	void Start () {
        StartCoroutine(ScreenFade());
		scoreTextController.UpdateScore (0);
        UpdateFeatherCounter(numFeathers, totalFeathers);
        UpdateDoor(numFeathers, requiredFeathers);
	}
	
	// Update is called once per frame
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

    int getLives()
    {
        return livesRemaining;
    }

    bool getGameOver()
    {
        return isGameOver;
    }

    void AddFeathers(int add)
    {
        numFeathers += add;
        UpdateFeatherCounter(numFeathers, totalFeathers);
        UpdateDoor(numFeathers, requiredFeathers);
    }

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

    IEnumerator GameOver()
    {
        screenFade.active = true;
        float alpha = 0f;
        float dAlpha = 1f / (float)fadeTime;
        for (int i = 0; i < fadeTime; i++)
        {
            alpha += dAlpha;
            screenFade.color = new Color(1f, 1f, 1f, alpha);
            yield return 0;
        }
        Application.LoadLevel("GameOver");
    }

    void CompleteLevel()
    {
        if (numFeathers >= requiredFeathers)
        {
            Settings.Results.Score = scoreTextController.getScore();
            Settings.Results.Feathers = numFeathers;
            Settings.Results.Time = (int)(timer * 100);
            Application.LoadLevel("ResultsScreen");
        }
    }

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
        screenFade.active = false;
    }
}
