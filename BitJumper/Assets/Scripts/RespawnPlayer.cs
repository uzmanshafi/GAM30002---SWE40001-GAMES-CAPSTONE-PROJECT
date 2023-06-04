using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPlayer : MonoBehaviour
{
    private Transform respawn;
    private Transform player;
    private HealthController health;


    // Start is called before the first frame update
    void Start()
    {
        respawn = GameObject.FindWithTag("Respawn").transform;
    }

    // Update is called once per frame
    void Update()
    {
        player = GameObject.FindWithTag("Player").transform;
        health = GameObject.FindWithTag("Player").GetComponent<HealthController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Boundary")
        {
            player.position = respawn.position;
            if (health != null)
            {
                health.TakeDamage(1f);
            }
        }
    }
    public void ForceSpawn()
    {
        player.position = respawn.position;
    }
}
