using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public int damage;
    public GameObject impact;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        /*
        //Collider2D other = collision.collider;
        if (other.CompareTag("Enemy"))
        {
            // Deal damage to the enemy or destroy it
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        if (other.CompareTag("Breakable"))
        {
            // Deal damage to the enemy or destroy it
            BreakableObject breakable = other.GetComponent<BreakableObject>();
            if (breakable != null)
            {
                breakable.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        */
        if (other.CompareTag("Player")){

        }
        else if (other.CompareTag("PlayerProjectile"))
        {

        }
        else
        {
            if (other.CompareTag("FrogKing"))
            {
                // Deal damage to the enemy or destroy it
                FrogKingAI boss = other.GetComponent<FrogKingAI>();
                if (boss != null)
                {
                    boss.takeDamage(damage);
                }
            }
            Instantiate(impact, transform.position, Quaternion.identity);
            Destroy(gameObject);
        } 
    }
}
