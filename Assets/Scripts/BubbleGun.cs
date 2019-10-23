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
    public Vector2 timeBetweenBubbles = Vector2.one * .25f;
    public bool stopShootingWhenDropped = true;

    private WorldBucket bubbleGunBucket;

    private float timer = 0;

    public bool shooting = false;

    private void Start()
    {
        bubbleGunBucket = GetComponent<WorldBucket>();
    }


    private void Update()
    {
        if (shooting)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                // then shoot!
                Fire();
                timer = Random.Range(timeBetweenBubbles.x, timeBetweenBubbles.y);
            }
            if (stopShootingWhenDropped)
            {
                if (bubbleGunBucket != null && !bubbleGunBucket.beingHeld)
                {
                    shooting = false;
                }
            }
        }
    }

    public void ToggleShooting()
    {
        shooting = !shooting;
        timer = 0;
    }

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

        // make sure it pops at some point:
        go.GetComponent<BubbleCollision>().PopAfterTime(Random.Range(randomDestroyRange.x, randomDestroyRange.y));
    }
}
