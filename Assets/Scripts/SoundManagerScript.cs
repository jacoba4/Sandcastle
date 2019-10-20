using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManagerScript : MonoBehaviour
{
    public static SoundManagerScript instance;
    public static TrackClass track = new TrackClass();

    private static bool keepFadingIn;
    private static bool keepFadingOut;

    private void Awake()
    {
        instance = this;
    }

    public static void CreateTrack (GameObject gameObj)
    {
        TrackClass track = new TrackClass { audioSource = gameObj.AddComponent<AudioSource>() };
    }

    public static void PlayBackground(GameObject gameObj, AudioClip audioClip)
    {
        gameObj.GetComponent<AudioSource>().PlayOneShot(audioClip);
    }

    //audioClip starts from volume 0 and increase to maxVolume, increasing in volume by speed every .1s
    static IEnumerator FadeIn (TrackClass track, float speed, float maxVolume)
    {
        keepFadingIn = true;
        keepFadingOut = false;

        track.audioSource.volume = 0;
        float audioVolume = track.audioSource.volume;

        while (track.audioSource.volume < maxVolume && keepFadingIn)
        {
            audioVolume += speed;
            track.audioSource.volume = audioVolume;
            yield return new WaitForSeconds(0.1f);
        }
    }

    //audioClip starts at max volume and fades to 0, decreasing in volume by the value of speed every .1s
    static IEnumerator FadeOut(TrackClass track, float speed)
    {
        keepFadingIn = false;
        keepFadingOut = true;

        float audioVolume = track.audioSource.volume;

        while (track.audioSource.volume >= speed && keepFadingOut)
        {
            audioVolume -= speed;
            track.audioSource.volume = audioVolume;
            yield return new WaitForSeconds(0.1f);
        }
    }


    //calls the static FadeIn coroutine so the AmbSoundScript can access it
    public static void FadeInCaller(TrackClass track, float speed, float maxVolume)
    {
        instance.StartCoroutine(FadeIn(track, speed, maxVolume));
    }

    //calls the static FadeOut coroutine
    public static void FadeOutCaller(TrackClass track, float speed)
    {
        instance.StartCoroutine(FadeOut(track, speed));
    }
}
