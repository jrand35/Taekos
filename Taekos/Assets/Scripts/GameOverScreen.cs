using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class GameOverScreen : MonoBehaviour {

    public Graphic[] fadingObjects;
	public Image gameOverImage;
    public Image fadeImage;
    public Text remainingText;
    private float delay = 3f;

	// Use this for initialization
    void Start()
    {
        Settings.NumberOfContinues = 1;
        foreach (Graphic g in fadingObjects)
        {
            g.enabled = false;
            g.color = new Color(1f, 1f, 1f, 0f);
        }
        remainingText.text = "Remaining: " + Settings.NumberOfContinues;
		StartCoroutine (Run ());
	}

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
        }
	}

    IEnumerator FadeOut()
    {
        for (int i = 0; i <= 1000; i += 5)
        {
            fadeImage.rectTransform.localPosition = new Vector3(0f, 0f, 0f);
            fadeImage.color = new Color(1f, 1f, 1f, i / 1000.0f);
            yield return 0;
        }
    }

    public void Continue()
    {
        Application.LoadLevel("Main");
        foreach (Graphic g in fadingObjects)
        {
            g.enabled = false;
        }
        Settings.NumberOfContinues--;
        remainingText.text = "Remaining: " + Settings.NumberOfContinues;
        StartCoroutine(FadeOut());
    }
}