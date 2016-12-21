using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class ResultsScreen : MonoBehaviour {

    public Text scoreNumber;
    public Text feathersNumber;
    public Text timeNumber;
    private int delayFrames = 60;
    private long score;
    private int feathers;
    private int time;

    void Awake()
    {
        score = Settings.Results.Score;
        feathers = Settings.Results.Feathers;
        time = Settings.Results.Time;
    }

	// Use this for initialization
	void Start () {
        StartCoroutine(Run());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator Run()
    {
        int feathersBonus;
        int timeBonus = Mathf.Max(36000 - time, 0);
        scoreNumber.text = score.ToString();
        feathersNumber.text = feathers.ToString() + " x 200";
        timeNumber.text = timeBonus.ToString();
        for (int i = 0; i < delayFrames; i++)
        {
            yield return 0;
        }
        GetComponent<AudioSource>().Play();
        feathersBonus = 200 * Settings.Results.Feathers;
        feathersNumber.text = feathersBonus.ToString();
        for (int i = 0; i < delayFrames; i++)
        {
            yield return 0;
        }
        GetComponent<AudioSource>().Play();
        score += feathersBonus;
        scoreNumber.text = score.ToString();
        feathersNumber.text = "0";
        for (int i = 0; i < delayFrames; i++)
        {
            yield return 0;
        }
        GetComponent<AudioSource>().Play();
        score += timeBonus;
        scoreNumber.text = score.ToString();
        timeNumber.text = "0";
    }

    public void ReturnToTitle()
    {
        GetComponent<AudioSource>().Play();
        Application.LoadLevel("Title");
    }
}
