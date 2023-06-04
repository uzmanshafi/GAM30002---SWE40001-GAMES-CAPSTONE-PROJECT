using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FrogScript : MonoBehaviour
{

    private GameObject player;
    private Rigidbody rb;
    private float agro_distance = 10;
    private bool isFacingRight = false;
    [SerializeField] int HP = 1;
    [SerializeField] private float jump_strength = 2;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float damageAmount = 3.8f / 3;  // The amount of damage the frog does to the player
    [SerializeField] private float damageInterval = 0.5f;  // Time in seconds between damage instances

    private float lastDamageTime;  // Time when last damage was dealt

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        if (player == null) {
            throw new Exception("Player not found");
        }
        rb = gameObject.GetComponent<Rigidbody>();
        lastDamageTime = -damageInterval;  // Initialize to ensure damage can be dealt immediately
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPlayerNear() && IsGrounded())
        {
            //Add check so that it can only jump if on the ground
            Move();
        }
    }


    bool IsPlayerNear()
    {
        return ((transform.position.x - player.transform.position.x) * (transform.position.x - player.transform.position.x) +
                (transform.position.y - player.transform.position.y) * (transform.position.y - player.transform.position.y) <=
                agro_distance * agro_distance);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    void Move()
    {
        int player_direction = transform.position.x - player.transform.position.x > 0 ? -1 : 1;
        
            if (isFacingRight != (player_direction == 1)) {
                Flip();
            }

        rb.velocity = new Vector3(jump_strength * player_direction, jump_strength, 0);

    }
        
        // if (grounded.IsGrounded) {
        // Debug.Log("Help");
        //     transform.position = new Vector3(transform.position.x + player_direction * Time.deltaTime, 10, transform.position.z);
        // }
        
        //rb.velocity = new Vector3(jump_strength * player_direction, jump_strength, 0);

    // OnCollisionStay is called once per frame for every collider/rigidbody that is touching rigidbody/collider
    // OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider
    bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, 0.1f, LayerMask.GetMask("Ground"));
    }
    void OnCollisionEnter(Collision collision)
    {
        DamagePlayer(collision.gameObject);
    }

    void OnCollisionStay(Collision collision)
    {
        DamagePlayer(collision.gameObject);
    }

    void DamagePlayer(GameObject collidedObject)
    {
        // Check if the object we collided with is the player
        if (collidedObject == player.gameObject)
        {
            // Check if enough time has passed since last damage
            if (Time.time - lastDamageTime >= damageInterval)
            {
                // Get the HealthController component of the player and make it take damage
                HealthController playerHealthController = player.gameObject.GetComponent<HealthController>();
                if (playerHealthController != null)
                {
                    playerHealthController.TakeDamage(damageAmount);
                    lastDamageTime = Time.time;  // Update the last damage time
                }
            }
        }
    }

    public void takeDamage(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            die();
        }
    }

    private void die()
    {
        Destroy(gameObject);
    }

}
