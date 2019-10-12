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
    private GameObject tempBullet;
    [SerializeField]
    private float bulletSpeed = 5f;


    [ContextMenu("Get rewired player from id")]
    void TestStart()
    {
        player = Rewired.ReInput.players.GetPlayer(playerID);
    }


    // Start is called before the first frame update
    void Start()
    {
        TestStart();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
        UpdatePlacing();
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
        if (player.GetButton("ScoopPlaceBucket"))
        {
            // Shoot things
            GameObject go = Instantiate(tempBullet);
            go.transform.position = transform.position + transform.right * 1;
            go.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, 90);
            Rigidbody rb = go.GetComponent<Rigidbody>();
            rb.velocity = -go.transform.up * bulletSpeed;
        }
        if (player.GetButtonDown("ScoopPlaceBucket"))
        {
            // then place it!
            Debug.Log("Scoop Bucket");
        }
        else if (player.GetButtonDown("PickupDropBucket"))
        {
            // 
            Debug.Log("Drop bucket");
        }
    }
}
