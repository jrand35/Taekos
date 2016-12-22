using UnityEngine;
using System.Collections;

/// <summary>
/// Bonus points that appear over Taekos if he collects a powerup, not used in the final build
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class BonusPoints : MonoBehaviour {
	
	public Sprite[] spr;                    ///< 100, 200, 300, etc. text sprites
	private SpriteRenderer spriteRenderer;  ///< The GameObject's sprite renderer
	private float fadeDelay = 0.7f;         ///< The delay in seconds before the sprite should start fading out
	private long points = 100;              ///< The number of points added to the score, originally indended to be variable

	/// <summary>
    /// Set the GameObject to move slowly upward, get a reference to the sprite renderer, start the Fade coroutine
    /// </summary>
	void Start () {
		GetComponent<Rigidbody2D>().velocity = new Vector2 (0f, 1f);
		spriteRenderer = GetComponent<SpriteRenderer> ();
		SetSprite ();
		StartCoroutine (Fade ());
	}

    /// <summary>
    /// Set the value of points variable
    /// </summary>
	public void SetPoints(long pts){
		points = pts;
	}

    /// <summary>
    /// Set the sprite according to the points value
    /// </summary>
	public void SetSprite(){
		int spriteIndex = 0;
		spriteIndex = (int)(points / 100) - 1;
		spriteRenderer.sprite = spr[spriteIndex];
	}

    /// <summary>
    /// Wait 0.75 seconds, then fade out the sprite, then destroy the GameObject
    /// </summary>
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