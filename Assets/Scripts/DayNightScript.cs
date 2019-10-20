using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightScript : MonoBehaviour
{

    public Light sun;
    public float secondsInFullDay = 120f;
    [Range(0, 1)]
    public float currentTimeOfDay = 0f;
    [HideInInspector]
    public float timeMultiplier = 1f;

    public int startPosition;
    public bool isSun;

    float sunInitialIntensity;

    void Start()
    {
        sunInitialIntensity = sun.intensity;
    }

    void Update()
    {
        UpdateSun();

        currentTimeOfDay += (Time.deltaTime / secondsInFullDay) * timeMultiplier;

        if (currentTimeOfDay >= 1)
        {
            currentTimeOfDay = 0;
        }
    }

    void UpdateSun()
    {
        sun.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 360f) + startPosition, -30, 0);

        float intensityMultiplier = 1;

        //if night
        if (currentTimeOfDay <= 0.23f || currentTimeOfDay >= 0.75f)
        {
            timeMultiplier = 2;

            if (isSun)
            {
                intensityMultiplier = 0;
            }
            else
            {
                intensityMultiplier = 1;
            }
        }

        //if sunrise
        else if (currentTimeOfDay <= 0.25f)
        {

            timeMultiplier = 1.5f;

            if (isSun)
            {
                intensityMultiplier = Mathf.Clamp01((currentTimeOfDay - 0.23f) * (1 / 0.02f));
            }
            else
            {
                intensityMultiplier = Mathf.Clamp01(1 - ((currentTimeOfDay - 0.73f) * (1 / 0.02f)));
            }
            
            
        }

        //if sunset
        else if (currentTimeOfDay >= 0.73f)
        {
            if (isSun)
            {
                intensityMultiplier = Mathf.Clamp01(1 - ((currentTimeOfDay - 0.73f) * (1 / 0.02f)));
            }
            else
            {
                intensityMultiplier = Mathf.Clamp01((currentTimeOfDay - 0.23f) * (1 / 0.02f));
            }
        }

        else
        {
            timeMultiplier = 1f;
        }

        

        sun.intensity = sunInitialIntensity * intensityMultiplier;
    }
}
