using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    public AudioClip jumpSFX;
    public AudioClip shootSFX;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayJumpSFX()
    {
        audioSource.PlayOneShot(jumpSFX);
    }
    public void PlayFireSFX()
    {
        audioSource.PlayOneShot(shootSFX);
    }
}
