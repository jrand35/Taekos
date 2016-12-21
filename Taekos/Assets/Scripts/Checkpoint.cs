using UnityEngine;
using System.Collections;

/// <summary>
/// Checkpoint class. If Taekos gets a checkpoint, the next time he loses a life, he will respawn at the checkpoint's location
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class Checkpoint : MonoBehaviour {
    public int checkpointIndex; ///< The order of the checkpoints. If Taekos gets checkpoint 2, he cannot get checkpoint 1
    private SpriteRenderer sr;  ///< Reference to the sprite renderer
    private Color normal;       ///< Color of the checkpoint before it's hit
    private Color got;          ///< Color of the checkpoint after it's hit

    /// <summary>
    /// Initialize the colors and get a reference to the sprite renderer
    /// </summary>
    void Start()
    {
        normal = new Color(0.25f, 0.25f, 0.25f, 1f);
        got = new Color(1f, 1f, 1f, 1f);
        sr = GetComponent<SpriteRenderer>();
        sr.color = normal;
    }

    /// <summary>
    /// Return checkpointIndex
    /// </summary>
    public int getCheckpointIndex(){
        return checkpointIndex;
    }

    /// <summary>
    /// Called by HitBox when Taekos gets the checkpoint
    /// </summary>
    public void getCheckpoint()
    {
        sr.color = got;
    }
}
