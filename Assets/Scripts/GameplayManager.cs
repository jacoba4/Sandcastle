using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class GameplayManager : MonoBehaviour
{
    public int maxPlayers = 4;
    public int copiesOfEachBucket = 5;
    public GameObject playerPrefab;
    public List<GameObject> bucketPrefabs = new List<GameObject>(); // list of prefabs to spawn of each bucket
    public List<PlayerControl> players = new List<PlayerControl>();
    public List<WorldBucket> buckets = new List<WorldBucket>();
    public WorldGrid sandWorld;

    public int copiesOfSpecialBuckets = 3;
    public List<GameObject> specialBucketPrefabs = new List<GameObject>(); // list of prefabs to spawn of each bucket


    private void Start()
    {
        SpawnBuckets();
    }

    // Update is called once per frame
    void Update()
    {
        WatchForJoiningGame();
    }

    public void DisconnectAllPlayers()
    {
        for (int i = players.Count - 1; i >= 0; i--)
        {
            players[i].DisconnectPlayer();
        }
    }

    public void LeaveGame(PlayerControl p)
    {
        players.Remove(p);
        Destroy(p.gameObject);
    }

    [ContextMenu("Spawn buckets")]
    private void SpawnBuckets()
    {
        DestroyBuckets();
        for (int i = 0; i < bucketPrefabs.Count; i++)
        {
            for (int j = 0; j < copiesOfEachBucket; j++)
            {
                // spawn the bucket prefabs!
                GameObject g = Instantiate(bucketPrefabs[i]);
                // move it around somewhere!

                g.transform.position = new Vector3(Random.Range(0f, sandWorld.width), 10, Random.Range(0f, sandWorld.height));

                // then initialize it and store it!
                WorldBucket b = g.GetComponent<WorldBucket>();
                b.Initialize(this);
                buckets.Add(b);
            }
        }

        for (int i = 0; i < specialBucketPrefabs.Count; i++)
        {
            for (int j = 0; j < copiesOfSpecialBuckets; j++)
            {
                // spawn the bucket prefabs!
                GameObject g = Instantiate(specialBucketPrefabs[i]);
                // move it around somewhere!

                g.transform.position = new Vector3(Random.Range(0f, sandWorld.width), 10, Random.Range(0f, sandWorld.height));

                // then initialize it and store it!
                WorldBucket b = g.GetComponent<WorldBucket>();
                b.Initialize(this);
                buckets.Add(b);
            }
        }
    }

    public WorldBucket GetClosestBucket(Vector3 pos, float maxRange = -1)
    {
        WorldBucket closest = null;
        float closestDistance = maxRange; // if max Range is -1 then it has no max range, otherwise it uses that as the max range duh.
        for (int i = 0; i < buckets.Count; i++)
        {
            float d = Vector3.Distance(pos, buckets[i].transform.position);
            if (!buckets[i].beingHeld && (closestDistance == -1 || d < closestDistance))
            {
                // then you can pick it up!
                closest = buckets[i];
                closestDistance = d;
            }
        }

        return closest;
    }

    private void DestroyBuckets()
    {
        // in the off chance that we managed to create lots of buckets somehow, destroy old buckets
        foreach(WorldBucket go in buckets)
        {
            Destroy(go.gameObject);
        }
        buckets = new List<WorldBucket>();
    }

    public void WatchForJoiningGame()
    {
        // monitor the players and wait for a player pressing any key
        foreach (Player p in ReInput.players.AllPlayers)
        {
            if (players.Count < maxPlayers)
            {
                if (!p.isPlaying)
                {
                    // then check if they're pressing anything!
                    if (p.GetAnyButtonDown() || p.GetAnyNegativeButtonDown())
                    {
                        p.isPlaying = true;
                        // create the player!
                        GameObject player = Instantiate(playerPrefab);
                        PlayerControl c = player.GetComponent<PlayerControl>();
                        c.manager = this;
                        c.SetPlayer(p);
                        players.Add(c);
                    }
                }
            }
        }
    }
}
