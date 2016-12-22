using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// The Game Over screen that appears when Taekos loses all lives
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class GameOverScreen : MonoBehaviour {
    public AudioSource continueClick;   ///< Sound to play when clicking the Continue button
    public Graphic[] fadingObjects; ///< The continue button, quit button, and remaining continues counter
	public Image gameOverImage;     ///< The Game Over UI text
    public Image fadeImage;         ///< A black rectangle for the background
    public Text remainingText;      ///< Displays the remaining number of continues
    private float delay = 4f;       ///< Delay in seconds before returning to the title screen, if there are no continues left

	/// <summary>
    /// Display the number of remaining continues and start the Run coroutine
    /// </summary>
    void Start()
    {
        //Settings.NumberOfContinues = 1;
        foreach (Graphic g in fadingObjects)
        {
            g.enabled = false;
            g.color = new Color(1f, 1f, 1f, 0f);
        }
        remainingText.text = "Remaining: " + Settings.NumberOfContinues;
		StartCoroutine (Run ());
	}

    /// <summary>
    /// Fade in the Game Over text, then display the buttons and Continues counter
    /// </summary>
	IEnumerator Run(){
		for (float i = 0f, alpha = 0f; i <= 1f; i += 0.005f, alpha += 0.005f) {
			float scale = 1f + 2f * (Mathf.Pow(i - 1, 2));
			gameOverImage.rectTransform.localScale = new Vector3 (scale, scale, 1f);
			gameOverImage.color = new Color(1f, 1f, 1f, alpha);
			yield return 0;
		}
        foreach (Graphic g in fadingObjects)
        {
            g.enabled = true;
        }
        if (Settings.NumberOfContinues > 0)
        {
            for (int i = 0; i <= 500; i += 5)
            {
                Color newColor = new Color(1f, 1f, 1f, (float)i / 500.0f);
                foreach (Graphic g in fadingObjects)
                {
                    g.color = newColor;
                }
                yield return 0;
            }
        }
        else
        {
            yield return new WaitForSeconds(delay);
            Quit();
        }
	}

    /// <summary>
    /// If the player clicks "Continue" then fade the screen out before loading the main level,
    /// Called by Continue coroutine
    /// </summary>
    IEnumerator FadeOut()
    {
        for (int i = 0; i <= 1000; i += 5)
        {
            fadeImage.rectTransform.localPosition = new Vector3(0f, 0f, 0f);
            fadeImage.color = new Color(1f, 1f, 1f, i / 1000.0f);
            yield return 0;
        }
        SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// When the player clicks Continue, turn off the buttons and call FadeOut coroutine
    /// </summary>
    public void Continue()
    {
        continueClick.Play();
        foreach (Graphic g in fadingObjects)
        {
            g.enabled = false;
        }
        Settings.NumberOfContinues--;
        remainingText.text = "Remaining: " + Settings.NumberOfContinues;
        StartCoroutine(FadeOut());
    }

    /// <summary>
    /// Quit the game and return to the title screen
    /// </summary>
    public void Quit()
    {
        SceneManager.LoadScene("Title");
    }
}