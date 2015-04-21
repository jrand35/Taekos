using UnityEngine;
using System.Collections;

public class DetectPlayer : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            SendMessageUpwards("Attack", true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            SendMessageUpwards("Attack", false);
        }
    }
}
