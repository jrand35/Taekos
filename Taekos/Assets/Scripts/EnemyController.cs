using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
    public int enemyLife = 3;
	private int playerDamageValue = 1;

	public int getPlayerDamageValue(){
		return playerDamageValue;
	}

    public void DamageEnemy(int damage){
        enemyLife -= damage;
        if (enemyLife <= 0){
            KillEnemy();
        }
    }

    void KillEnemy(){
        Destroy(gameObject);
    }
}
