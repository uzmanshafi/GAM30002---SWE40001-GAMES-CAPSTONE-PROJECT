using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FrogScript : MonoBehaviour
{

    private Rigidbody2D rb;
    private Collider2D c2D;

    private GameObject player;
    [SerializeField] private float agro_distance = 50;
    [SerializeField] private float jump_strength = 10;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        if (player == null)
        {
            throw new Exception("Player not found");
        }

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
        if (    (transform.position.x - player.transform.position.x) * (transform.position.x - player.transform.position.x) +
                (transform.position.y - player.transform.position.y) * (transform.position.y - player.transform.position.y) <=
                agro_distance * agro_distance)
        {
            return true;
        }
        return false;
    }

    void Move()
    {
        int player_direction = transform.position.x - player.transform.position.x > 0 ? -1 : 1;
        rb.velocity = new Vector2(jump_strength * player_direction, jump_strength);
    }

}
