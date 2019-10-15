using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="BucketData", menuName = "Create Bucket Data")]
public class BucketData : ScriptableObject
{
    public string bucketName;
    public int bucketID = -1;
    public Sprite bucketSprite;
    [Header("Also put in things like meshes here so that it's easy to find them!")]
    [Tooltip("I have no clue if we're actually going to use this")]
    public float bucketRarity = 1;
}
