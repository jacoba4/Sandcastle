using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGun : MonoBehaviour
{
    public List<Material> playerMaterials = new List<Material>();
    public List<Material> sunglassesMaterials = new List<Material>();
    public List<Material> handMaterials = new List<Material>();
    public List<Material> headMaterials = new List<Material>();

    public void ChangePlayerColor(PlayerControl player)
    {
        // change the material used by this player!
        if (playerMaterials.Count == 0)
        {
            return; // can't set it if we don't have any materials!
        }
        Material bodymat = RandomMaterial(playerMaterials);
        Material sunglasses = RandomMaterial(sunglassesMaterials);
        int skintone = Random.Range(0, handMaterials.Count);
        Material handTone = handMaterials[skintone];
        Material headTone = headMaterials[skintone];
        player.playerBodyMeshRenderer.material = bodymat;
        player.sunglassesRenderer.material = sunglasses;
        foreach(SkinnedMeshRenderer r in player.handRenderers)
        {
            r.material = handTone;
        }
        player.headRenderer.material = headTone;
    }

    public Material RandomMaterial(List<Material> list)
    {
        return list[Random.Range(0, list.Count)];
    }
}
