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
        int steps = (int)(distance / movementPerStep);
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
        //return points;

    }

}
