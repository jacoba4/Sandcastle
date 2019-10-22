using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationGun : MonoBehaviour
{
    public bool isSeaWeed = true;

    public void Fire(PlayerControl player)
    {
        player.UpgradeFacingBlock(isSeaWeed);
    }
}
