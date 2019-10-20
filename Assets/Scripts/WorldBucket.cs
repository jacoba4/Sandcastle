using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBucket : MonoBehaviour
{
    public BucketData bucket; // the bucket data that's associated with this!
    public List<GameObject> colliders = new List<GameObject>(); // things to disable
    public List<GameObject> fullMeshes = new List<GameObject>(); // things to disable
    private GameplayManager manager;
    public bool beingHeld = false;
    public Rigidbody rb;


    private void Start()
    {
        SetFullOfSand(false);
    }

    public void Pickup()
    {
        foreach (GameObject go in colliders)
        {
            go.SetActive(false);
        }
        beingHeld = true;
        rb.isKinematic = true;
    }

    public void Drop()
    {
        foreach (GameObject go in colliders)
        {
            go.SetActive(true);
        }
        beingHeld = false;
        rb.isKinematic = false;
    }

    public void SetFullOfSand(bool full)
    {
        foreach(GameObject go in fullMeshes)
        {
            go.SetActive(full);
        }
    }

    private void OnDestroy()
    {
        manager.buckets.Remove(this);
    }

    public void Initialize(GameplayManager gameplayManger)
    {
        manager = gameplayManger;
    }
}
