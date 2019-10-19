using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSoundScript : MonoBehaviour
{
    public AudioClip ambientNoise;

    // Start is called before the first frame update
    void Start()
    {
        SoundManagerScript.PlayBackground(gameObject, ambientNoise);
        SoundManagerScript.FadeInCaller(SoundManagerScript.track, 0.0f, SoundManagerScript.track.trackVolume);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
