using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    public float speed = 6.0f; 
    public float jumpForce = 5.8f; 
    public float fallMultiplier = 2.5f; 
    public float lowJumpMultiplier = 2f; 
    public Transform groundCheck; 
    public float checkRadius = 0.3f; 
    public LayerMask whatIsGround; 
    public float coyoteTime = 0.2f; 

    public float acceleration = 30f;  // Set your desired acceleration
    public float maxSpeed = 10f;  // Set your maximum speed
    private bool isGrounded; 
    private float coyoteCounter; 
    private Rigidbody rb; 
    private float jumpTimeCounter;
    private bool isJumping;

    private float moveInput;
    // animation reference
    private Animator anim;

    private enum MovementState
    {
        Idle,
        Running,
        Jumping,
        Falling
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, checkRadius, whatIsGround); 

        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.fixedDeltaTime;
        }
    }

    void Update()
    {
        moveInput = Input.GetAxis("Horizontal");

        rb.velocity = new Vector3(moveInput * speed, rb.velocity.y, 0);

        if (Mathf.Abs(rb.velocity.x) < maxSpeed)
        {
            rb.AddForce(new Vector3(moveInput * acceleration, 0, 0), ForceMode.Acceleration);
        }

        // Rotate the sprite based on the direction
        if (moveInput < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);  // Rotate to face left
        }
        else if (moveInput > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);  // Rotate to face right
        }

        if (Input.GetButtonDown("Jump") && coyoteCounter > 0)
        {
            isJumping = true;
            jumpTimeCounter = coyoteTime;
            rb.velocity = Vector3.up * jumpForce;
            coyoteCounter = 0; 
        }

        if (Input.GetButton("Jump") && isJumping == true)
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = Vector3.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        UpdatePlayerAnimation();
    }

    public void UpdatePlayerAnimation()
    {
        MovementState movementState;
        if (moveInput != 0)
        {
            movementState = MovementState.Running;
        }
        else
        {
            movementState = MovementState.Idle;
        }

        if (rb.velocity.y > .1f)
        {
            movementState = MovementState.Jumping;
        }
        else if (rb.velocity.y < -.1f && !isGrounded && coyoteCounter < 0f)
        {
            movementState = MovementState.Falling;
        }

        anim.SetInteger("MovementState", (int)movementState);
    }
}