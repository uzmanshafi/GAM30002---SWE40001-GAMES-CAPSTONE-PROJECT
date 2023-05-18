using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    private Transform target;
    public Vector3 perspectiveOffset = new Vector3(2, 2f, -20);
    public Vector3 orthographicOffset = new Vector3(2, 3f, 0);
    private Camera cam;
    private GameObject player;
    private bool isSwitching = false; 
    private float switchTime = 1f; 
    private float currentSwitchTime = 0f; 
    private float startSize;
    private Vector3 startPosition;
    public float orthoSize = 5f;
    public float perspectiveFOV = 20f; 
    public float startOrthoSize = 4f;
    public float startPerspectiveFOV = 60f;

    public GameObject screenFader; // GameObject with the Animator component
    private Animator faderAnimator; // Animator component for controlling the animations

    public KeyCode switchProjectionKey = KeyCode.P;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = startOrthoSize;
        cam.fieldOfView = startPerspectiveFOV;

        faderAnimator = screenFader.GetComponent<Animator>();
        
        player = GameObject.FindWithTag("Player");
        if(player != null) 
        {
            target = player.transform;
        }
        else
        {
            Debug.LogError("Player object not found.");
        }
    }

    void Update()
    {
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
        if (!isSwitching)
        {
            faderAnimator.SetTrigger("FadeIn");
            StartCoroutine(TriggerFadeOut());
            
            startSize = cam.orthographic ? cam.orthographicSize : cam.fieldOfView;
            startPosition = transform.position;
            isSwitching = true;
    }
}

    IEnumerator TriggerFadeOut()
    {
        yield return new WaitForSeconds(2f);
        faderAnimator.SetTrigger("FadeOut");
    }
}