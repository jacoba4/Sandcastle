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

    public bool Placeable(int s)
    {
        int c = bucketID;
        switch (c)
        {
            //Bucket is ground
            case 0:
                //Can be placed on floor and square
                switch (s)
                {
                    case 0:
                        return (true);
                    case 1:
                        return (false);
                    case 2:
                        return (true);
                    case 3:
                        return (false);
                    case 4:
                        return (false);
                }
                break;

            //Bucket is Cylinder
            case 1:
                //Can be placed on floor, cylinder, and square
                switch (s)
                {
                    case 0:
                        return (true);
                    case 1:
                        return (true);
                    case 2:
                        return (true);
                    case 3:
                        return (false);
                    case 4:
                        return (false);
                }
                break;
            //Bucket is Square
            case 2:
                //Can be placed on floor and square
                switch (s)
                {
                    case 0:
                        return (true);
                    case 1:
                        return (false);
                    case 2:
                        return (true);
                    case 3:
                        return (false);
                    case 4:
                        return (false);
                }
                break;
            //Bucket is wall
            case 3:
                //Can be placed on floor, square, wall, and gate
                switch (s)
                {
                    case 0:
                        return (true);
                    case 1:
                        return (false);
                    case 2:
                        return (true);
                    case 3:
                        return (true);
                    case 4:
                        return (true);
                }
                break;
            //Bucket is gate
            case 4:
                //Can be placed on floor
                switch (s)
                {
                    case 0:
                        return (true);
                    case 1:
                        return (false);
                    case 2:
                        return (false);
                    case 3:
                        return (false);
                    case 4:
                        return (false);
                }
                break;

        }
        return false;
    }
}
