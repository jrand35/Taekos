using UnityEngine.UI;
using UnityEngine;
using System.Collections;

/// <summary>
/// The UI for the score in the top-right corner,
/// The score increases when Taekos kills enemies
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class ScoreTextController : MonoBehaviour {

	public Image[] scoreDigits;     ///< The UI for each number on the screen
	public Sprite[] scoreNumbers;   ///< Numbers from 0-9
	private long score;             ///< The score. Used as a long int because the score can be as high as 10 digits

    /// <summary>
    /// Set the score to 0 at the start of the game
    /// </summary>
    void Awake()
    {
        score = 0;
    }

    /// <summary>
    /// Subscribe to events
    /// </summary>
    void OnEnable()
    {
        CollectItems.addScore += UpdateScore;
        EnemyController.addScore += UpdateScore;
    }

    /// <summary>
    /// Unsubscribe to events
    /// </summary>
    void OnDisable()
    {
        CollectItems.addScore -= UpdateScore;
        EnemyController.addScore -= UpdateScore;
    }

	void Start () {
		for (int i = 0; i < 10; i++) {
			Vector3 digitPosition = new Vector3(-250f + (i * 25), 0f, 0f);
			scoreDigits[i].rectTransform.position = transform.position + digitPosition;
		}
	}
	
    ///<summary>
    /// Update the score UI
    /// Far left digit: / 10^10
    /// Far right digit: %10
    /// </summary>
	public void UpdateScore(long addScore) {
		score += addScore;
		for (int i = 0; i < 10; i++){
			long digit = score / (int)(Mathf.Pow (10, 9 - i)) % 10 ;
			scoreDigits[i].sprite = scoreNumbers[digit];
		}
	}

    /// <summary>
    /// Return the score
    /// </summary>
    public long getScore()
    {
        return score;
    }
}