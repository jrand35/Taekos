using UnityEngine;
using System.Collections;

public class Pecking : MonoBehaviour {
    public int damageValue = 1;
    private bool enablePeck;
    void Start()
    {
        Debug.Log(transform.localPosition);
        enablePeck = true;
    }
	void OnTriggerEnter2D(Collider2D other){
        if (enablePeck){
            if (other.gameObject.tag == "Enemies")
            {
                EnemyController enemyController = other.gameObject.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    enemyController.DamageEnemy(damageValue);   //Fix; Kills enemies instantly
                }
            }
        }
    }

    //public void EnablePeck(bool peck){
        //enablePeck = peck;
    //}
}