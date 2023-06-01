using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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

    [Header("Shoot Attack")]
    [SerializeField] GameObject shot;
    [SerializeField] Transform shotSP;

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
        applyGravity();
        if (Input.GetKeyDown(KeyCode.C))
        {
            aimPrediction(player.transform.position, 10);
        }
        if (!isCharging && isGrounded && rb.velocity.y < 3)
        {
            chargeJump();
        }
        if (isCharging)
        {
            if (playerRB.velocity.y > 0.5)
            {
                jumpChoice(player.transform.position); //makeJumpPrediction(jumpChoice);
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

    private void applyGravity()
    {
        float dx = player.transform.position.x - transform.position.x;
        if (rb.velocity.y < 0.5 && player.transform.position.y - transform.position.y < -2 )//&& (dx < 1 && dx > -1) )
        {
            rb.velocity += (Physics.gravity * 8) * Time.deltaTime;
        }
        
    }

    private void pollDirection()
    {
        if (player.transform.position.x - gameObject.transform.position.x > 5 && !isFacingRight)
        {
            flip();
        }
        else if(player.transform.position.x - gameObject.transform.position.x < -5 && isFacingRight)
        {
            flip();
        }

    }

    private void flip()
    {
        isFacingRight = !isFacingRight;
        transform.RotateAround(GetComponent<Collider>().bounds.center, Vector3.up, 180f);
        rb.velocity = new Vector3(0.1f, rb.velocity.y, rb.velocity.z); //to make sure we land on target (shouldnt really be flipping in air anyway)
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
        int randomInt = Random.Range(0, 2);
        //Debug.Log(randomInt); random seems not too random
        if (randomInt == 1)
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

    }  //not working when right facing for unknown reason

    private void jumpAttack(Vector3 target) //Usage of transposed Kinematics equations 
    {                                       //to create the perfect jump arc with a given target and jumpHeight
        Vector3 gravity = Physics.gravity;
        //float dx = target.position.x - gameObject.transform.position.x;
        float dy = target.y - gameObject.transform.position.y;
        Vector3 dxy = new Vector3(target.x - rb.transform.position.x, 0, target.z - rb.transform.position.z);

        float tempJH = jumpAtkHeight;
        if (dxy.x < 5 && dxy.x > -5)
        {
            jumpAtkHeight = jumpAtkHeight / 2;
        }

        Vector3 initialUpVel = Vector3.up * Mathf.Sqrt(-2 * (gravity.y * 2) * jumpAtkHeight);

        float upwardTime = Mathf.Sqrt((-2 * jumpAtkHeight) / gravity.y);
        float downwardTime = Mathf.Sqrt((2*(dy - jumpAtkHeight))/ gravity.y);

        Vector3 initialHorizontalVel = (dxy * 1.7f) / (upwardTime + downwardTime);

        isCharging = false;
        rb.velocity = initialHorizontalVel + initialUpVel;
        lastJumped = Time.time;
        jumpAtkHeight = tempJH;
        Debug.Log("jumped");
    }

    private void jumpAttack2(Vector3 target) 
    {       
        Vector3 gravity = Physics.gravity;
        float dy = target.y - gameObject.transform.position.y;
        Vector3 dxy = new Vector3(target.x - rb.transform.position.x, 0, target.z - rb.transform.position.z);

        float tempJH = jumpAtkHeight;
        if (dxy.x < 5 && dxy.x > -5)
        {
            jumpAtkHeight = jumpAtkHeight / 2;
        }

        Vector3 initialUpVel = Vector3.up * Mathf.Sqrt(-2 * (gravity.y * 1.2f) * jumpAtkHeight);

        float upwardTime = Mathf.Sqrt((-2 * jumpAtkHeight) / gravity.y);
        float downwardTime = Mathf.Sqrt((2 * (dy - jumpAtkHeight)) / gravity.y);

        Vector3 initialHorizontalVel = (dxy * 1.5f) / (upwardTime + downwardTime);

        isCharging = false;
        tongueTrigger = true;
        rb.velocity = initialHorizontalVel + initialUpVel;
        lastJumped = Time.time;
        jumpAtkHeight = tempJH;
        Debug.Log("jumped2");
    }

    private void tongueAttack()
    {
        if (Time.time - lastJumped > 0.5) //make sure we dont shoot early
        {
            tongueTrigger = false;
            GameObject tongue = Instantiate(tonguePrefab, tongueSP);
            tongue.transform.parent = gameObject.transform;
            //Physics.IgnoreCollision(tongue.GetComponent<Collider>(), GetComponent<Collider>());
        }

    }

    
    private void aimPrediction(Vector3 target, float speed = 0.5f ) 
    {
        float distance_to_target = Vector3.Distance(transform.position, target);
        float time_to_target = distance_to_target / speed;
        float targetXMovement = playerRB.velocity.x;
        float targetXPos = player.transform.position.x;
        float xdisplacementPerTick;

        if (targetXMovement > 0) {
            xdisplacementPerTick = targetXPos - (targetXPos + (targetXMovement * Time.deltaTime));
        }
        else {
            xdisplacementPerTick = targetXPos - (targetXPos - (targetXMovement * Time.deltaTime));
        }

        float targetYMovement = playerRB.velocity.y;
        float targetYPos = player.transform.position.y;
        float ydisplacementPerTick;

        if (targetYMovement > 0)
        {
            ydisplacementPerTick = targetYPos - (targetYPos + (targetYMovement * Time.deltaTime));
        }
        else
        {
            ydisplacementPerTick = targetYPos - (targetYPos - (targetYMovement * Time.deltaTime));
        }

        float xDisplacement = xdisplacementPerTick * time_to_target;
        float yDisplacement = ydisplacementPerTick * time_to_target;
        Vector3 predictedVec = new Vector3(targetXPos + xDisplacement, targetYPos + yDisplacement);

        float dx = predictedVec.x - transform.position.x;
        float dy = predictedVec.y - transform.position.y;

        double firing_angle = Math.Atan2(dy, dx);

        Vector3 bulletVel = new Vector3((float)(speed * Math.Cos(firing_angle)), (float)(speed * Math.Sin(firing_angle)));

        GameObject shotInstance = Instantiate(shot, new Vector3(shotSP.position.x, shotSP.position.y), Quaternion.identity);
        shotInstance.transform.LookAt(player.transform);
        shotInstance.GetComponent<Rigidbody>().velocity = bulletVel;

    }

    public bool FacingRight()
    {
        return isFacingRight;
    }
    public void takeDamage(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }
}
