using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="BucketData", menuName = "Create Bucket Data")]
public class BucketData : ScriptableObject
{
    [Header("Data")]
    public string bucketName;
    public int bucketID = -1;
    public GameObject prefab;
    //[Tooltip("I have no clue if we're actually going to use this")]
    //public Sprite bucketSprite;
    //[Header("Also put in things like meshes here so that it's easy to find them!")]
    

    [Header("Placing rules")]
    
    public bool POFloor;
    public bool POCylinder;
    public bool POSquare;
    public bool POWall;
    public bool POGate;
    public bool POStraightWall;
    public bool POWallRoof;
    public bool POCylinderRoof;
    public bool POCylinder2Roof;
    public bool POCylinder3Roof;
    public bool POSquareRoof;
    public bool POStraightRoad;
    public bool POCurvedRoad;
    public bool POIntersectionRoad;
}
