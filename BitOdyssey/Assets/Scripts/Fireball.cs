using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public int damage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D other = collision.collider;
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
    }
}
