using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// A hitbox attached to Taekos,
/// Handles ground detection and collisions between enemies and checkpoints
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class HitBox : MonoBehaviour {
    public delegate void TakeDamageHandler(int damage);                             ///< Event handler for taking damage
    public static event TakeDamageHandler takeDamage;                               ///< Event for taking damage, called when colliding with an enemy
    public delegate void KillPlayerHandler();                                       ///< Event handler for killing the player
    public static event KillPlayerHandler killPlayer;                               ///< Event for killing the player, called when touching a death boundary at the bottom of the screen
    public delegate void CheckpointHandler(Vector3 position, int checkpointIndex);  ///< Event handler for getting checkpoints
    public static event CheckpointHandler getCheckpoint;                            ///< Event for getting a checkpoint, called when colliding with a checkpoint
    public delegate void CompleteLevelHandler();                                    ///< Event handler for completing the level
    public static event CompleteLevelHandler completeLevel;                         ///< Event for completing the level, called when the player presses down in front of a door
    public Transform back_, top, front_, bottom;                                    ///< Used for checking Physics overlapping with a door
    public LayerMask whatIsDoor;                                                    ///< Reference to the door's layer
    private float backx, backy, frontx, fronty;                                     ///< Positions of back_, top, front_, and bottom                     

    ///<summary>
    /// Fire the getCheckpoint event when getting a checkpoint,
    /// Message sent to Controller.cs
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Checkpoint")
        {
            Vector3 checkpointPos = other.gameObject.transform.position;
            Checkpoint script = other.GetComponent<Checkpoint>();
            int checkpointIndex = script.getCheckpointIndex();
            script.getCheckpoint();
            getCheckpoint(checkpointPos, checkpointIndex);
        }
    }

    /// <summary>
    /// Detect collision with death boundary at the bottom of the screen
    /// </summary>
    void OnCollisionEnter2D(Collision2D other)
    {
        //Kill the player instantly when they fall off the screen
        if (other.gameObject.tag == "KillPlayer")
        {
            killPlayer();
        }
    }

    /// <summary>
    /// Handle collisions with enemies and projectiles,
    /// Fire the takeDamage event, sending a message to Controller.cs
    /// </summary>
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemies" || other.gameObject.tag == "Enemy Projectiles")
        {
            EnemyController damageValue = other.gameObject.GetComponent<EnemyController>();
            if (damageValue != null)
            {
                if (!damageValue.EnemyDead())
                {
                    int damage = damageValue.getPlayerDamageValue();
                    takeDamage(damage);
                }
            }
            else
            {
                //Default to taking 1 damage
                takeDamage(1);
            }
        }
/*        else if (other.gameObject.tag == "Doors")
        {
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                completeLevel();
            }
        }*/
    }

    /// <summary>
    /// Complete the level if the player presses S or Down in front of a door
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            backx = back_.position.x;
            backy = bottom.position.y;
            frontx = front_.position.x;
            fronty = top.position.y;
            Vector2 back = new Vector2(backx, backy);
            Vector2 front = new Vector2(frontx, fronty);
            if (Physics2D.OverlapArea(back, front, whatIsDoor))
            {
                completeLevel();
            }
        }
    }
}