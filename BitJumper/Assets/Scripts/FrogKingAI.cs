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

    [Header("Relative Transforms")]
    [SerializeField] Transform upCheck;
    [SerializeField] Transform downCheck;
    [SerializeField] Transform frontCheck;

    private Rigidbody rb;

    private GameObject player;
    private Rigidbody playerRB;

    private bool isCharging = false;
    private float charging; //current charge time
    //private float initialHeight;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player");
        playerRB = player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (rb.transform.position.y > initialHeight + 15)
        {
            rb.velocity = new Vector3(rb.velocity.x, Physics.gravity.y * 2, rb.velocity.z);
        }*/
        if (Input.GetKeyDown(KeyCode.C))
        {
            chargeJump();
        }
        if (isCharging)
        {
            if (playerRB.velocity.y != 0)
            {
                makeJumpPrediction();
            }
            if (Time.time - charging > chargeTime)
            {
                jumpAttack(player.transform.position);
            }
        }
    }


    //private void chargeJump(Vector3 target)
    private void chargeJump()
    {
        charging = Time.time;
        isCharging = true;
        //initialHeight = rb.transform.position.y;
    }

    private void makeJumpPrediction()
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
        jumpAttack(points[points.Length-1]);

    }

    private void jumpAttack(Vector3 target)
    {
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
    }

}
