using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    public delegate void FeatherHandler(int numFeathers, int maxFeathers);
    public static event FeatherHandler UpdateFeatherCounter;
	public ScoreTextController scoreTextController;
    public Image screenFade;
    public int fadeTime = 45;
    public int totalFeathers = 20;
    private int numFeathers;
    private int livesRemaining;
    private bool isGameOver;

    void Awake()
    {
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
    }

    void OnDisable()
    {
        Controller.getCurrentLives += getLives;
        Controller.getGameOver -= getGameOver;
        Controller.AddLives -= AddLives;
        CollectItems.addFeathers -= AddFeathers;
    }

	// Use this for initialization
	void Start () {
        StartCoroutine(ScreenFade());
		scoreTextController.UpdateScore (0);
        UpdateFeatherCounter(numFeathers, totalFeathers);
	}
	
	// Update is called once per frame
	void Update () {
	
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
        Debug.Log("Game Over");
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
