using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeGun : MonoBehaviour
{
    DayNightScript[] suns;

    public float normalTimeMultiplier = 1;
    public List<float> times = new List<float>(); // loop through these then go to a moving day/night cycle after you exhaust them, then loop again
    private int status = -1; // what the current status is of this gun.

    // Start is called before the first frame update
    void Start()
    {
        suns = FindObjectsOfType<DayNightScript>();
    }

    public void Fire()
    {
        // this is the function that gets called whenever you scoop when holding this item!
        status++;
        status %= times.Count + 1;
        if (status >= times.Count)
        {
            // then set the time to dynamic!
            foreach (DayNightScript t in suns)
            {
                t.timeMultiplier = normalTimeMultiplier;
            }
            Debug.Log("Time is moving!");
        }
        else
        {
            foreach (DayNightScript t in suns)
            {
                t.timeMultiplier = 0; // stop time!
                t.currentTimeOfDay = times[status];
            }
            Debug.Log("Paused time  at " + times[status]);
        }
    }
}
