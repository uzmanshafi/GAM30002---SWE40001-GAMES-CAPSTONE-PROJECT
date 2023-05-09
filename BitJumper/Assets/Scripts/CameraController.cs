using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 offset = new Vector3(2, 1.2f, -3);

    void LateUpdate()
    {
        transform.position = GetPlayerPosition() + offset;
    }

    private Vector3 GetPlayerPosition()
{
    // Find the player's GameObject using its tag
    GameObject player = GameObject.FindGameObjectWithTag("Player");

    if (player != null)
    {
        // Return the player's position with the offset
        return player.transform.position + offset;
    }
    else
    {
        Debug.LogWarning("FixedCamera could not find player GameObject with tag 'Player'.");
        return transform.position;
    }
}

}
