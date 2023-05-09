using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody rb;
    private Collider c2D;

    [SerializeField] public float moveSpeed = 5.0f;
    [SerializeField] public float jumpHeight = 10.0f;
    public Transform groundCheck;

    private float inputDirectionHorizontal;
    public bool isFacingRight = true;

    private bool isWalking = false; //flag for when to play certain animations
    //private bool isGrounded = false; //not used but can be added for animations if need be.

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        c2D = GetComponent<Collider>();

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
            isGrounded();
        }
    }

    private void applyMovement()
    {
        rb.velocity = new Vector3(inputDirectionHorizontal * moveSpeed, rb.velocity.y, rb.velocity.z);
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
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
        if (Physics.CheckSphere(groundCheck.position, 0.1f, (LayerMask.GetMask("Ground"))))
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