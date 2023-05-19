using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    public enum Version { VeryOld, Old, LittleOld, New };

    public GameObject fireballPrefab;
    public Transform fireballSP;
    public float speed;
    public Animator animator;
    public ParticleSystem Rune1;
    public ParticleSystem Rune2;
    public Version Mode;

    private PlayerSoundManager playerSoundManager;

    void Start()
    {
        playerSoundManager = GetComponent<PlayerSoundManager>();
        if (Rune1 != null && Rune2 != null)
        {
            Rune1.Stop();
            Rune2.Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            launchFireball();
        }
    }

    private void launchFireball()
    {
        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
        }
        if(Rune1 != null && Rune2 != null)
        {
            Rune1.Stop();
            Rune2.Stop();
            Rune1.Play();
            Rune2.Play();
        }
        if (playerSoundManager != null)
        {
            playerSoundManager.PlayFireSFX();
        }
        GameObject newFB = Instantiate(fireballPrefab, fireballSP.position, fireballSP.rotation * Quaternion.Euler(0, 0, 90));
        Rigidbody fireballRB = newFB.GetComponent<Rigidbody>();
        if ((Mode == Version.New && GetComponent<AdvancePlayerMovement>().IsFacingRight()) || (Mode == Version.VeryOld && GetComponent<BasicMovement>().IsFacingRight()))
        {
            fireballRB.velocity = new Vector3(speed * 1, 0, 0);
            fireballRB.angularVelocity = new Vector3(speed * 1, 0, 0);
        }
        else
        {
            fireballRB.velocity = new Vector3(speed * -1, 0, 0);
            fireballRB.angularVelocity = new Vector3(speed * 1, 0, 0);
        }
        Destroy(newFB, 5.0f);
    }
}
