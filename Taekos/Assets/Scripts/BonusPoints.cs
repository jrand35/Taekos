using UnityEngine;
using System.Collections;

public class BonusPoints : MonoBehaviour {
	
	public Sprite[] spr;
	private SpriteRenderer spriteRenderer;
	private float fadeDelay = 0.7f;
	private long points = 100;

	// Use this for initialization
	void Start () {
		rigidbody2D.velocity = new Vector2 (0f, 1f);
		spriteRenderer = GetComponent<SpriteRenderer> ();
		SetSprite ();
		StartCoroutine (Fade ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetPoints(long pts){
		points = pts;
	}

	public void SetSprite(){
		int spriteIndex = 0;
		spriteIndex = (int)(points / 100) - 1;
		spriteRenderer.sprite = spr[spriteIndex];
	}

	public IEnumerator Fade(){
		float alpha = 1f;
		yield return new WaitForSeconds (fadeDelay);
		while (alpha > 0) {
			alpha -= 0.02f;
			spriteRenderer.color = new Color (1f, 1f, 1f, alpha);
			yield return 0;
		}
		Destroy (gameObject);
	}
}