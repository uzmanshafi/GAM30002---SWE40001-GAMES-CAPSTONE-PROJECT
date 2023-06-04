using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPlayer : MonoBehaviour
{
    public Transform respawn;
    public Transform player;


    // Start is called before the first frame update
    void Start()
    {
        respawn = GameObject.FindWithTag("Respawn").transform;
    }

    // Update is called once per frame
    void Update()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Boundary")
        {
            player.position = respawn.position;
        }
        if (other.tag == "Enemy")
        {
            player.position = respawn.position;
        }
    }
    public void ForceSpawn()
    {
        player.position = respawn.position;
    }
}
