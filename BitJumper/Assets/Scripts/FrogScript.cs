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
    [SerializeField] private float jump_strength = 10;
    [SerializeField] private Transform groundCheck;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        if (player == null) {
            throw new Exception("Player not found");
        }
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPlayerNear())
        {
            //Add check so that it can only jump if on the ground
            Move();
        }
    }


    bool IsPlayerNear()
    {
        return (    (transform.position.x - player.transform.position.x) * (transform.position.x - player.transform.position.x) +
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

        //divide scale by 2
        if (Physics.CheckBox(groundCheck.position, groundCheck.localScale / 2, Quaternion.identity, LayerMask.GetMask("Ground"), QueryTriggerInteraction.UseGlobal)) {
        
            int player_direction = transform.position.x - player.transform.position.x > 0 ? -1 : 1;

            if (isFacingRight != (player_direction == 1)) {
                Flip();
            }

            //2 / Physics.gravity.y = t
            float vx = (transform.position.x - player.transform.position.x) / (2 * jump_strength / Physics.gravity.y);
            float vy = jump_strength;
            //-(Physics.gravity.y / 2) * (player.transform.position.x - transform.position.x) / vx;

            rb.velocity = new Vector3(vx, vy, 0);

        }
        
        // if (grounded.IsGrounded) {
        // Debug.Log("Help");
        //     transform.position = new Vector3(transform.position.x + player_direction * Time.deltaTime, 10, transform.position.z);
        // }
        
        //rb.velocity = new Vector3(jump_strength * player_direction, jump_strength, 0);
    }

}
