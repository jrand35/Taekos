using UnityEngine;
using System.Collections;

public class Pecking : MonoBehaviour {
    public int damageValue = 1;
    private bool enablePeck;
    private Controller controller;

    void Start()
    {
        controller = GetComponentInParent<Controller>();
        enablePeck = true;
    }
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