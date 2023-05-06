using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D rb;
    private Collider2D c2D;

    public float moveSpeed = 5.0f;
    public float jumpHeight = 10.0f;

    private float inputDirectionHorizontal;
    public bool isFacingRight = true;

    private bool isWalking = false; //flag for when to play certain animations
    //private bool isGrounded = false; //not used but can be added for animations if need be.

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        c2D = GetComponent<Collider2D>();

    }

    private void Awake()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        checkInput();
        pollMovementDirection();
    }

    private void FixedUpdate()
    {
        applyMovement();
      
    }

    private void checkInput()
    {
        inputDirectionHorizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    private void applyMovement()
    {
        rb.velocity = new Vector2(inputDirectionHorizontal * moveSpeed, rb.velocity.y);
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
    }

    private void pollMovementDirection()
    {
        if(isFacingRight && inputDirectionHorizontal < 0)
        {
            Flip();
        }
        else if(!isFacingRight && inputDirectionHorizontal > 0)
        {
            Flip();
        }

        if(rb.velocity.x != 0)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }

        
    }

    private void isGrounded()
    {
        if (c2D.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            Jump();
        }
    }
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }
}