using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGun : MonoBehaviour
{
    public GameObject bubblePrefab;
    public Vector2 randomSizeRange = Vector2.one;
    public Vector2 randomSpeedRange = Vector2.one * 2;
    public Vector2 randomDestroyRange = Vector2.one * 5;
    public Transform spawnPoint; // move in spawnpoint.forward plus some randomness I guess?

    public void Fire()
    {
        // fire the bubble!
        GameObject go = Instantiate(bubblePrefab);
        go.transform.position = spawnPoint.position;
        go.transform.localScale = Random.Range(randomSizeRange.x, randomSizeRange.y) * Vector3.one;

        float speed = Random.Range(randomSpeedRange.x, randomSpeedRange.y);
        Rigidbody rb = go.GetComponent<Rigidbody>();
        Vector3 vel = Random.onUnitSphere;
        if (vel.z < 0)
        {
            vel.z = -vel.z;
        }
        rb.velocity = spawnPoint.TransformDirection(vel) * speed;

        Destroy(go, Random.Range(randomDestroyRange.x, randomDestroyRange.y));
    }
}
