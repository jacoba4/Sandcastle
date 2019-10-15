using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerControl : MonoBehaviour
{
    Player player;
    public int playerID = 0;

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

    public WorldGrid sandWorld; // a reference to the world to place things in

    [SerializeField]
    private GameObject tempBullet;
    [SerializeField]
    private float bulletSpeed = 5f;


    // temp bucket stuff
    bool bucketFull = false;


    [ContextMenu("Get rewired player from id")]
    void TestStart()
    {
        player = Rewired.ReInput.players.GetPlayer(playerID);
    }


    // Start is called before the first frame update
    void Start()
    {
        TestStart();
        if (sandWorld == null)
        {
            // make sure we have a reference to it!
            sandWorld = FindObjectOfType<WorldGrid>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
        UpdatePlacing();
    }

    private bool IsBucketFull()
    {
        // this is a function just to abstract it in case we change how we do things around
        return bucketFull;
    }

    private void SetBucketFull(bool full)
    {
        bucketFull = full;
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
        if (player.GetButtonDown("ScoopPlaceBucket"))
        {
            // then place it!
            Vector2 character2dPos = placePosition.position;
            character2dPos.y = placePosition.position.z;
            Vector3Int pos = sandWorld.WorldtoGrid(character2dPos);
            Debug.Log("Scooping placing " + IsBucketFull() + " full?");
            // pos.z is the grid height, switching coordinate systems

            if (IsBucketFull())
            {
                // then place!
                SetBucketFull(false);
                sandWorld.AddBlock(pos.x, pos.y, 0);
            }
            else
            {
                // pickup!
                SetBucketFull(true);
                sandWorld.PopBlock(pos.x, pos.y);
            }
        }
        else if (player.GetButtonDown("PickupDropBucket"))
        {
            // 
            Debug.Log("Drop bucket");
            if (IsBucketFull())
            {
                // first empty it
                SetBucketFull(false);
            }
            else
            {
                // if it's empty then drop the bucket!
            }
        }
    }
}
