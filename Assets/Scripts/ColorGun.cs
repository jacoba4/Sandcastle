using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGun : MonoBehaviour
{
    public List<Material> playerMaterials = new List<Material>();

    public void ChangePlayerColor(PlayerControl player)
    {
        // change the material used by this player!
        if (playerMaterials.Count == 0)
        {
            return; // can't set it if we don't have any materials!
        }
        Material m = playerMaterials[Random.Range(0, playerMaterials.Count)];
        player.playerBodyMeshRenderer.material = m;
    }
}
