using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public ScoreTextController scoreTextController;
    public Image screenFade;
    public int fadeTime = 45;

	// Use this for initialization
	void Start () {
        StartCoroutine(ScreenFade());
		scoreTextController.UpdateScore (0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public IEnumerator ScreenFade()
    {
        screenFade.rectTransform.position = new Vector3(400f, 300f, 0);
        float alpha = 1f;
        float dAlpha = 1f / (float)fadeTime;
        for (int i = 0; i < fadeTime; i++)
        {
            alpha -= dAlpha;
            screenFade.color = new Color(1f, 1f, 1f, alpha);
            yield return 0;
        }
        screenFade.active = false;
    }
}
