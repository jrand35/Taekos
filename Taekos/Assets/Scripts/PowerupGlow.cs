using UnityEngine;
using System.Collections;

/// <summary>
/// Allow the mango to glow
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class PowerupGlow : MonoBehaviour {

    public float dRad = 0.1f;               ///< The rotation speed
    private SpriteRenderer spriteRenderer;

	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(Glow());
	}

    /// <summary>
    /// Allow the mango to glow along a factor between 0.5 and 1
    /// </summary>
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
