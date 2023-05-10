using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    private Rigidbody rb;
    private Collider c2D;

    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float jumpHeight = 4.0f;
    [SerializeField] private Transform groundCheck;

    private float jumpCooldown = 0.1f; // The amount of time between jumps
    private float lastJumpTime = 0.0f; // The time of the last jump
    private bool isJumping = false;
    private float jumpTime = 0.0f;
    private float jumpVelocity;
    private float inputDirectionHorizontal;
    private bool isFacingRight = true;

    private bool isWalking = false; //flag for when to play certain animations

    [Header("Events")]
    [Space]
    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

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
        animator.SetFloat("speed", Mathf.Abs(inputDirectionHorizontal));
        if (Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            animator.SetBool("isJumping", true);
            isGrounded();
        }
    }

    private void applyMovement()
{
    if (inputDirectionHorizontal != 0)
    {
        // Apply movement in the desired direction
        rb.velocity = new Vector3(inputDirectionHorizontal * moveSpeed, rb.velocity.y, rb.velocity.z);
        rb.drag = 0;
    }
    else
    {
        // Reduce velocity gradually if the player is not trying to move
        Vector3 newVelocity = rb.velocity;
        newVelocity.x = Mathf.Lerp(rb.velocity.x, 0, Time.deltaTime * 10);
        rb.velocity = newVelocity;
        rb.drag = 0.5f;
    }
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

    public void isGrounded()
    {
        animator.SetBool("isJumping", false);

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
