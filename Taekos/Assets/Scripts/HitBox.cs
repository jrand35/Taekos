using UnityEngine;
using System.Collections;
using System;

public class HitBox : MonoBehaviour {
    public delegate void TakeDamageHandler(int damage);
    public static event TakeDamageHandler takeDamage;
    public delegate void KillPlayerHandler();
    public static event KillPlayerHandler killPlayer;
    public delegate void CheckpointHandler(Vector3 position, int checkpointIndex);
    public static event CheckpointHandler getCheckpoint;
    public delegate void CompleteLevelHandler();
    public static event CompleteLevelHandler completeLevel;
    public Transform back_, top, front_, bottom;
    public LayerMask whatIsDoor;
    private float backx, backy, frontx, fronty;

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

    void OnCollisionEnter2D(Collision2D other)
    {
        //Kill the player instantly when they fall off the screen
        if (other.gameObject.tag == "KillPlayer")
        {
            killPlayer();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemies")
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
        else if (other.gameObject.tag == "Doors")
        {
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                completeLevel();
            }
        }
    }

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