using UnityEngine;
using System.Collections;

/// <summary>
/// Attached to Litholite enemies,
/// Detects if the player is in front of it, sends a message upwards
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class DetectPlayer : MonoBehaviour {

    /// <summary>
    /// When the player touches the boundary
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            SendMessageUpwards("Attack", true);
        }
    }

    /// <summary>
    /// When the player leaves the boundary
    /// </summary>
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            SendMessageUpwards("Attack", false);
        }
    }
}
