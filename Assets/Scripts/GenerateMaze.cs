using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using System;
using UnityEngine.Experimental.Rendering;

[ExecuteInEditMode]
public class GenerateMaze : MonoBehaviour
{
    public Terrain floor;
    public GameObject parent;
    public GameObject roomParent;
    public GameObject cellPrefab;
    public GameObject roomPrefab;

    public Material hallwayMat;
    public Material emptyMat;
    public Material roomMat;

    public Vector2Int roomSizeMinMax;

    [Range(1,50)]
    public int tileSize;

    private TerrainData terrainData;
    public Vector3 TerrainSize { get; private set; }

    private List<Room> rooms= new List<Room>();

    public bool[,] foundation;
    public Cell[,] grid;

    private System.Random random;
    private Stack<Vector2Int> stack = new Stack<Vector2Int>();


    public Cell[] GetNeighbors(Vector2Int current)
    {
        Cell[] neighbors = new Cell[4];

        //finds neighbors and adds valid ones to neighbors list
        for (int i = 0; i < 4; i++) 
        {
            Vector2Int test = new Vector2Int(0, 0);

            switch (i) {

                case (int)Cell.Wall.north:

                    test = Utils.Vector2IntClamp(current + Vector2Int.up, 0, (int)(TerrainSize.x / tileSize) - 1);
                    break;
                case (int)Cell.Wall.south:

                    test = Utils.Vector2IntClamp(current + Vector2Int.down, 0, (int)(TerrainSize.x / tileSize) - 1);
                    break;
                case (int)Cell.Wall.east:

                    test = Utils.Vector2IntClamp(current + Vector2Int.right, 0, (int)(TerrainSize.z / tileSize) - 1);
                    break;
                case (int)Cell.Wall.west:

                    test = Utils.Vector2IntClamp(current + Vector2Int.left, 0, (int)(TerrainSize.z / tileSize) - 1);
                    break;
            }

            if (foundation[test.x, test.y] && (test - current) != Vector2Int.zero) 
            {
                neighbors[i] = grid[test.x, test.y];

            }
            else
            {
                neighbors[i] = null;
            }
            
        }

        return neighbors;
    }

    void ScanTerrain()
    {

        Init();


        //raycast down onto the terrain to see if lifted at all
        for (int i = 0; i < (int)(TerrainSize.x / tileSize); i ++)
        {
            for (int j = 0; j < (int)(TerrainSize.z / tileSize); j ++)
            {
                

                RaycastHit hit;

                if (Physics.Raycast(new Vector3(i*tileSize, TerrainSize.y, j*tileSize) ,Vector3.down, out hit, TerrainSize.y*2) )
                {

                    foundation[i, j] = (hit.point.y == floor.transform.position.y);

                }
            }
        }
    }


    public void DrawBlankGrid()
    {

        ScanTerrain();


        for (int i = 0; i < (int)(TerrainSize.x / tileSize); i ++)
        {
            for (int j = 0; j < (int)(TerrainSize.z / tileSize); j ++)
            {

                if (foundation[i, j])
                {
                    GameObject newCell = Instantiate(cellPrefab, new Vector3(i * tileSize, 0, j * tileSize), Quaternion.identity);
                    newCell.transform.localScale = Vector3.one * tileSize;
                    grid[i, j] = newCell.GetComponent<Cell>();

                    newCell.transform.SetParent(parent.transform);
                    grid[i, j].CellOfType = Cell.CellType.empty;
                    grid[i,j].Location = new Vector2Int(i,j);
                }
                else
                {
                    grid[i, j] = null;
                }

            }
        }
    }

    public Vector2Int FindStartPoint() 
    {

        Vector2Int start = new Vector2Int(0, 0);
        bool checking = true;

        while (checking) 
        {
            start.x = (int) random.Next(1, (int) (TerrainSize.x / tileSize) -2);
            start.y = (int) random.Next(1, (int) (TerrainSize.z / tileSize) -2);

            if (foundation[start.x, start.y])
            {
                if (grid[start.x, start.y].CellOfType == Cell.CellType.empty) checking = false;
            }
        }

        return start;
    }


    public void AddRooms(int count)
    {
        ClearRooms();

        //for how many rooms
        for (int l = 0; l < count; l++)
        {
            GameObject newRoom = Instantiate(roomPrefab);

            newRoom.transform.SetParent(roomParent.transform);

            Room roomComponent = newRoom.GetComponent<Room>();

            roomComponent.DrawRoom(roomSizeMinMax);
            

            rooms.Add(roomComponent);
        }
    }

   

    void AddFeatures()
    {
        for (int i = 0; i < (int)(TerrainSize.x / tileSize); i++) 
        {
            for (int j = 0; j < (int)(TerrainSize.z / tileSize); j++) 
            {
                if (foundation[i, j]) 
                {

                }
            }
        }
    }

    public void DrawMaze()
    {

        ClearMaze();
        DrawBlankGrid();
        AddRooms(5);
        stack.Clear();


        bool searching = true;

        //random start
        Vector2Int current = FindStartPoint();

        grid[current.x, current.y].CellOfType = Cell.CellType.hallway;

        //searches through maze
        List<Cell> neighbors = new List<Cell>();
        

        while (searching)
        {
            neighbors.Clear();

            //finds neighbors and adds valid ones to neighbors list
            for (int i = 0; i < 4; i++)
            {
                Vector2Int test = new Vector2Int(0, 0);
                switch (i)
                {
                    case (int)Cell.Wall.north:
                        test = Utils.Vector2IntClamp(current + Vector2Int.up, 0, (int) (TerrainSize.x/tileSize)-1);
                        break;
                    case (int)Cell.Wall.south:
                        test = Utils.Vector2IntClamp(current + Vector2Int.down, 0, (int) (TerrainSize.x/tileSize)-1);
                        break;
                    case (int)Cell.Wall.east:
                        test = Utils.Vector2IntClamp(current + Vector2Int.right, 0, (int) (TerrainSize.z/tileSize)-1);
                        break;
                    case (int)Cell.Wall.west:
                        test = Utils.Vector2IntClamp(current + Vector2Int.left, 0, (int) (TerrainSize.z/tileSize)-1);
                        break;
                }
                
                if (foundation[test.x, test.y] && (test - current) != Vector2Int.zero)
                {
                    if (grid[test.x, test.y].CellOfType == Cell.CellType.empty)
                    {
                        neighbors.Add(grid[test.x, test.y]);
                    }
                }
            }
            
            if (neighbors.Count > 0)
            {
                //randomly decide which neighbor to go to
                int nextIndex = UnityEngine.Random.Range(0, neighbors.Count);
                Cell next = neighbors[nextIndex];

                Vector2Int dir = next.Location - current;

                next.SetDirection(dir);
                grid[current.x, current.y].SetDirection(dir*-1);

                next.CellOfType = Cell.CellType.hallway;
                next.transform.Find("Floor").GetComponent<MeshRenderer>().sharedMaterial = hallwayMat;
                next.Nutrition = UnityEngine.Random.Range(0, 2);

                stack.Push(dir*-1);
                current = next.Location;
            }
            else if(neighbors.Count == 0 && stack.Count > 1)
            {
                current += stack.Pop();
            }
            else
            {
                searching = false; //end the seach loop
            }
        }

        for (int i = 0; i < rooms.Count; i++)
        {
            rooms[i].AddDoors();
        }
    }

    public void ClearMaze()
    {
        Cell[] trash = parent.GetComponentsInChildren<Cell>();

        for (int i = 0; i < trash.Length; i++)
        {
            if (trash[i].gameObject != parent.gameObject)
            {
                DestroyImmediate(trash[i].gameObject);
            }
        }
    }

    public void ClearRooms() {
        rooms.Clear();
        Room[] trash = roomParent.GetComponentsInChildren<Room>();

        for (int i = 0; i < trash.Length; i++) 
        {
            if (trash[i].gameObject != roomParent.gameObject) 
            {
                DestroyImmediate(trash[i].gameObject);
            }
        }
    }


    void Init()
    {
        random = new System.Random();
        
        terrainData = floor.terrainData;
        TerrainSize = terrainData.size;

        foundation = new bool[(int) (TerrainSize.x / tileSize), (int) (TerrainSize.z / tileSize)];
        grid = new Cell[(int) (TerrainSize.x / tileSize), (int) (TerrainSize.z / tileSize)];
    }
}
