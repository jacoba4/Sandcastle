﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerControl : MonoBehaviour
{
    Player player = null;
    public int playerID = 0;
    public GameplayManager manager; // use this for joining/leaving the game

    public Transform bucketAttachment; // used for holding buckets!

    [SerializeField]
    private float playerSpeed = 1f;
    [SerializeField]
    private float inputRotation = 45f; // rotate the player input depending on how the camera is rotated...
    [SerializeField]
    private float lookmoveCutoff = .2f;
    [SerializeField]
    private float ignoreLookCutoff = .1f;
    [SerializeField]
    private Transform placePosition; // a child gameobject of the parent which is where we should place/remove sandcastle stuff

    [SerializeField]
    private float pickupBucketRange = 2f;
    [SerializeField]
    private float bucketDropForce = 17f;
    [SerializeField]
    private float pickupPercentThing = .2f;
    [SerializeField]
    private float pickupRotationPercentThing = .2f;

    private Vector3Int highlightPosition = new Vector3Int();

    public WorldGrid sandWorld; // a reference to the world to place things in
    public InventoryItems inventoryUI;

    [SerializeField]
    private GameObject tempBullet;
    [SerializeField]
    private float bulletSpeed = 5f;

    //for the noises
    //idk how to public/private change these if u want @jordan
    public AudioClip diggingSound;
    public AudioClip placingSound;
    public AudioClip footstepSound;
    AudioSource audioSource;
    float volume = 0.7f;

    private bool canMove = true;
    private bool hasBucket = false;


    private WorldBucket carryingBucket;
    private BucketData carryingBucketData;

    [SerializeField]
    private float scoopDelayTime = .7f;

    private Coroutine scoopPlaceCoroutine = null;

    public Animator playerAnimator;
    public SkinnedMeshRenderer playerBodyMeshRenderer;
    public SkinnedMeshRenderer sunglassesRenderer;
    public SkinnedMeshRenderer headRenderer;
    public List<SkinnedMeshRenderer> handRenderers = new List<SkinnedMeshRenderer>();

    // temp bucket stuff
    bool bucketFull = false;


    [ContextMenu("Get rewired player from id")]
    void TestStart()
    {
        player = Rewired.ReInput.players.GetPlayer(playerID);
    }


    public void SetPlayer(Player p)
    {
        player = p;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            TestStart();
        }
        if (sandWorld == null)
        {
            // make sure we have a reference to it!
            sandWorld = FindObjectOfType<WorldGrid>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            UpdateMovement();
        }
        UpdatePlacing();
        UpdateUI();
    }

    private bool IsBucketFull()
    {
        // this is a function just to abstract it in case we change how we do things around
        return bucketFull;
    }

    private void SetBucketFull(bool full)
    {
        bucketFull = full;
        if (carryingBucket)
        {
            carryingBucket.SetFullOfSand(full);
        }
        inventoryUI.SetFull(full);
    }

    public void PickupBucket()
    {
        // used for UI stuff
        inventoryUI.SetBucketPickedUp(carryingBucketData);
    }

    public void DropBucket()
    {
        // used for UI stuff
        inventoryUI.SetBucketPickedUp(null);
    }

    public void DisconnectPlayer()
    {
        player.isPlaying = false;
        manager.LeaveGame(this);
    }

    private void UpdateUI()
    {
        if (player.GetButtonDown("LeaveGame")) {
            // disconnect!
            DisconnectPlayer();
        }
    }

    private void UpdateMovement()
    {
        Vector2 movementInput = new Vector2(player.GetAxis("MoveHorizontal"), player.GetAxis("MoveVertical"));
        Vector2 lookInput = new Vector2(player.GetAxis("LookHorizontal"), player.GetAxis("LookVertical"));

        if (lookInput.magnitude < ignoreLookCutoff && (movementInput.magnitude < lookmoveCutoff))
        {
            // look input = movement input and don't move if the thumbstick isn't pushed very much
            lookInput = movementInput;
            movementInput = Vector2.zero;
        }

        // then move and look around based on those inputs!
        movementInput = Quaternion.Euler(0, 0, inputRotation) * movementInput;
        lookInput = Quaternion.Euler(0, 0, inputRotation) * lookInput;
        transform.position += Vector3.right * playerSpeed * movementInput.x + Vector3.forward * playerSpeed * movementInput.y;
        playerAnimator.SetBool("Walking", movementInput.sqrMagnitude > 0);

        float lookAngle = 0;
        if (lookInput.magnitude > ignoreLookCutoff)
        {
            // then look around with the right stick!
            lookAngle = Mathf.Atan2(-lookInput.y, lookInput.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, lookAngle, 0);
        }
        else if (movementInput.magnitude > ignoreLookCutoff)
        {
            lookAngle = Mathf.Atan2(-movementInput.y, movementInput.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, lookAngle, 0);
        }
    }

    private void UpdatePlacing()
    {
        //if (player.GetButton("ScoopPlaceBucket"))
        //{
        //    // Shoot things
        //    GameObject go = Instantiate(tempBullet);
        //    go.transform.position = transform.position + transform.right * 1;
        //    go.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, 90);
        //    Rigidbody rb = go.GetComponent<Rigidbody>();
        //    rb.velocity = -go.transform.up * bulletSpeed;
        //}

        // highlight blocks that we're using!
        sandWorld.SetPlayer(gameObject);
        Vector2 character2dPos = placePosition.position;
        character2dPos.y = placePosition.position.z;
        Vector3Int pos = sandWorld.WorldtoGrid(character2dPos);

        if (pos != highlightPosition)
        {
            // de-highlight the highlight position
            sandWorld.UnHighlightBlock(highlightPosition.x, highlightPosition.y);
            highlightPosition.Set(pos.x, pos.y, pos.z - 1);
        }

        if (player.GetButtonDown("ScoopPlaceBucket"))
        {
            // then place it!
            //Debug.Log("Scooping placing " + IsBucketFull() + " full?");
            // pos.z is the grid height, switching coordinate systems
            if (hasBucket)
            {
                if (scoopPlaceCoroutine == null)
                {
                    if (carryingBucket.isSpecialItem)
                    {
                        if (carryingBucket.specialItemScoopAnimation != WorldBucket.SpecialItemAnimation.None)
                        {
                            scoopPlaceCoroutine = StartCoroutine(AnimatedUseSpecialBucketAfterTime(scoopDelayTime, pos));
                        } else
                        {
                            UseSpecialBucket(pos);
                        }
                    }
                    else
                    {
                        scoopPlaceCoroutine = StartCoroutine(ScoopPlaceAfterTime(scoopDelayTime, pos));
                    }
                }
            }
            else
            {
                // try picking up a bucket! for now just do it...
                WorldBucket wb = manager.GetClosestBucket(transform.position, pickupBucketRange);
                if (wb != null)
                {
                    wb.beingHeld = true; // so that other players can't pick it up
                    hasBucket = true;
                    playerAnimator.SetBool("HoldingBucket", true);
                    carryingBucket = wb;
                    carryingBucketData = wb.bucket;
                    // also do UI stuff
                    wb.Pickup();
                    StartCoroutine(PickUpBucketAfterTime(.25f));
                    PickupBucket();
                }
            }
        }
        else if (player.GetButtonDown("PickupDropBucket"))
        {
            // 
            //Debug.Log("Drop bucket");
            if (IsBucketFull())
            {
                // first empty it
                SetBucketFull(false);
            }
            else
            {
                // if it's empty then drop the bucket!
                //playerAnimator.SetBool("HasBucket", false);
                hasBucket = false;
                playerAnimator.SetBool("HoldingBucket", false);
                if (carryingBucket != null)
                {
                    // if it's currently carrying a bucket then drop it
                    // do UI stuff
                    // then drop it!
                    carryingBucket.Drop();
                    carryingBucket.transform.parent = null;
                    Vector3 bucketForce = Random.onUnitSphere;
                    if (bucketForce.y < .4)
                    {
                        bucketForce.y = .4f;
                    }
                    bucketForce *= bucketDropForce;
                    carryingBucket.rb.AddForce(bucketForce, ForceMode.Acceleration);
                    carryingBucket = null;
                    carryingBucketData = null;
                    DropBucket();
                }
            }
        }
        sandWorld.HighlightBlock(pos.x, pos.y);
    }

    private void UseSpecialBucket(Vector3Int pos)
    {
        if (carryingBucket == null)
        {
            return; // return early, perhaps they dropped it during the animation?
        }
        if (carryingBucket.unHighlightPosition)
        {
            sandWorld.UnHighlightBlock(pos.x, pos.y);
        }
        carryingBucket.InvokeSpecialEvent(this);
        if (carryingBucket.isSingleUse)
        {
            carryingBucket.Drop();
            carryingBucket.transform.parent = null;
            carryingBucket.transform.position = manager.RandomWorldBucketPosition(); // randomize the position after use
            carryingBucket = null;
            carryingBucketData = null;
            DropBucket();

            hasBucket = false;
            playerAnimator.SetBool("HoldingBucket", false);
        }
    }

    private IEnumerator AnimatedUseSpecialBucketAfterTime(float t, Vector3Int pos)
    {
        if (carryingBucket == null)
        {
            yield break; // can't do this so leave!
        }

        canMove = false;
        playerAnimator.SetBool("Walking", false);
        if (carryingBucket.specialItemScoopAnimation == WorldBucket.SpecialItemAnimation.Place)
        {
            playerAnimator.SetTrigger("Place");
        }
        else if (carryingBucket.specialItemScoopAnimation == WorldBucket.SpecialItemAnimation.Scoop)
        {
            playerAnimator.SetTrigger("Scoop");
        }

        yield return new WaitForSeconds(t);

        UseSpecialBucket(pos);
        //carryingBucket.specialItemEvent.Invoke();
        //if (IsBucketFull())
        //{
        //    //Check if current bucket is placeable on selected block
        //    bool p = false;
        //    int s = sandWorld.GetSpot(pos.x, pos.y)[sandWorld.GetSpot(pos.x, pos.y).Count - 1];
        //    p = carryingBucketData.Placeable(s);

        //    //Debug.Log("Carrying: " + carryingBucketData.bucketID);
        //    //Debug.Log("Spot: " + s);
        //    //Debug.Log("Placeable?: " + p);
        //    if (p)
        //    {
        //        SetBucketFull(false);
        //        sandWorld.AddBlock(pos.x, pos.y, carryingBucketData.bucketID);
        //        //plays placing sound effect
        //        PlaySoundEffect(placingSound, volume);
        //    }

        //}
        //else
        //{
        //    // pickup!
        //    SetBucketFull(true);
        //    sandWorld.PopBlock(pos.x, pos.y);
        //    //plays digging sound effect
        //    PlaySoundEffect(diggingSound, volume);
        //}
        yield return new WaitForSeconds(t / 2);
        scoopPlaceCoroutine = null; // let people scoop and place again!
        canMove = true;
    }

    private IEnumerator PickUpBucketAfterTime(float t)
    {
        //float percent = 0;
        //float startTime = Time.time;
        Vector3 delta = Vector3.one;
        int i = 0;
        //Quaternion deltaRot;
        while (delta.magnitude > .01f && i < 100)
        {
            yield return null; // wait a frame
            //percent = (Time.time - startTime) / t;
            if (carryingBucket == null)
            {
                break; // leave early since we dropped the bucket
            }
            delta = bucketAttachment.position - carryingBucket.transform.position;
            delta *= pickupPercentThing * Time.deltaTime;
            //deltaRot = bucketAttachment.rotation * Quaternion.Inverse(carryingBucket.transform.rotation);
            carryingBucket.transform.rotation = Quaternion.Slerp(carryingBucket.transform.rotation, bucketAttachment.rotation, pickupRotationPercentThing);
            carryingBucket.transform.position += delta;
            i++;
            //Debug.Log("Delta: " + delta.magnitude);
        }
        //yield return new WaitForSeconds(t);
        if (carryingBucket != null)
        {
            // i.e. if we haven't really quickly dropped it for some reason...
            carryingBucket.transform.parent = bucketAttachment;
            carryingBucket.transform.localPosition = Vector3.zero;
            carryingBucket.transform.localRotation = Quaternion.identity;
        }
    }

    public void UpgradeFacingBlock(bool isSeaweed)
    {
        Vector2 character2dPos = placePosition.position;
        character2dPos.y = placePosition.position.z;
        Vector3Int pos = sandWorld.WorldtoGrid(character2dPos);
        Debug.LogError("CUrrently unable to upgrade thigns!");
    }

    private IEnumerator ScoopPlaceAfterTime(float t, Vector3Int pos)
    {
        Debug.Log("Placing stuff?");
        canMove = false;
        playerAnimator.SetBool("Walking", false);
        if (IsBucketFull())
        {
            playerAnimator.SetTrigger("Place");
        }
        else
        {
            playerAnimator.SetTrigger("Scoop");
        }

        yield return new WaitForSeconds(t);

        sandWorld.UnHighlightBlock(pos.x, pos.y);
        if (IsBucketFull())
        {
            //Check if current bucket is placeable on selected block
            bool p = false;
            int s = sandWorld.GetSpot(pos.x, pos.y)[sandWorld.GetSpot(pos.x, pos.y).Count - 1];
            switch (s)
            {
                case 0:
                    p = carryingBucketData.POFloor;
                    break;
                case 1:
                    p = carryingBucketData.POCylinder;
                    break;
                case 2:
                    p = carryingBucketData.POSquare;
                    break;
                case 3:
                    p = carryingBucketData.POWall;
                    break;
                case 4:
                    p = carryingBucketData.POGate;
                    break;
                case 5:
                    p = carryingBucketData.POStraightWall;
                    break;
                case 6:
                    p = carryingBucketData.POWallRoof;
                    break;
                case 7:
                    p = carryingBucketData.POCylinderRoof;
                    break;
                case 8:
                    p = carryingBucketData.POCylinder2Roof;
                    break;
                case 9:
                    p = carryingBucketData.POCylinder3Roof;
                    break;
                case 10:
                    p = carryingBucketData.POSquareRoof;
                    break;
                case 11:
                    p = carryingBucketData.POStraightRoad;
                    break;
                case 12:
                    p = carryingBucketData.POCurvedRoad;
                    break;
                case 13:
                    p = carryingBucketData.POIntersectionRoad;
                    break;
            }
            

            //Debug.Log("Carrying: " + carryingBucketData.bucketID);
            //Debug.Log("Spot: " + s);
            //Debug.Log("Placeable?: " + p);
            if(p)
            {
                SetBucketFull(false);
                sandWorld.AddBlock(pos.x, pos.y, carryingBucketData.bucketID);
                //plays placing sound effect
                PlaySoundEffect(placingSound, volume);
                Debug.Log("added stuff?");

            }

        }
        else
        {
            // pickup!
            SetBucketFull(true);
            sandWorld.PopBlock(pos.x, pos.y);
            //plays digging sound effect
            PlaySoundEffect(diggingSound, volume);
            Debug.Log("Popped stuff?");
        }
        yield return new WaitForSeconds(t/2);
        scoopPlaceCoroutine = null; // let people scoop and place again!
        canMove = true;
    }

    private void OnEnable()
    {
        // add yourself to the camera system
        SplitScreenRects split = GameObject.FindObjectOfType<SplitScreenRects>();
        if (split != null)
        {
            split.AddPlayer(transform);
        }
    }

    private void OnDisable()
    {
        // remove yourself from the camera system
        SplitScreenRects split = GameObject.FindObjectOfType<SplitScreenRects>();
        if (split != null)
        {
            split.RemovePlayer(transform);
        }
        // remove any highlighting you added when you leave
        sandWorld.UnHighlightBlock(highlightPosition.x, highlightPosition.y);
    }

    //plays sound effects at a volume
    public void PlaySoundEffect(AudioClip soundEffect, float volume)
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(soundEffect, volume);
    }

    //halts player controls when game paused
    public void DisablePlayerMovement()
    {
        canMove = false;
    }

    //re-enables player controls
    public void EnablePlayerMovement()
    {
        canMove = true;
    }
}
