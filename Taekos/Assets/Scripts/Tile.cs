using UnityEngine;
using System.Collections;

/// <summary>
/// Stretches the texture of certain tile GameObjects
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class Tile : MonoBehaviour {
    
    /// <summary>
    /// Change the texture scale
    /// </summary>
	void Start () {
        transform.GetComponent<Renderer>().material.mainTextureScale = new Vector2(0.5f, 0.5f);
	}
}
