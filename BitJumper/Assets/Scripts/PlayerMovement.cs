using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Collider c2D;

    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float jumpHeight = 4.0f;
    [SerializeField] private Transform groundCheck;

    private float jumpCooldown = 0.1f; // The amount of time between jumps
    private float lastJumpTime = 0.0f; // The time of the last jump
    private bool isJumping = false;
    private float jumpTime = 0.0f;
    private float maxJumpTime = 0.3f;
    private float jumpVelocity;
    private float inputDirectionHorizontal;
    private bool isFacingRight = true;

    private bool isWalking = false; //flag for when to play certain animations
    //private bool isGrounded = false; //not used but can be added for animations if need be.

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        c2D = GetComponent<Collider>();
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
            isGrounded();
        }
    }

    private void applyMovement()
    {
        rb.velocity = new Vector3(inputDirectionHorizontal * moveSpeed, rb.velocity.y, rb.velocity.z);
    }

    private void Jump()
    {
        float jumpVelocity = Mathf.Sqrt(2 * jumpHeight * Physics.gravity.magnitude);
        rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
    }

    private void pollMovementDirection()
    {
        if (IsFacingRight() && inputDirectionHorizontal < 0)
        {
            Flip();
        }
        else if (!IsFacingRight() && inputDirectionHorizontal > 0)
        {
            Flip();
        }

        if (rb.velocity.x != 0)
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
        if (Time.time - lastJumpTime >= jumpCooldown && Physics.CheckSphere(groundCheck.position, 0.1f, (LayerMask.GetMask("Ground"))))
        {
            Jump();
            lastJumpTime = Time.time;
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    public bool IsFacingRight()
    {
        return isFacingRight;
    }
}
