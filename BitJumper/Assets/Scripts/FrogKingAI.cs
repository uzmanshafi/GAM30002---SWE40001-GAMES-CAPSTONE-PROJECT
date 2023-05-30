using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogKingAI : MonoBehaviour
{
    [Header("Boss Attributes")]
    [SerializeField] int HP;
    [SerializeField] int Damage;
    [SerializeField] float AttackCooldown;

    [Header("Jump Attack atr")]
    [SerializeField] float jumpAtkHeight;
    [SerializeField] float jumpAtkMS;
    [SerializeField] float chargeTime; //Max charge
    [SerializeField] GameObject tonguePrefab;
    [SerializeField] Transform tongueSP;
    private bool tongueTrigger = false;

    [Header("Relative Transforms")]
    [SerializeField] Transform upCheck;
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform frontCheck;

    private bool isGrounded = true;
    private bool isTouchingFront = false;
    private bool isTouchingUp = false;
    private bool isFacingRight = false;

    private Rigidbody rb;

    private GameObject player;
    private Rigidbody playerRB;

    //for timing in relation to charge/jump
    private bool isCharging = false;
    private float charging; //current charge time
    private float lastJumped;

    // For choosing between attacks
    private delegate void jumpAttkChoice(Vector3 target); //delegate for passing choice of jump Function
    private jumpAttkChoice jumpChoice;

    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(Time.frameCount);
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player");
        playerRB = player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        checkBounds();
        pollDirection();
        if (Input.GetKeyDown(KeyCode.C))
        {
            chargeJump();
        }
        if (isCharging)
        {
            if (playerRB.velocity.y > 0.5)
            {
                makeJumpPrediction(jumpChoice);
            }
            if (Time.time - charging > chargeTime)
            {
                jumpChoice(player.transform.position);
            }
        }
        if (tongueTrigger && isGrounded)
        {
            tongueAttack();
        }
    }

    private void pollDirection()
    {
        if (player.transform.position.x - gameObject.transform.position.x > 0 && !isFacingRight)
        {
            flip();
        }
        else if(player.transform.position.x - gameObject.transform.position.x < 0 && isFacingRight)
        {
            flip();
        }

    }

    private void flip()
    {
        isFacingRight = !isFacingRight;
        transform.RotateAround(GetComponent<Collider>().bounds.center, Vector3.up, 180f);
    }

    private void checkBounds()
    {
        if (Physics.CheckSphere(groundCheck.position, 0.3f, (LayerMask.GetMask("Ground"))))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        if (Physics.CheckSphere(frontCheck.position, 0.3f, (LayerMask.GetMask("Ground"))))
        {
            isTouchingFront = true;
        }
        else
        {
            isTouchingFront = false;
        }
        if (Physics.CheckSphere(upCheck.position, 0.3f, (LayerMask.GetMask("Ground"))))
        {
            isTouchingUp = true;
        }
        else
        {
            isTouchingUp = false;
        }
    }

    private void chargeJump()
    {
        charging = Time.time;
        isCharging = true;
        if (false)//(Random.Range(0,2) == 0)
        {
            jumpChoice = new jumpAttkChoice(jumpAttack);
        }
        else
        {
            jumpChoice = new jumpAttkChoice(jumpAttack2);
        }
    }

    private void makeJumpPrediction(jumpAttkChoice attkchoice)
    {
        var pos = playerRB.transform.position;
        var vel = playerRB.velocity;
        var drag = playerRB.drag;
        var dt = Time.fixedDeltaTime / Physics.defaultSolverVelocityIterations;
        float distance = Vector3.Distance(playerRB.position, gameObject.transform.position);
        float movementPerStep = (jumpAtkHeight + jumpAtkHeight / 2) * dt;
        int steps = (int) (distance / movementPerStep);
        var acc = Physics.gravity;

        var points = new Vector3[steps];
        points[0] = playerRB.position;
        for (int i = 1; i < steps; i++)
        {
            float t = 0;
            while (t < 1f)
            {
                vel += acc * dt;
                pos += vel * dt;
                pos *= drag;
                t += dt;
            }
            points[i] = pos;
        }
        attkchoice(points[points.Length-1]);

    }

    private void jumpAttack(Vector3 target) //Usage of transposed Kinematics equations 
    {                                       //to create the perfect jump arc with a given target and jumpHeight
        Vector3 gravity = Physics.gravity;
        //float dx = target.position.x - gameObject.transform.position.x;
        float dy = target.y - gameObject.transform.position.y;
        Vector3 dxy = new Vector3(target.x - rb.transform.position.x, 0, target.z - rb.transform.position.z);

        Vector3 initialUpVel = Vector3.up * Mathf.Sqrt(-2 * gravity.y * jumpAtkHeight);

        float upwardTime = Mathf.Sqrt((-2 * jumpAtkHeight) / gravity.y);
        float downwardTime = Mathf.Sqrt((2*(dy - jumpAtkHeight))/ gravity.y);

        Vector3 initialHorizontalVel = (dxy * 1.1f) / (upwardTime + downwardTime);

        isCharging = false;
        rb.velocity = initialHorizontalVel + initialUpVel;
        lastJumped = Time.time;
    }

    private void jumpAttack2(Vector3 target) 
    {                                       
        Vector3 gravity = Physics.gravity;
        float dy = target.y - gameObject.transform.position.y;
        Vector3 dxy = new Vector3(target.x - rb.transform.position.x, 0, target.z - rb.transform.position.z);

        Vector3 initialUpVel = Vector3.up * Mathf.Sqrt(-2 * gravity.y * jumpAtkHeight);

        float upwardTime = Mathf.Sqrt((-2 * jumpAtkHeight) / gravity.y);
        float downwardTime = Mathf.Sqrt((2 * (dy - jumpAtkHeight)) / gravity.y);

        Vector3 initialHorizontalVel = (dxy * 0.85f) / (upwardTime + downwardTime);

        isCharging = false;
        tongueTrigger = true;
        rb.velocity = initialHorizontalVel + initialUpVel;
        lastJumped = Time.time;
    }

    private void tongueAttack()
    {
        if (Time.time - lastJumped > 0.5) //make sure we dont shoot early
        {
            tongueTrigger = false;
            GameObject tongue = Instantiate(tonguePrefab, tongueSP);
            //Physics.IgnoreCollision(tongue.GetComponent<Collider>(), GetComponent<Collider>());
        }

    }

  

}
