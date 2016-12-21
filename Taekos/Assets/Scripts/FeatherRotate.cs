using UnityEngine;
using System.Collections;

/// <summary>
/// Attached to the feather prefab. Pseudo-rotates the feather along a vertical axis using a cosine function
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class FeatherRotate : MonoBehaviour {

    private float dRad = 0.08f; ///< The difference in rotation angle, in radians

	void Start () {
        StartCoroutine(Rotate());
	}

    /// <summary>
    /// Rotates the feather each frame with a cosine function
    /// </summary>
    IEnumerator Rotate()
    {
        float rad = 0f;
        while (gameObject != null)
        {
            //rad += dRad;
            rad += dRad * Time.deltaTime * 50f;
            if (rad > (2f * Mathf.PI))
            {
                rad -= (2f * Mathf.PI);
            }
            float scale = Mathf.Cos(rad);
            Vector3 newScale = transform.localScale;
            newScale.x = scale;
            transform.localScale = newScale;
            yield return 0;
        }
    }
}
