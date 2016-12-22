using UnityEngine;
using System.Collections;

/// <summary>
/// Pecking script, allows Taekos to peck enemies
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class Pecking : MonoBehaviour {
    public int damageValue = 2;     ///< How many hits of damage pecking inflicts
    private bool enablePeck;
    private Controller controller;  ///< Reference to the Controller script

    void Start()
    {
        controller = GetComponentInParent<Controller>();
        enablePeck = true;
    }
    /// <summary>
    /// Hit enemies and play the pecking sound
    /// </summary>
	void OnTriggerEnter2D(Collider2D other){
        if (enablePeck){
            if (other.gameObject.tag == "Enemies")
            {
                EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    if (enemyController.getInvincible() || enemyController.EnemyDead())
                    {

                    }
                    else
                    {
                        SendMessageUpwards("PlayPeckSound");
                        enemyController.DamageEnemy(damageValue, controller.getFacing());
                    }
                }
            }
        }
    }
}