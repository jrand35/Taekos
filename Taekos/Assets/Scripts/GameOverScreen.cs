using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class GameOverScreen : MonoBehaviour {

    public Button continueButton;
    public Button quitButton;
	public Image gameOverImage;

	// Use this for initialization
    void Start()
    {
        continueButton.enabled = false;
        quitButton.enabled = false;
        continueButton.image.color = new Color(1f, 1f, 1f, 0f);
        quitButton.image.color = new Color(1f, 1f, 1f, 0f);

		StartCoroutine (Run ());
	}

	IEnumerator Run(){
		for (float i = 0f, alpha = 0f; i <= 1f; i += 0.005f, alpha += 0.005f) {
			float scale = 1f + 2f * (Mathf.Pow(i - 1, 2));
			gameOverImage.rectTransform.localScale = new Vector3 (scale, scale, 1f);
			gameOverImage.color = new Color(1f, 1f, 1f, alpha);
			yield return 0;
		}
        continueButton.enabled = true;
        quitButton.enabled = true;
        for (int i = 0; i <= 500; i += 5)
        {
            Color newColor = new Color(1f, 1f, 1f, (float)i / 500.0f);
            continueButton.image.color = newColor;
            quitButton.image.color = newColor;
            yield return 0;
        }
	}
}
