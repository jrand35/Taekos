using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class GameOverScreen : MonoBehaviour {

	public Image gameOverImage;

	// Use this for initialization
	void Start () {
		StartCoroutine (Run ());
	}

	IEnumerator Run(){
		for (float i = 0f, alpha = 0f; i <= 1f; i += 0.005f, alpha += 0.005f) {
			float scale = 1f + 2f * (Mathf.Pow(i - 1, 2));
			gameOverImage.rectTransform.localScale = new Vector3 (scale, scale, 1f);
			gameOverImage.color = new Color(1f, 1f, 1f, alpha);
			yield return 0;
		}
	}
}
