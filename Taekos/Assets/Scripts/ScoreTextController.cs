using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class ScoreTextController : MonoBehaviour {

	public Image[] scoreDigits;
	public Sprite[] scoreNumbers;
	private long score;

    void Awake()
    {
        score = 0;
    }

    void OnEnable()
    {
        CollectItems.addScore += UpdateScore;
        EnemyController.addScore += UpdateScore;
    }

    void OnDisable()
    {
        CollectItems.addScore -= UpdateScore;
        EnemyController.addScore -= UpdateScore;
    }

	// Use this for initialization
	void Start () {
        //score not setting to this value
		for (int i = 0; i < 10; i++) {
			Vector3 digitPosition = new Vector3(-250f + (i * 25), 0f, 0f);
			scoreDigits[i].rectTransform.position = transform.position + digitPosition;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	//Far left: / 10^10
	//Far right: %10
	public void UpdateScore(long addScore) {
		score += addScore;
		for (int i = 0; i < 10; i++){
			long digit = score / (int)(Mathf.Pow (10, 9 - i)) % 10 ;
			scoreDigits[i].sprite = scoreNumbers[digit];
		}
	}

    public long getScore()
    {
        return score;
    }
}