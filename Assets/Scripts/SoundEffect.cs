using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    public AudioClip soundEffect;
    AudioSource audioSource;
    
    public void PlaySoundEffect()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(soundEffect, 0.7F);
    }
}
