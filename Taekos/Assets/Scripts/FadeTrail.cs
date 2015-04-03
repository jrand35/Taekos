using UnityEngine;
using System.Collections;

public class FadeTrail : MonoBehaviour {

	private int lifetime = 2;
	private float alpha = 1f;
	private float xscale;
	private float yscale;
	private float grow = 0.02f;
	private SpriteRenderer spriteRenderer;
	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
		xscale = transform.localScale.x;
		yscale = transform.localScale.y;
		StartCoroutine (Run ());
	}

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