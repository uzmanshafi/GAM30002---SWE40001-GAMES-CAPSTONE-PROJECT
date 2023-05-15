using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFollowPlayer : MonoBehaviour
{
    private Transform target;
    public Vector3 perspectiveOffset = new Vector3(2, -4f, -12);
    public Vector3 orthographicOffset = new Vector3(2, -3f, 0);
    private Camera cam;
    private GameObject player;
    private bool isSwitching = false; // flag to indicate if camera is in the process of switching
    private float switchTime = 1f; // total time for switching
    private float currentSwitchTime = 0f; // current time of switch
    private float startSize; // start size of orthographic camera
    private Vector3 startPosition; // start position of camera
    public float orthoSize = 5f; // size of orthographic camera
    public float perspectiveFOV = 20f; // FOV of perspective camera
    public float startOrthoSize = 5f; // starting size of orthographic camera
    public float startPerspectiveFOV = 60f; // starting FOV of perspective camera

    // Specify the key for switching camera projection mode
    public KeyCode switchProjectionKey = KeyCode.P;
    public Image fadeOverlay; // Drag your UI Image here in the inspector
    public float fadeDuration = 1f; // The time it will take for the screen to fade to black and back
    private bool isFading = false; // This is to keep track of whether a fade is currently happening


    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = startOrthoSize;
        cam.fieldOfView = startPerspectiveFOV;
    }

    IEnumerator FadeToBlack()
    {
    isFading = true;
    float fadeSpeed = 1f / fadeDuration;

    // Fade to black
    for (float t = 0f; t < 1f; t += Time.deltaTime * fadeSpeed)
    {
        Color color = fadeOverlay.color;
        color.a = t;
        fadeOverlay.color = color;
        yield return null;
    }

    // Ensure the overlay is fully opaque
    Color black = fadeOverlay.color;
    black.a = 1f;
    fadeOverlay.color = black;

    // Wait for 3 seconds
    yield return new WaitForSeconds(2f);

    // Fade to clear
    for (float t = 0f; t < 1f; t += Time.deltaTime * fadeSpeed)
    {
        Color color = fadeOverlay.color;
        color.a = 1f - t;
        fadeOverlay.color = color;
        yield return null;
    }

    // Ensure the overlay is fully transparent
    Color clear = fadeOverlay.color;
    clear.a = 0f;
    fadeOverlay.color = clear;

    isFading = false;
}


    void Update()
    {
        player = GameObject.FindWithTag("Player");
        target = player.transform;

        if (Input.GetKeyDown(switchProjectionKey))
        {
            SwitchProjection();
        }
        
        if (isSwitching)
        {
            currentSwitchTime += Time.deltaTime;
            float t = currentSwitchTime / switchTime;
            if (t >= 1f)
            {
                isSwitching = false;
                currentSwitchTime = 0f;
                cam.orthographic = !cam.orthographic;
                if (cam.orthographic)
                {
                    cam.orthographicSize = orthoSize;
                }
                else
                {
                    cam.fieldOfView = perspectiveFOV;
                }
            }
            else
            {
                if (cam.orthographic)
                {
                    cam.orthographicSize = Mathf.Lerp(startSize, orthoSize, t);
                    transform.position = Vector3.Lerp(startPosition, target.position + perspectiveOffset, t);
                }
                else
                {
                    cam.fieldOfView = Mathf.Lerp(startSize, perspectiveFOV, t); 
                    transform.position = Vector3.Lerp(startPosition, new Vector3(target.position.x + orthographicOffset.x, target.position.y + orthographicOffset.y, transform.position.z), t);
                }
            }
        }
    }

        void LateUpdate()
    {
        if (target != null && !isSwitching)
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
        else if (target == null)
        {
            Debug.LogWarning("CameraController could not find a target.");
        }
    }

    public void PerspChange()
    {
        SwitchProjection();
    }

    private void SwitchProjection()
    {
        if (!isSwitching && !isFading)
        {
            StartCoroutine(FadeToBlack());
            startSize = cam.orthographic ? cam.orthographicSize : cam.fieldOfView;
            startPosition = transform.position;
            isSwitching = true;
        }
    }
}
