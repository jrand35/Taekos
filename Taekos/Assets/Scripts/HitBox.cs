using UnityEngine;
using System.Collections;

public class HitBox : MonoBehaviour {
    public delegate void TakeDamageHandler(int damage);
    public static event TakeDamageHandler takeDamage;
    public delegate void KillPlayerHandler();
    public static event KillPlayerHandler killPlayer;
    public delegate void CheckpointHandler(Vector3 position, int checkpointIndex);
    public static event CheckpointHandler getCheckpoint;

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
            int damage = damageValue.getPlayerDamageValue();
            takeDamage(damage);
        }
    }
}