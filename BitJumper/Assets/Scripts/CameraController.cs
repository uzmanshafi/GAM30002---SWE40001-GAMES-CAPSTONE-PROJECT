using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target;
    public Vector3 perspectiveOffset = new Vector3(2, 2f, -6.5f);
    public Vector3 orthographicOffset = new Vector3(2, 1f, 0);
    private Camera cam;
    private GameObject player;

    // Specify the key for switching camera projection mode
    public KeyCode switchProjectionKey = KeyCode.P;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        player = GameObject.FindWithTag("Player");
        target = player.transform;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            if (cam.orthographic)
            {
                transform.position = new Vector3(target.position.x + orthographicOffset.x, target.position.y + orthographicOffset.y, transform.position.z);
            }
            else
            {
                transform.position = target.position + perspectiveOffset;
            }
        }
        else
        {
            Debug.LogWarning("CameraController could not find a target.");
        }
    }
    public void PerspChange()
    {
        cam.orthographic = !cam.orthographic;
    }
}
