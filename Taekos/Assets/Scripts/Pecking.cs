using UnityEngine;
using System.Collections;

public class Pecking : MonoBehaviour {
    public int damageValue = 1;
    private bool enablePeck;
    void Start()
    {
        enablePeck = false;
    }
	void OnTriggerEnter2D(Collider2D other){
        if (enablePeck){
            if (other.gameObject.tag == "Enemy")
            {
                EnemyController enemyController = other.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    enemyController.DamageEnemy(damageValue);
                }
            }
        }
    }

    public void EnablePeck(bool peck){
        enablePeck = peck;
    }
}