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
    [SerializeField] float chargeTime;

    [Header("Relative Transforms")]
    [SerializeField] Transform upCheck;
    [SerializeField] Transform downCheck;
    [SerializeField] Transform frontCheck;

    private Rigidbody rb;

    private GameObject player;
    private Rigidbody playerRB;

    private bool isCharging = false;
    private float charging;

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
        if (isCharging)
        {
            if (playerRB.velocity.y != 0)
            {
                
            }
            if (charging > chargeTime)
            {
                
            }
        }
    }

    private void chargeJump(Vector3 target)
    {
        charging = Time.time;
        isCharging = true;
        Vector3 gravity = Physics.gravity;
        float initialUpVel = Mathf.Sqrt(-2 * gravity.y * jumpAtkHeight);

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
        //return points;

    }

}
