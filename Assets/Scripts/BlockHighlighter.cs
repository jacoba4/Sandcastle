using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHighlighter : MonoBehaviour
{

    public List<MeshRenderer> renderersToReplace = new List<MeshRenderer>();
    public List<Material> originalMaterials = new List<Material>();

    [ContextMenu("Find all renderers")]
    public void FindAllRenderers()
    {
        renderersToReplace.Clear();
        originalMaterials.Clear();

        renderersToReplace.AddRange(gameObject.GetComponentsInChildren<MeshRenderer>());
        CopyMats();
    }

    [ContextMenu("Copy mats")]
    public void CopyMats()
    {
        originalMaterials.Clear();
        for (int i = 0; i < renderersToReplace.Count; i++)
        {
            originalMaterials.Add(renderersToReplace[i].sharedMaterial);
        }
    }

    public void HighlightMat(Material m)
    {
        for (int i = 0; i < renderersToReplace.Count; i++)
        {
            renderersToReplace[i].material = m;
        }
    }

    public void UnHighlight()
    {
        for (int i = 0; i < renderersToReplace.Count; i++)
        {
            renderersToReplace[i].material = originalMaterials[i];
        }
    }
}
