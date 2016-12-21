using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	// Use this for initialization
	void Start () {
        transform.GetComponent<Renderer>().material.mainTextureScale = new Vector2(0.5f, 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
