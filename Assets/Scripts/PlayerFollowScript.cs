using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollowScript : MonoBehaviour
{
    public Transform toFollow;
    public float followDistance = 10f;

    // Update is called once per frame
    void Update()
    {
        transform.position = toFollow.position + transform.forward * -followDistance;
    }
}
