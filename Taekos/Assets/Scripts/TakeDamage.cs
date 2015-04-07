using UnityEngine;
using System.Collections;

public class TakeDamage : MonoBehaviour {
    public delegate void TakeDamageHandler(int damage);
    public static event TakeDamageHandler takeDamage;

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