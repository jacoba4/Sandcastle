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
    List<GameObject>[,] objectgrid;
    SaveGrid sg;
    public GameObject floor;
    public Transform worldParent; // the parent of all the cubes added to the scene so that it's organized
    
    void Start()
    {
        Init();
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.B))
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            AddBlock(x, y, 0);
        }
        /*
        if (Input.GetKeyDown(KeyCode.O))
        {
            Save();
        }*/
    }


    //Initializes the WorldGrid
    //Each block will have a not dug ground by default
    void Init()
    {
        grid = new List<int>[width, height];
        objectgrid = new List<GameObject>[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                grid[i, j] = new List<int>();
                grid[i, j].Add(0);

                objectgrid[i, j] = new List<GameObject>();
                GameObject g = Instantiate(floor, worldParent);
                g.transform.position = new Vector3(i, 0, j);
                objectgrid[i, j].Add(g);
            }
        }
    }

    //Refreshes the map, going through the grid data and creating blocks specified there
    void RefreshMap()
    {
        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                for(int k = 0; k < grid[i,j].Count; k++)
                {
                    if(grid[i,j][k] == 0)
                    {
                        GameObject g = Instantiate(floor, worldParent);
                        g.transform.position = new Vector3(i, k, j);
                        
                    }
                }
            }
        }
    }

    //Given a vector2 of world positions, returns a Vector3Int of the corresponding grid position and the height of the block
    public Vector3Int WorldtoGrid(Vector2 worldpos)
    {
        Vector3Int gridpos = new Vector3Int();

        gridpos.x = Mathf.RoundToInt(Mathf.Floor(worldpos.x));
        gridpos.y = Mathf.RoundToInt(Mathf.Floor(worldpos.y));
        gridpos.z = grid[gridpos.x, gridpos.y].Count;

        return gridpos;
    }

    //Returns the list of items at the specified block
    public List<int> GetSpot(int x, int y)
    {
        return grid[x, y];
    }

    public List<GameObject> GetSpotObject(int x, int y)
    {
        return objectgrid[x, y];
    }

    public GameObject GetSpotTop(int x, int y)
    {
        return objectgrid[x, y][objectgrid[x, y].Count - 1];
    }

    //Adds a specified structure to the specified block
    public void AddBlock(int x, int y, int block)
    {
        grid[x, y].Add(block);

        if(block == 0)
        {
            GameObject g = Instantiate(floor, worldParent);
            g.transform.position = new Vector3(x, grid[x,y].Count-1, y);
        }

    }

    //Removes the top structure from the specified block
    public void PopBlock(int x, int y)
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

    //Prints the grid in the unity console
    //NOTE: You will need to click on the message to see the whole thing
    public void PrintGrid()
    {
        print("Length: " + grid.Length);

        string temp = "\n";
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                temp += "[";
                for (int k = 0; k < grid[i,j].Count; k++)
                {
                    if(k == grid[i,j].Count-1)
                    {
                        temp += grid[i, j][k];
                    }
                    else
                    {
                        temp += grid[i, j][k] + ", ";
                    }
                }
                temp += "]";
            }
            temp += "\n";
        }

        print(temp);
    }


    //WIP
    /*
    public void Save()
    {
        sg = new SaveGrid();

        List<int>[] s = new List<int>[width * height];

        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                s[i + j * width] = grid[i,j];
            }
        }
        sg.grid = s;
        sg.width = width;
        sg.height = height;
        string json = JsonUtility.ToJson(sg);

        Debug.Log(json);
    }*/
}
