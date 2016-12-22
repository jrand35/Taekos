using UnityEngine;
using System.Collections;

/// <summary>
/// Displays an after-image around Taekos when he collects a powerup. Not used in the final build
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class FadeTrail : MonoBehaviour {

	private float alpha = 1f;               ///< The current alpha value of the image, fades from 1 to 0 over time
	private float xscale;                   ///< The horizontal scale of the image
	private float yscale;                   ///< The vertical scale of the image
	private float grow = 0.02f;             ///< The scale growth rate
	private SpriteRenderer spriteRenderer;  ///< Reference to the sprite renderer
	
    /// <summary>
    /// Get a reference to the sprite renderer and start the Run coroutine
    /// </summary>
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
		xscale = transform.localScale.x;
		yscale = transform.localScale.y;
		StartCoroutine (Run ());
	}

    /// <summary>
    /// Start scaling the afterimage and fade it out
    /// </summary>
	IEnumerator Run(){
		while (alpha > 0) {
			alpha -= 0.02f;
			if (xscale > 0){
				xscale += grow;
			}
			else{
				xscale -= grow;
			}
			yscale += grow;
			spriteRenderer.color = new Color(1f, 0f, 0f, alpha);
			transform.localScale = new Vector3(xscale, yscale, 1f);
			yield return 0;
		}
		Destroy (gameObject);
	}
}