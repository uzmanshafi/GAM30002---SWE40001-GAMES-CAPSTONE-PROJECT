using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPlayer : MonoBehaviour
{
    public static RespawnPlayer Instance;
    public Transform respawnPoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        respawnPoint = GameObject.FindGameObjectWithTag("Respawn").transform;
    }

    public void ForceRespawn()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            HealthController healthController = playerObject.GetComponent<HealthController>();
            if (healthController != null)
            {
                playerObject.transform.position = new Vector3(respawnPoint.position.x, respawnPoint.position.y, 0);  // Reset Z position
                healthController.ResetHealth();
            }
        }
        else
        {
            Debug.LogWarning("No player to respawn!");
        }
    }

    public void EnableCollider()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            Collider playerCollider = playerObject.GetComponent<Collider>();
            if (playerCollider != null)
            {
                playerCollider.enabled = true;
            }
            else
            {
                Debug.LogWarning("No collider to enable!");
            }
        }
        else
        {
            Debug.LogWarning("No player to enable collider!");
        }
    }
}
