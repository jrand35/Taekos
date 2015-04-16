using UnityEngine;
using System.Collections;

public class PowerupGlow : MonoBehaviour {

    public float dRad = 0.1f;
    private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(Glow());
	}

    IEnumerator Glow()
    {
        float rad = 0f;
        while (gameObject != null) {
            rad += dRad;
            if (rad > (2f * Mathf.PI))
            {
                rad -= (2f * Mathf.PI);
            }
            float glow = 0.75f + (0.25f * Mathf.Cos(rad));
            Color newColor = new Color(1f, 1f, 1f, glow);
            spriteRenderer.color = newColor;
            yield return 0;
        }
    }
}
