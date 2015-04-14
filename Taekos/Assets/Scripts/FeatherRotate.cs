using UnityEngine;
using System.Collections;

public class FeatherRotate : MonoBehaviour {

    private float dRad = 0.08f;

	// Use this for initialization
	void Start () {
        StartCoroutine(Rotate());
	}

    IEnumerator Rotate()
    {
        float rad = 0f;
        while (gameObject != null)
        {
            rad += dRad;
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
