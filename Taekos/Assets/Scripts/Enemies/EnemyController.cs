using UnityEngine;
using System.Collections;

/// <summary>
/// The enemy controller, attached to all enemies,
/// Determines how many points are added to the score after being killed,
/// Start coroutines for getting hurt
/// <remarks>
/// By Joshua Rand
/// </remarks>
/// </summary>
public class EnemyController : MonoBehaviour {
    public delegate void ScoreHandler(long addScore);
    public static event ScoreHandler addScore;          ///< Message sent to ScoreTextController when an enemy is killed
    public bool invincible = false;                     ///< Make the enemy invincible or not
    public bool moveBack = true;                        ///< Give the enemy knockback or not
    public int enemyLife = 3;                           ///< How many hits the enemy can take
    public int enemyScore = 100;                        ///< How many points added to the score when the enemy is killed
    private bool enemyHurt;                             ///< Set to true after being hit, gives the enemy temporary invincibility
    private int enemyHurtFrames = 10;                   ///< Duration of the enemy hurt state
	private int playerDamageValue = 1;                  ///< How many points the player lifebar loses

    /// <summary>
    /// Initialize values
    /// </summary>
    void Awake()
    {
        enemyHurt = false;
    }

    /// <summary>
    /// Return playerDamageValue
    /// </summary>
	public int getPlayerDamageValue(){
		return playerDamageValue;
	}

    /// <summary>
    /// Return invincible
    /// </summary>
    public bool getInvincible()
    {
        return invincible;
    }

    /// <summary>
    /// Return true if the enemy is dead
    /// </summary>
    public bool EnemyDead()
    {
        return (enemyLife <= 0);
    }

    /// <summary>
    /// Hurt the enemy, called by Banana and Pecking scripts
    /// </summary>
    public void DamageEnemy(int direction)
    {
        if (invincible)
            return;
        enemyLife -= 1;
        if (enemyLife <= 0)
        {
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

    /// <summary>
    /// Hurt the enemy and give it a specific damage number
    /// </summary>
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

    /// <summary>
    /// Kill the enemy
    /// </summary>
    void KillEnemy(){
        long longEnemyScore = (long)enemyScore;
        addScore(longEnemyScore);
        SendMessage("EnemyDeath");
    }

    /// <summary>
    /// Coroutine for the hit state
    /// </summary>
    IEnumerator HurtEnemy(int direction)
    {
        Vector2 startVelocity = new Vector2();
        enemyHurt = true;
        if (moveBack)
        {
            startVelocity = GetComponent<Rigidbody2D>().velocity;
            GetComponent<Rigidbody2D>().velocity = new Vector2(direction * 4f, 0f);
        }
        for (int i = 0; i < enemyHurtFrames; i++)
        {
            yield return 0;
        }
        if (moveBack)
            GetComponent<Rigidbody2D>().velocity = startVelocity;
        enemyHurt = false;
    }
}
