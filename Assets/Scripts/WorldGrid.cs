using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGrid : MonoBehaviour
{
    //WorldGrid is a 2d array of List<int>
    //
    //  The list contains all objects within a specified grid space (x,y)
    //with the first postion of the list being the ground (dug or not dug)



    public int width, height = 100;
    [SerializeField]
    private List<int>[,] grid;
    
    void Start()
    {
        Init();
    }


    //Initializes the WorldGrid
    //Each block will have a not dug ground by default
    void Init()
    {
        grid = new List<int>[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; i++)
            {
                grid[i, j] = new List<int>(0);
            }
        }
    }

    //Given a vector2 of world positions, returns a Vector3Int of the corresponding grid position and the height of the block
    Vector3Int WorldtoGrid(Vector2 worldpos)
    {
        Vector3Int gridpos = new Vector3Int();

        gridpos.x = Mathf.RoundToInt(Mathf.Floor(worldpos.x));
        gridpos.y = Mathf.RoundToInt(Mathf.Floor(worldpos.y));
        gridpos.z = grid[gridpos.x, gridpos.y].Count;

        return gridpos;
    }

    //Returns the list of items at the specified block
    List<int> GetSpot(int x, int y)
    {
        return grid[x, y];
    }

    //Adds a specified structure to the specified block
    void AddBlock(int x, int y, int block)
    {
        grid[x, y].Add(block);
    }

    //Removes the top structure from the specified block
    void PopBlock(int x, int y)
    {
        if(grid[x,y].Count == 0)
        {
            return;
        }
        else
        {
            grid[x, y].RemoveAt(grid[x, y].Count - 1);
        }
    }

    //Call this when you place a new structure
    //Checks surrounding blocks and updates the connecting walls
    void RefreshBlock(int x, int y)
    {
        for(int i = 0; i < grid[x,y].Count; i++)
        {
            if(x > 0)
            {
                //check what block is at (x-1,y)
                //For each level, check if this adjacent block needs connecting structures
                //If it does, add on adjacent and specified block
            }

            if (y > 0)
            {
                //check what block is at (x,y-1)
                //For each level, check if this adjacent block needs connecting structures
                //If it does, add on adjacent and specified block
            }

            if (x < width)
            {
                //check what block is at (x+1,y)
                //For each level, check if this adjacent block needs connecting structures
                //If it does, add on adjacent and specified block
            }

            if (y < height)
            {
                //check what block is at (x,y+1)
                //For each level, check if this adjacent block needs connecting structures
                //If it does, add on adjacent and specified block
            }
        }
    }
}
