using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class WorldGrid : MonoBehaviour
{
    //WorldGrid is a 2d array of List<int>
    //
    //  The list contains all objects within a specified grid space (x,y)
    //with the first postion of the list being the ground (dug or not dug)


    [Header("Data")]
    public int width = 100;
    public int height = 100;
    public List<BucketData> BucketData;
    [SerializeField]
    private List<int>[,] grid;
    List<GameObject>[,] objectgrid;
    SaveGrid sg;
    [Header("Prefabs")]
    public GameObject floor;
    public GameObject cylinder;
    public GameObject square;
    public GameObject wall;
    public GameObject gate;

    [Header("Materials")]
    [Tooltip("Regular material")]
    public Material umat;
    [Tooltip("Highlighted material")]
    public Material hmat;

    [Header("References")]
    public TMP_InputField textbox;
    public Transform worldParent; // the parent of all the cubes added to the scene so that it's organized
    public GameObject player;

    [Header("Paths")]
    public GameObject straightPath;
    public GameObject curvedPath;
    public GameObject tJunctionPath;
    public GameObject intersectionPath;

    public List<int> pathIDs = new List<int>(); // this is used to save all paths as one type of path I guess?

    [Space]
    public BucketData cubeBucketdata;
    
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
            AddBlock(x, y, BucketData[Random.Range(0, BucketData.Count)]);
        }
        
        if (Input.GetKeyDown(KeyCode.O))
        {
            //Save();
        }
    }

    public bool IsTopBlockAPath(Vector3Int pos)
    {
        if (!WithinBounds(pos.x, pos.y))
        {
            return false; // it's outside the bounds it's not a path duh.
        }
        if (grid[pos.x, pos.y].Count == 0)
        {
            return false;
        }
        return pathIDs.Contains(grid[pos.x, pos.y][grid[pos.x, pos.y].Count - 1]); // is the top thing a path?
    }

    //Checks if the position is within the bounds of the world
    public bool WithinBounds(Vector3Int pos)
    {
        if (WithinBounds(pos.x, pos.y))
        {
            // then check to see if the height is ok
            if (pos.z >= 0 && pos.z <= grid[pos.x, pos.y].Count)
            {
                return true;
            }
        }
        return false;
    }

    //Checks if the position is within the bounds of the world
    public bool WithinBounds(Vector2Int pos)
    {
        return WithinBounds(pos.x, pos.y);
    }

    //Checks if the position is within the bounds of the world
    public bool WithinBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
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
                for(int k = 1; k < grid[i,j].Count; k++)
                {
                    GameObject g = null;
                    if(grid[i,j][k] == 0)
                    {
                        g = Instantiate(floor, worldParent);           
                    }
                    if(grid[i,j][k] == 1)
                    {
                        g = Instantiate(cylinder, worldParent);                     
                    }
                    if (grid[i, j][k] == 2)
                    {
                        g = Instantiate(square, worldParent);                       
                    }
                    if (grid[i, j][k] == 3)
                    {
                        g = Instantiate(wall, worldParent);                       
                    }
                    if (grid[i, j][k] == 4)
                    {
                        g = Instantiate(gate, worldParent);
                    }

                    if (pathIDs.Contains(grid[i, j][k]))
                    {
                        // then spawn a random path object I guess?
                        g = Instantiate(straightPath, worldParent);
                    }

                    if (grid[i,j][k] != 0)
                    {
                        if(grid[i,j][k] == 1 || grid[i, j][k] == 2)
                        {
                            int rot = Random.Range(0, 4);
                            float yrot = 0;
                            switch (rot)
                            {
                                case 1:
                                    yrot = 90;
                                    return;
                                case 2:
                                    yrot = 180;
                                    return;
                                case 3:
                                    yrot = 270;
                                    return;

                            }
                            g.transform.eulerAngles = new Vector3(g.transform.rotation.x, yrot, g.transform.rotation.z);
                        }
                        g.transform.position = new Vector3(i, k, j);
                    }
                    else
                    {
                        g.transform.position = new Vector3(i, k, j);
                    }
                    //g.transform.position = new Vector3(i, k, j);
                    objectgrid[i, j].Add(g);

                    if (pathIDs.Contains(grid[i, j][k]))
                    {
                        // update surroundings and yourself!
                        UpdatePath(i, j, k, true);
                    }
                }
            }
        }
    }

    //Given a vector2 of world positions, returns a Vector3Int of the corresponding grid position and the height of the block
    public Vector3Int WorldtoGrid(Vector2 worldpos)
    {
        worldpos += Vector2.one * .5f; // offset it
        Vector3Int gridpos = new Vector3Int();

        gridpos.x = Mathf.RoundToInt(Mathf.Floor(worldpos.x));
        gridpos.y = Mathf.RoundToInt(Mathf.Floor(worldpos.y));
        if (!WithinBounds(gridpos.x, gridpos.y))
        {
            gridpos.z = -1;
        }
        else
        {
            gridpos.z = grid[gridpos.x, gridpos.y].Count;
        }

        //Debug.Log("World: " + worldpos);
        //Debug.Log("Grid: " + gridpos);

        return gridpos;
    }

    //Returns the list of items at the specified block
    public List<int> GetSpot(int x, int y)
    {
        if (!WithinBounds(x, y))
        {
            return null; // outside of bounds
        }
        return grid[x, y];
    }

    //Returns the list of GameObjects at the given spot
    public List<GameObject> GetSpotObject(int x, int y)
    {
        if (!WithinBounds(x, y))
        {
            return null; // outside of bounds
        }
        return objectgrid[x, y];
    }

    //Returns the top block at a given spot
    public GameObject GetSpotTop(int x, int y)
    {
        if (!WithinBounds(x, y))
        {
            return null; // outside of bounds
        }
        return objectgrid[x, y][objectgrid[x, y].Count - 1];
    }

    public void SetPlayer(GameObject g)
    {
        player = g;
    }

    private bool IsPath(int x, int y, int z)
    {
        return WithinBounds(x, y) && (grid[x, y].Count > z) && pathIDs.Contains(grid[x, y][z]);
    }

    private void UpdatePath(int x, int y, int z, bool updateSurroundings = false)
    {
        if (!WithinBounds(x, y))
        {
            return;
        }

        bool up = IsPath(x, y + 1, z);
        bool down = IsPath(x, y - 1, z);
        bool left = IsPath(x - 1, y, z);
        bool right = IsPath(x + 1, y, z);

        if (grid[x, y].Count <= z)
        {
            // then we removed the path!, so only do neighbors!
        }
        else
        {
            int connectionCount = 0;
            if (up) connectionCount++;
            if (down) connectionCount++;
            if (left) connectionCount++;
            if (right) connectionCount++;

            //string debugLog = "Found a connection, pos = x:" + x + " y:" + y + " z:" + z + " count = " + connectionCount + ":" + up + down + left + right + ": adding ";

            if (connectionCount == 0)
            {
                //Debug.Log(debugLog);
                return; // no need to change, there are no paths nearby
            }
            else if ((connectionCount == 1) || (connectionCount == 2 && ((up && down) || (left && right))))
            {
                // if there's one connection, or if there's two and it's a straight line
                // make it straight and rotate it to the correct direction
                Destroy(objectgrid[x, y][z]); // this is inefficient but I can deal.
                objectgrid[x, y][z] = Instantiate(straightPath, worldParent);
                objectgrid[x, y][z].transform.position = new Vector3(x, z, y);
                if (up || down)
                {
                    // then rotate it vertically! (I changed this from before to match the player rotation better)  
                    objectgrid[x, y][z].transform.eulerAngles = new Vector3(0, 90, 0);
                }
            }
            else if (connectionCount == 2)
            {
                // then make it a curve and rotate it
                Destroy(objectgrid[x, y][z]); // this is inefficient but I can deal.
                objectgrid[x, y][z] = Instantiate(curvedPath, worldParent);
                objectgrid[x, y][z].transform.position = new Vector3(x, z, y);
                if (left && down)
                {
                    // don't rotate it
                }
                else if (down && right)
                {
                    objectgrid[x, y][z].transform.eulerAngles = new Vector3(0, -90, 0);
                }
                else if (right && up)
                {
                    objectgrid[x, y][z].transform.eulerAngles = new Vector3(0, 180, 0);
                }
                else if (left && up)
                {
                    objectgrid[x, y][z].transform.eulerAngles = new Vector3(0, 90, 0);
                }
            }
            else if (connectionCount == 3)
            {
                Destroy(objectgrid[x, y][z]); // this is inefficient but I can deal.
                objectgrid[x, y][z] = Instantiate(tJunctionPath, worldParent);
                objectgrid[x, y][z].transform.position = new Vector3(x, z, y);
                // rotate it!
                if (!right)
                {
                    // don't rotate it!
                }
                else if (!down)
                {
                    objectgrid[x, y][z].transform.eulerAngles = new Vector3(0, 90, 0);
                }
                else if (!left)
                {
                    objectgrid[x, y][z].transform.eulerAngles = new Vector3(0, 180, 0);
                }
                else if (!up)
                {
                    objectgrid[x, y][z].transform.eulerAngles = new Vector3(0, -90, 0);
                }
            }
            else if (connectionCount == 4)
            {
                Destroy(objectgrid[x, y][z]); // this is inefficient but I can deal.
                objectgrid[x, y][z] = Instantiate(intersectionPath, worldParent);
                objectgrid[x, y][z].transform.position = new Vector3(x, z, y);
            }
            //if (updateSurroundings)
            //{
            //    Debug.Log(debugLog);
            //}
        }

        if (updateSurroundings)
        {
            if (right) UpdatePath(x + 1, y, z, false);
            if (left) UpdatePath(x - 1, y, z, false);
            if (up) UpdatePath(x, y + 1, z, false);
            if (down) UpdatePath(x, y - 1, z, false);
        }
    }

    //Adds a specified structure to the specified block
    public void AddBlock(int x, int y, BucketData bucketData)
    {
        int block = bucketData.bucketID;


        if (!WithinBounds(x, y))
        {
            return; // outside of bounds
        }

        GameObject g = null;


        g = Instantiate(BucketData[block].prefab, worldParent);

        objectgrid[x, y].Add(g);
        grid[x, y].Add(block);

        if (pathIDs.Contains(block))
        {
            // it's a path!
            // if we're placing it then update it I guess? We rotate if there's nothing else attached to it I guess?
            g.transform.position = new Vector3(x, grid[x, y].Count - 1f, y);
            g.transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Round(player.transform.eulerAngles.y / 90) * 90f, transform.eulerAngles.z);

            UpdatePath(x, y, grid[x, y].Count - 1, true);
        }
        else if(bucketData.rotateWithPlayer)
        {
            g.transform.position = new Vector3(x, grid[x, y].Count - 1f, y);
            g.transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Round(player.transform.eulerAngles.y / 90) * 90f, transform.eulerAngles.z);
        }
        else if(bucketData.randomRotation)
        {
            int rot = Random.Range(0, 4);
            float yrot = 0;
            switch (rot)
            {
                case 1:
                    yrot = 90;
                    break;
                case 2:
                    yrot = 180;
                    break;
                case 3:
                    yrot = 270;
                    break;

            }
            g.transform.eulerAngles = new Vector3(g.transform.rotation.x, yrot, g.transform.rotation.z);
            g.transform.position = new Vector3(x, grid[x, y].Count - 1f, y);
        }
        else
        {
            g.transform.position = new Vector3(x, grid[x, y].Count - 1f, y);
        }
        
        //Debug.Log(g.transform.position);

    }

    //Removes the top structure from the specified block
    public void PopBlock(int x, int y)
    {
        if(!CheckBlock(x,y))
        {
            return;
        }
        if (objectgrid[x, y].Count > 0)
        {
            Destroy(objectgrid[x, y][objectgrid[x, y].Count - 1]);
            objectgrid[x, y].RemoveAt(objectgrid[x, y].Count - 1);
        }

        if (grid[x,y].Count == 0)
        {
            return;
        }
        else
        {
            int block = grid[x, y][grid[x, y].Count - 1];
            if (pathIDs.Contains(block))
            {
                grid[x, y].RemoveAt(grid[x, y].Count - 1);

                // it's a path!

                UpdatePath(x, y, grid[x, y].Count, true);
            } else
            {
                // just remove it normally
                grid[x, y].RemoveAt(grid[x, y].Count - 1);
            }
        }
    }

    public bool Placeable(int x, int y, int block)
    {
        int t = grid[x, y].Count - 1;
        bool p = false;
        switch (t)
        {
            case 0:
                p = BucketData[block].POFloor;
                break;
            case 1:
                p = BucketData[block].POCylinder;
                break;
            case 2:
                p = BucketData[block].POSquare;
                break;
            case 3:
                p = BucketData[block].POWall;
                break;
            case 4:
                p = BucketData[block].POGate;
                break;
            case 5:
                p = BucketData[block].POStraightWall;
                break;
            case 6:
                p = BucketData[block].POWallRoof;
                break;
            case 7:
                p = BucketData[block].POCylinderRoof;
                break;
            case 8:
                p = BucketData[block].POCylinder2Roof;
                break;
            case 9:
                p = BucketData[block].POCylinder3Roof;
                break;
            case 10:
                p = BucketData[block].POSquareRoof;
                break;
            case 11:
                p = BucketData[block].POStraightRoad;
                break;
            case 12:
                p = BucketData[block].POCurvedRoad;
                break;
            case 13:
                p = BucketData[block].POIntersectionRoad;
                break;
            case 14:
                p = BucketData[block].POTShapedRoad;
                break;
        }
        return p;
    }

    public bool CheckBlock(int x, int y)
    {
        if (!WithinBounds(x, y))
        {
            return false; // outside of bounds
        }
        if (grid[x,y].Count == 1)
        {
            return false;
        }
        else
        {
            return true;
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

    //Highlights the top block of the specified spot
    public void HighlightBlock(int x, int y)
    {
        if (!WithinBounds(x, y))
        {
            return; // outside of bounds
        }

        GameObject block = objectgrid[x, y][objectgrid[x, y].Count - 1];
        if (block == null)
        {
            return; // it was probably destroyed and the world is shutting down atm.
        }
        BlockHighlighter bh = block.GetComponent<BlockHighlighter>();
        if (bh != null)
        {
            // then use it!
            bh.HighlightMat(hmat);
        }
        else
        {
            if (block.transform.childCount == 0)
            {
                //umat = objectgrid[x, y][objectgrid[x, y].Count - 1].GetComponent<MeshRenderer>().material;
                objectgrid[x, y][objectgrid[x, y].Count - 1].GetComponent<MeshRenderer>().material = hmat;
                return;
            }
            //bool first = true;
            //foreach (Transform child in block.transform)
            //{
            //    if (first)
            //    {
            //        first = false;
            //        umat = child.GetComponent<MeshRenderer>().material;
            //    }
            //    child.GetComponent<MeshRenderer>().material = hmat;
            //}

            foreach (MeshRenderer m in objectgrid[x, y][objectgrid[x, y].Count - 1].GetComponentsInChildren<MeshRenderer>())
            {
                //if (first)
                //{
                //    first = false;
                //    umat = m.material;
                //}
                m.material = hmat;
            }
        }
    }

    //UnHighlights the top block of the specified spot
    public void UnHighlightBlock(int x, int y)
    {
        if (!WithinBounds(x, y))
        {
            return; // outside of bounds
        }

        GameObject block = objectgrid[x, y][objectgrid[x, y].Count - 1];
        if (block == null)
        {
            return; // it was probably destroyed, so return early
        }
        BlockHighlighter bh = block.GetComponent<BlockHighlighter>();
        if (bh != null)
        {
            // then use it!
            bh.UnHighlight();
        }
        else
        {
            if (block.transform.childCount == 0)
            {
                if (objectgrid[x, y][objectgrid[x, y].Count - 1] == null)
                {
                    //Debug.Log("Object is null, was it destroyed?");
                    return; // it was destroyed...
                }
                else
                {
                    objectgrid[x, y][objectgrid[x, y].Count - 1].GetComponent<MeshRenderer>().material = umat;
                }
                return;
            }
            //foreach (Transform child in objectgrid[x, y][objectgrid[x, y].Count - 1].transform)
            //{
            //    child.GetComponent<MeshRenderer>().material = umat;
            //}

            foreach (MeshRenderer m in objectgrid[x, y][objectgrid[x, y].Count - 1].GetComponentsInChildren<MeshRenderer>())
            {
                m.material = umat;
            }
        }
    }

    //UnHighlights the top block of the specified spot
    public void UnHighlightBlock(Vector3Int pos)
    {
        if (!WithinBounds(pos))
        {
            return; // outside of bounds
        }
        //objectgrid[pos.x, pos.y][pos.z].GetComponent<MeshRenderer>().material = umat;

        foreach (MeshRenderer m in objectgrid[pos.x, pos.y][pos.z].GetComponentsInChildren<MeshRenderer>())
        {
            m.material = umat;
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


    //Saves the current map
    //Will save in playerprefs with a name specified by the text in "textbox"
    public void Save()
    {
        sg = new SaveGrid();

        SaveGridList[] s = GridToSaveGrid(grid);
        sg.grid = s;
        sg.width = width;
        sg.height = height;
        string json = JsonUtility.ToJson(sg);

        //Debug.Log("json: " + json);
        string savename = textbox.text;
        //Debug.Log("Save name: " + savename);

        PlayerPrefs.SetString(savename, json);
    }

    //Loads a map
    //Will load from playerprefs with a name specified by the text in "textbox"
    public void LoadGrid()
    {
        double starttime = Time.fixedTime;
        string loadname = textbox.text;
        if(!PlayerPrefs.HasKey(loadname))
        {
            return;
        }
        //Debug.Log("Load name: " + loadname);
        string json = PlayerPrefs.GetString(loadname);
        //Debug.Log("got string");
        //Debug.Log("json: " + json);

        SaveGrid s = JsonUtility.FromJson<SaveGrid>(json);
        //Debug.Log("saved grid");
        //Debug.Log("savegridlist length: " + s.grid.Length);

        ClearGrid();
        //Debug.Log("cleared");

        grid = SaveGridToGrid(s.grid);
        //Debug.Log("changed grid");
        RefreshMap();
        //Debug.Log("refreshed");
        double endtime = Time.fixedTime;
        Debug.Log("Load time: " + (endtime - starttime));
    }

    //Converts a List<int>[,] to a SavegridList[]
    public SaveGridList[] GridToSaveGrid(List<int>[,] grid)
    {
        SaveGridList[] s = new SaveGridList[width * height];
        int d = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                SaveGridList t = new SaveGridList(grid[i, j]);
                s[d] = t;
                d++;
            }
        }

        //Debug.Log(s);
        return s;
    }

    //Converts a SaveGridList[] to a List<int>[,]
    public List<int>[,] SaveGridToGrid(SaveGridList[] s)
    {
        List<int>[,] ret = new List<int>[width,height];

        int i = 0;
        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                //Debug.Log("i: " + i + "\n" + "x: " + x + "\n" + "y: " + y);
                //Debug.Log("s.size: " + s.Length);
                ret[x, y] = new List<int>();
                ret[x, y] = s[i].objects;
                i++;
            }
        }

        //Debug.Log(ret);
        return ret;
    }

    //Clears the map
    public void ClearGrid()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                for (int k = 1; k < grid[i, j].Count; k++)
                {
                    Destroy(objectgrid[i, j][k]);
                }
            }
        }

        grid = new List<int>[width, height];
    }
}
