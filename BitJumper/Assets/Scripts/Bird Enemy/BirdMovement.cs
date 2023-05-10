using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMovement : MonoBehaviour
{ 
    public Transform player;
    [SerializeField] float moveSpeed = 2f;
    private Rigidbody2D rb;
    Transform target;
    Vector2 moveDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            Vector3 direction = target.position - transform.position;
            moveDirection = direction;
            //rb.velocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
        }

    }

    void FixedUpdate()
    {
        if (target)
        { 
            rb.velocity = new Vector2(moveDirection.x, moveDirection.y) * moveSpeed;
        }
    }
}
