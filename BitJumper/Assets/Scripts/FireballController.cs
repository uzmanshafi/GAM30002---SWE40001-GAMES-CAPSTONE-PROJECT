using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    public GameObject fireballPrefab;
    public Transform fireballSP;
    public float speed;
    public Animator animator;

    void Start()
    {
        
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
        GameObject newFB = Instantiate(fireballPrefab, fireballSP.position, Quaternion.identity);
        Rigidbody fireballRB = newFB.GetComponent<Rigidbody>();
        if (GetComponent<AdvancePlayerMovement>().IsFacingRight())
        {
            fireballRB.velocity = new Vector3(speed * 1, 0, 0);
        }
        else
        {
            fireballRB.velocity = new Vector3(speed * -1, 0, 0);
        }
        Destroy(newFB, 5.0f);
    }
}
