using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] private float Max_HealthBar = 3.8f;
    public float currentHealth { get; private set; }
    public bool isDying = false;
    private BasicMovement basicMovement;
    private Rigidbody rb;
    private Collider playerCollider;

    public static bool isPlayerDead = false;
    public float deathJumpForce = 8f;
    public float deathFallForce = 0.75f;
    [SerializeField] private Animator anim;



    public void Awake()
    {
        currentHealth = Max_HealthBar;
        basicMovement = GetComponent<BasicMovement>();
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
    }

    public void TakeDamage(float damage)
    {
        if (isDying) return;

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, Max_HealthBar);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void ResetHealth()
    {
        isPlayerDead = false;
        currentHealth = Max_HealthBar;
        isDying = false;
        StopCoroutine(FallFasterWhenDeadCoroutine());
    }

    private void Die()
    {
        isDying = true;
        rb.velocity = Vector3.up * deathJumpForce;
        basicMovement.SetMovementStateToJumping();
        StartCoroutine(FallFasterWhenDeadCoroutine());
        StartCoroutine(DieCoroutine());
    }



    IEnumerator DieCoroutine()
    {
        isPlayerDead = true;
        yield return new WaitForSeconds(1);
        playerCollider.enabled = false; // Disable the collider instead of making it a trigger
        transform.position += new Vector3(0, 0, -4);  // Move the player down on the Z axis
        yield return new WaitForSeconds(1);
        RespawnPlayer.Instance.ForceRespawn();
        RespawnPlayer.Instance.EnableCollider();  // Call the method to re-enable the collider after respawning
    }

    private IEnumerator FallFasterWhenDeadCoroutine()
    {
        while (isDying)
        {
            // Check if the player is falling (y velocity is negative)
            if (rb.velocity.y < 0)
            {
                // Apply extra downward force
                rb.AddForce(new Vector3(0, -1, 0) * deathFallForce, ForceMode.VelocityChange);
            }
            yield return null;  // Wait for next frame
        }
    }


}




