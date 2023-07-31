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
    [SerializeField] GameObject BossDoor;

    private int currentHP;

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
    [SerializeField] float shotCooldown;
    [SerializeField] float shootPhaseLength;

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

    // Projectile Phase
    private bool isHiding = false;
    private bool isShooting = false;
    private int hidingHP;
    private float shootingStart = 0f;
    private float lastShot = 0f;


    //Rage Mode
    [Header("Rage Mode Attr")]
    [SerializeField] float RageModeLength;
    private float rageModeStart;
    private bool rageMode = false;

    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(Time.frameCount);
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player");
        playerRB = player.GetComponent<Rigidbody>();
        currentHP = HP;
    }

    // Update is called once per frame
    void Update()
    {
        checkBounds();
        pollDirection();
        applyGravity();
        phase1();
        if (tongueTrigger && isGrounded) //used in both phases
        {
            tongueAttack();
        }
        phase2();
    }

    private void phase1()
    {
        if (!isCharging && rb.velocity.y < 3 && currentHP > HP / 2 && (isGrounded || Time.time - lastJumped > chargeTime * 2)) //not jumping, HP is above 50%, and on ground or stuck on ledge charge jump
        {
            chargeJump();
        }
        if (isCharging)
        {
            if (playerRB.velocity.y > 0.5 && Time.time - charging > chargeTime / 2) //If player makes a move while Boss is charging, the boss would predict the movement and jump to said position. This did not work with both directions so is not used.
            {
                jumpChoice(player.transform.position); //makeJumpPrediction(jumpChoice);
            }
            if (Time.time - charging > chargeTime)
            {
                jumpChoice(player.transform.position);
            }
        }
    }

    private void phase2()
    {
        rage();
        if (!rageMode) //if in rage, don't hide/shoot
        {
            if (!isHiding && !isCharging && isGrounded && rb.velocity.y < 3 && currentHP < HP / 2) //starts hiding phase if hp is lower than half
            {
                hidingHP = currentHP;
                hide();
            }
            if (isHiding && !isShooting && isGrounded && rb.velocity.y < 3) //Once boss is 'hiding' and touching the ground, start shooting phase
            {
                isShooting = true;
                shootingStart = Time.time;
            }
            if (isShooting && isGrounded && Time.time - lastShot > shotCooldown) //Make Aim predicition and fire shot on cooldown
            {
                aimPrediction(player.transform.position, 10);
            }
            if (isShooting && Time.time - shootingStart > shootPhaseLength) //Once we have exceeded the shooting phase length, stop both and start hiding
            {

                isShooting = false;
                isHiding = false;
            }
        }
    }

    private void applyGravity()
    {
        float dx = player.transform.position.x - transform.position.x;
        if (!isHiding && (rb.velocity.y < 0.5 && player.transform.position.y - transform.position.y < -2))//&& (dx < 1 && dx > -1) ) // dont stomp if finding hiding spot
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
        else if (player.transform.position.x - gameObject.transform.position.x < -5 && isFacingRight)
        {
            flip();
        }

    }

    private void flip()
    {
        if (!isHiding || isShooting) //dont clamp velocity or turn if not targeting player
        {
            isFacingRight = !isFacingRight;
            transform.RotateAround(GetComponent<Collider>().bounds.center, Vector3.up, 180f);
            rb.velocity = new Vector3(0.1f, rb.velocity.y, rb.velocity.z); //to make sure we land on target (shouldnt really be flipping in air anyway)
        }
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
        charging = Time.time; // Used to check how long we've been charging
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
        int steps = (int)(distance / movementPerStep);
        var acc = Physics.gravity;

        var points = new Vector3[steps];
        points[0] = playerRB.position;
        for (int i = 1; i < steps; i++) //Iterates through steps simulating 1 update of movement, not super accurate especially when right facing
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
        attkchoice(points[points.Length - 1]);

    }  //not working when right facing for unknown reason

    private void jumpAttack(Vector3 target) //Usage of transposed Kinematics equations 
    {                                       //to create the perfect jump arc with a given target and jumpHeight
        Vector3 gravity = Physics.gravity;
        //float dx = target.position.x - gameObject.transform.position.x;
        float dy = target.y - gameObject.transform.position.y;
        Vector3 dxz = new Vector3(target.x - rb.transform.position.x, 0, target.z - rb.transform.position.z);

        float tempJH = jumpAtkHeight;
        if (dxz.x < 5 && dxz.x > -5)
        {
            jumpAtkHeight = jumpAtkHeight / 2;
        }

        Vector3 initialUpVel = Vector3.up * Mathf.Sqrt(-2 * (gravity.y * 2) * jumpAtkHeight);

        float upwardTime = Mathf.Sqrt((-2 * jumpAtkHeight) / gravity.y);
        float downwardTime = Mathf.Sqrt((2 * (dy - jumpAtkHeight)) / gravity.y);
        Vector3 initialHorizontalVel = Vector3.zero;
        if (float.IsNaN(upwardTime) || float.IsNaN(downwardTime))
        {
            initialHorizontalVel = (dxz * 1.7f) / (3);
            Debug.Log("fumbled");
        }
        else
        {
            initialHorizontalVel = (dxz * 1.7f) / (upwardTime + downwardTime);
        }
        

        isCharging = false;
        
        rb.velocity = initialHorizontalVel + initialUpVel;
        lastJumped = Time.time;
        jumpAtkHeight = tempJH;
        
    }

    private void jumpAttack2(Vector3 target)
    {
        Vector3 gravity = Physics.gravity;
        float dy = target.y - gameObject.transform.position.y;
        Vector3 dxz = new Vector3(target.x - rb.transform.position.x, 0, target.z - rb.transform.position.z);

        float tempJH = jumpAtkHeight;
        if (dxz.x < 5 && dxz.x > -5)
        {
            jumpAtkHeight = jumpAtkHeight / 2;
        }

        Vector3 initialUpVel = Vector3.up * Mathf.Sqrt(-2 * (gravity.y * 1.2f) * jumpAtkHeight);

        float upwardTime = Mathf.Sqrt((-2 * jumpAtkHeight) / gravity.y);
        float downwardTime = Mathf.Sqrt((2 * (dy - jumpAtkHeight)) / gravity.y);

        Vector3 initialHorizontalVel = (dxz * 1.5f) / (upwardTime + downwardTime);

        isCharging = false;
        tongueTrigger = true;
        rb.velocity = initialHorizontalVel + initialUpVel;
        lastJumped = Time.time;
        jumpAtkHeight = tempJH;
        //Debug.Log("jumped2");
    }

    private void tongueAttack()
    {
        if (Time.time - lastJumped > 0.5) //make sure we dont shoot before we leave the ground
        {
            tongueTrigger = false;
            GameObject tongue = Instantiate(tonguePrefab, tongueSP);
            tongue.transform.parent = gameObject.transform;
            //Physics.IgnoreCollision(tongue.GetComponent<Collider>(), GetComponent<Collider>());
        }

    }

    private void hide()
    {
        isHiding = true;
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
        Transform[] platformPos = new Transform[platforms.Length];
        for (int i = 0; i < platforms.Length; i++) //Get a referance to all platforms positions
        {
            platformPos[i] = platforms[i].GetComponent<Transform>();
        }
        float tempDist;
        float bestdistance = 0;
        Transform bestHidingSpot = gameObject.transform;
        foreach (Transform p in platformPos) //Iterate through platforms to find best hiding spot (Most distance, would have made it distance from player but it does not work well for gameplay))
        {
            tempDist = Vector3.Distance(gameObject.transform.position, p.position);
            if (tempDist > bestdistance)
            {
                bestdistance = tempDist;
                bestHidingSpot = p;
            }
        }
        Vector3 hidingSpot = Vector3.zero + bestHidingSpot.position;
        hidingSpot.x -= 4; //Append hiding spot location to account for frog's large size and offset body
        jump(hidingSpot);
    }

    private void aimPrediction(Vector3 target, float speed = 0.5f)
    {
        float distance_to_target = Vector3.Distance(transform.position, target);
        float time_to_target = distance_to_target / speed;
        float targetXMovement = playerRB.velocity.x;
        float targetXPos = player.transform.position.x;
        float xdisplacementPerTick;

        if (targetXMovement > 0) //Find player's X movement/displacement
        {
            xdisplacementPerTick = targetXPos - (targetXPos + (targetXMovement * Time.deltaTime));
        }
        else
        {
            xdisplacementPerTick = targetXPos - (targetXPos - (targetXMovement * Time.deltaTime));
        }

        float targetYMovement = playerRB.velocity.y;
        float targetYPos = player.transform.position.y;
        float ydisplacementPerTick;

        if (targetYMovement > 0) //Find player's Y movement/displacement
        {
            ydisplacementPerTick = targetYPos - (targetYPos + (targetYMovement * Time.deltaTime));
        }
        else
        {
            ydisplacementPerTick = targetYPos - (targetYPos - (targetYMovement * Time.deltaTime));
        }

        float xDisplacement = xdisplacementPerTick * time_to_target;
        float yDisplacement = ydisplacementPerTick * time_to_target;
        Vector3 predictedVec = new Vector3(targetXPos + xDisplacement, targetYPos + yDisplacement); //predict location based on time for projectile to reach the target

        float dx = predictedVec.x - transform.position.x; 
        float dy = predictedVec.y - transform.position.y;

        double firing_angle = Math.Atan2(dy, dx); //using the displacmement, Find the angle we will have to fire at from our current pos.

        Vector3 bulletVel = new Vector3((float)(speed * Math.Cos(firing_angle)), (float)(speed * Math.Sin(firing_angle))); //Utilising the speed and firing angle, create a vector for the new bullet's required velocity

        GameObject shotInstance = Instantiate(shot, new Vector3(shotSP.position.x, shotSP.position.y), Quaternion.identity);
        shotInstance.transform.LookAt(player.transform);
        shotInstance.GetComponent<Rigidbody>().velocity = bulletVel;
        lastShot = Time.time;

    }

    private void jump(Vector3 target)
    {                                    
        Vector3 gravity = Physics.gravity; // G = gravity
        float dy = target.y - gameObject.transform.position.y;
        Vector3 dxz = new Vector3(target.x - rb.transform.position.x, 0, target.z - rb.transform.position.z);

        float tempJH = jumpAtkHeight;// H = jumpHeight
        if (dxz.x < 5 && dxz.x > -5)
        {
            jumpAtkHeight = jumpAtkHeight / 2;
        }

        Vector3 initialUpVel = Vector3.up * Mathf.Sqrt(-2 * (gravity.y) * (jumpAtkHeight*1.7f)); // Initial Up Vel U = SQRT(-2gh)

        float upwardTime = Mathf.Sqrt((-2 * jumpAtkHeight) / gravity.y); //Upward time T up = SQRT(-2h/g)
        float downwardTime = Mathf.Sqrt((2 * (dy - jumpAtkHeight)) / gravity.y); //Downward time T dwn = SQRT(2(dy - h) / g)
        Vector3 initialHorizontalVel = Vector3.zero;
        if (float.IsNaN(upwardTime) || float.IsNaN(downwardTime)) //Unity API rounds any float 0.09 or lower to 0 causing NAN and a failed jump.
        {
            initialHorizontalVel = (dxz) / (3);
            Debug.Log("fumbled");
        }
        else
        {
            initialHorizontalVel = (dxz) / (upwardTime + downwardTime); // Initial Left/Right Vel U = dx / T up + T dwn
        }


        isCharging = false;

        rb.velocity = initialHorizontalVel + initialUpVel;
        lastJumped = Time.time;
        jumpAtkHeight = tempJH;

    } //this function shows the mostly un-edited kinematics code, commented to show off the use of the equations and not tuned for a more playable jump like others

    private void rage()
    {
        if (currentHP - hidingHP < -5) //If boss takes a certain amount of Hits during the hiding phase move into Rage Mode.
        {
            isHiding = false;
            isShooting = false;
            rageMode = true;
            rageModeStart = Time.time;
        }
        if (rageMode && Time.time - rageModeStart < RageModeLength) // While in rage mode, make frequent random jumps
        {
            if (!isCharging && rb.velocity.y < 3 &&  (isGrounded || Time.time - lastJumped > chargeTime * 4))
            {
                chargeJump();
            }
            if (isCharging && Time.time - charging > 0.5)
            {
                jumpChoice(player.transform.position);
            }
        }
    }

    public bool FacingRight() //used by the Tounge to determine its direction to shoot.
    {
        return isFacingRight;
    }
    public void takeDamage(int damage) //So it can take damage from player.
    {
        currentHP -= damage;
        Debug.Log(currentHP);
        if (currentHP <= 0)
        {
            die();
        }
    }

    private void die() // Instansiate boss door before death (for gameplay purposes)
    {
        BossDoor.SetActive(true);
        //Instantiate(BossDoor, new Vector3(groundCheck.position.x, groundCheck.position.y, 0), Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            HealthController playerHP = collision.collider.GetComponent<HealthController>();
            playerHP.TakeDamage(1);
        }
    }
}