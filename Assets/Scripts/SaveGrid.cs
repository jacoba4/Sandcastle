using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveGrid
{
    public SaveGridList[] grid;
    public int width;
    public int height;
    public SaveGridList[] upgradegrid;
}

[System.Serializable]
public class SaveGridList
{
    public List<int> objects;

    public SaveGridList(List<int> l)
    {
        objects = l;
    }
}
