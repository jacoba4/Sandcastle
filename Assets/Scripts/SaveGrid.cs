using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveGrid
{
    [SerializeField]
    public List<int>[] grid;
    public int width;
    public int height;
}
