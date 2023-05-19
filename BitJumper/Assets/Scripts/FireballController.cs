using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    public GameObject fireballPrefab;
    public Transform fireballSP;
    public float speed;

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
<<<<<<< Updated upstream
        GameObject newFB = Instantiate(fireballPrefab, fireballSP.position, Quaternion.identity);
=======
        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
        }
        GameObject newFB = Instantiate(fireballPrefab, fireballSP.position, fireballSP.rotation * Quaternion.Euler(0, 0, 90));
>>>>>>> Stashed changes
        Rigidbody fireballRB = newFB.GetComponent<Rigidbody>();
        if (GetComponent<AdvancePlayerMovement>().IsFacingRight())
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
