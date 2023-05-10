using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FidelityOrb : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerModelChangeTest fidelityChange = other.gameObject.GetComponent<PlayerModelChangeTest>();
        if (fidelityChange != null)
        {
            fidelityChange.graphicalChange();
            Destroy(gameObject);
        }
    }
}
