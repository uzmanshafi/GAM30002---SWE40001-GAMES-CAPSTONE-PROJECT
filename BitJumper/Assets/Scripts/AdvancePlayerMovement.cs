using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AdvancePlayerMovement : MonoBehaviour
{
    public Animator animator;
    public float moveSpeed = 5.0f;
    public float jumpHeight = 4.0f;
    public float coyoteTimeDuration = 0.15f;
    public float jumpCutDuration = 0.2f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2.0f;
    public bool Is3D = false;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public SphereCollider groundCheckCollider;

    private Rigidbody rb;
    private bool isFacingRight = true;
    private bool isGrounded;
    private float lastGroundedTime;
    private float lastJumpTime;
    private bool isJumpCutAllowed;

    [Header("Events")]
    [Space]
    public UnityEvent OnJumpEvent;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleMovementInput();
        HandleJumpInput();
        AdjustFallGravity();
        FlipCharacter();
    }

    void FixedUpdate()
    {
        ApplyMovementForce();
        UpdateGroundedStatus();
    }

    private void HandleMovementInput()
    {
        float inputDirectionHorizontal = Input.GetAxisRaw("Horizontal");
        if (Is3D)
        {
            if (Mathf.Abs(inputDirectionHorizontal) > 0.01)
            {
                animator.SetBool("IsWalking",true);
            }
            else
            {
                animator.SetBool("IsWalking", false);
            }
        }
        else
        {
            animator.SetFloat("speed", Mathf.Abs(inputDirectionHorizontal));
        }
    }

    private void ApplyMovementForce()
    {
    float inputDirectionHorizontal = Input.GetAxisRaw("Horizontal");
    Vector3 force = new Vector3(inputDirectionHorizontal * moveSpeed * 50f, 0, 0);
    rb.AddForce(force, ForceMode.Force);

    // Apply friction
    float frictionForce = -rb.velocity.x * (isGrounded ? 1 : 0.5f) * 50f;
    rb.AddForce(new Vector3(frictionForce, 0, 0), ForceMode.Force);
    }



    private void UpdateGroundedStatus()
    {
        Collider[] groundColliders = Physics.OverlapSphere(groundCheck.position, groundCheckCollider.radius, groundLayer);
        isGrounded = groundColliders.Length > 0;
        if (isGrounded)
        {
            if (Is3D)
            {
                animator.SetBool("Grounded", true);
            }
            lastGroundedTime = Time.time;
            if (lastJumpTime > 1f) //added check for last jump time to ensure animation is played
            {
                if (!Is3D)
                {
                    animator.SetBool("isJumping", false);
                }
            }
            
        }
        else
        {
            if (Is3D)
            {
                animator.SetBool("Grounded", false);
            }
        }
    }




    private void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (Time.time - lastGroundedTime <= coyoteTimeDuration)
            {
                lastJumpTime = Time.time; //moved so that isground can check to play animation
                Jump();
                isJumpCutAllowed = true;
            }
        }

        if (Input.GetButtonUp("Jump") && isJumpCutAllowed)
        {
            if (Time.time - lastJumpTime <= jumpCutDuration)
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.5f, rb.velocity.z);
                isJumpCutAllowed = false;
            }
        }
    }

    private void Jump()
{
    float jumpVelocity = Mathf.Sqrt(2 * jumpHeight * -Physics.gravity.y);
    rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
    if (Is3D)
        {
            animator.ResetTrigger("Jump");
            animator.SetTrigger("Jump");
        }
        else
        {
            animator.SetBool("isJumping", true);
            animator.SetTrigger("isJumping"); // Replace "Jump" with the name of the trigger parameter you set up in the Animator Controller
        }
    }




    private void AdjustFallGravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void FlipCharacter()
    {
        float inputDirectionHorizontal = Input.GetAxisRaw("Horizontal");
        if ((inputDirectionHorizontal > 0 && !isFacingRight) || (inputDirectionHorizontal < 0 && isFacingRight))
        {
        isFacingRight = !isFacingRight;
        transform.Rotate(0, 180, 0);
        }
    }
    public bool IsFacingRight()
    {
        return isFacingRight;
    }
}

