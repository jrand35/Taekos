using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
    public delegate void ScoreHandler(long addScore);
    public static event ScoreHandler addScore;
    public bool invincible = false;
    public int enemyLife = 3;
    public int enemyScore = 100;
    private bool enemyHurt;
    private int enemyHurtFrames = 10;
	private int playerDamageValue = 1;

    void Awake()
    {
        enemyHurt = false;
    }

	public int getPlayerDamageValue(){
		return playerDamageValue;
	}

    public bool getInvincible()
    {
        return invincible;
    }

    public bool EnemyDead()
    {
        return (enemyLife <= 0);
    }

    public void DamageEnemy(int damage, int direction){
        if (invincible)
            return;
        enemyLife -= damage;
        if (enemyLife <= 0){
            KillEnemy();
        }
        else
        {
            if (!enemyHurt)
            {
                StartCoroutine(HurtEnemy(direction));
            }
        }
    }

    void KillEnemy(){
        long longEnemyScore = (long)enemyScore;
        addScore(longEnemyScore);
        SendMessage("EnemyDeath");
    }

    IEnumerator HurtEnemy(int direction)
    {
        enemyHurt = true;
        Vector2 startVelocity = rigidbody2D.velocity;
        rigidbody2D.velocity = new Vector2(direction * 4f, 0f);
        for (int i = 0; i < enemyHurtFrames; i++)
        {
            yield return 0;
        }
        rigidbody2D.velocity = startVelocity;
        enemyHurt = false;
    }
}
