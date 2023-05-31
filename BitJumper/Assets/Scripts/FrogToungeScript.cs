using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogToungeScript : MonoBehaviour
{

    public float speed;

    private Rigidbody rb;
    private float xStartPos;

    private FrogKingAI frogKingScript;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        xStartPos = gameObject.transform.position.x;
        frogKingScript = GetComponentInParent<FrogKingAI>();

        if (!frogKingScript.FacingRight())
        {
            rb.AddForce(Vector3.left * speed, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(Vector3.right * speed, ForceMode.Impulse);
        }
    }

    // Update is called once per frame
    void Update()
    {
       if(!frogKingScript.FacingRight())
        {
            if (rb.transform.position.x - xStartPos < -10f)
            {
                Destroy(gameObject);
            }
        }
       else
        {
            if (rb.transform.position.x - xStartPos > 10f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            //do damage
            Destroy(gameObject);
        }
        else if(other.tag == "FrogKing")
        {
            //do nothing
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "FrogKing")
        {
            Destroy(gameObject);
        }
    }
}
