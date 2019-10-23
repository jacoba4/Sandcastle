using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class WorldBucket : MonoBehaviour
{
    public BucketData bucket; // the bucket data that's associated with this!
    public List<GameObject> colliders = new List<GameObject>(); // things to disable
    public List<GameObject> fullMeshes = new List<GameObject>(); // things to disable
    private GameplayManager manager;
    public bool beingHeld = false;
    public Rigidbody rb;

    [Space]
    public bool callSpecialFunctionInsteadOfPlace = false;
    [Header("Special Bucket Settings")]
    public bool unHighlightPosition = true; // unhighlight the position by default
    public bool isSpecialItem = false; // if it's a special item then do things!
    public bool isSingleUse = false; // true only for shells and seaweed and decorations
    public SpecialItemAnimation specialItemScoopAnimation = SpecialItemAnimation.None; // if it's a special item should it use the usual animation?
    [System.Serializable] public class UnityUseItemEvent : UnityEvent<PlayerControl> { };
    public UnityUseItemEvent specialItemEvent; // invoke this!


    private Rect bounds = new Rect(0, -20, 50, 240);

    private void Start()
    {
        SetFullOfSand(false);
    }

    public void InvokeSpecialEvent(PlayerControl p)
    {
        specialItemEvent.Invoke(p);
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

    private void Update()
    {
        if (!beingHeld)
        {
            // teleport into bounds if out of bounds!
            if (transform.position.x < bounds.xMin || transform.position.x > bounds.xMax || transform.position.z < bounds.yMin || transform.position.z > bounds.yMax)
            {
                // teleport to a new place!
                //transform.position = manager.RandomWorldBucketPosition();
                Vector3 clippedPos = transform.position;
                clippedPos.x = Mathf.Max(bounds.xMin, Mathf.Min(bounds.xMax, clippedPos.x));
                clippedPos.z = Mathf.Max(bounds.yMin, Mathf.Min(bounds.yMax, clippedPos.z));
                transform.position = clippedPos;    
            }
        }
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

    public enum SpecialItemAnimation
    {
        None, Scoop, Place
    }

    public void OnCollisionEnter(Collision collision)
    {
        GetComponent<AudioSource>().Play();
    }
}
