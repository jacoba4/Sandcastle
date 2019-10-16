using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class GameplayManager : MonoBehaviour
{
    public int maxPlayers = 4;
    public GameObject playerPrefab;
    public List<PlayerControl> players = new List<PlayerControl>();

    // Update is called once per frame
    void Update()
    {
        WatchForJoiningGame();
    }

    public void LeaveGame(PlayerControl p)
    {
        players.Remove(p);
        Destroy(p.gameObject);
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
