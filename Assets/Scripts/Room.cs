using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class Room : MonoBehaviour
{
    public float doorChance = 0.2f;
    public List<Vector2Int> cells = new List<Vector2Int>();
    private GenerateMaze maze;
    private Vector2Int origin;

    public void DrawRoom(Vector2Int roomSizeMinMax)
    {
        maze = FindObjectOfType<GenerateMaze>();

        Vector2Int corner = new Vector2Int(0,0);

        Vector2Int randRoomSize = Vector2Int.zero;


        //First scan an area to see if its large enough for a room, if not find a new area
        int area = 0;

        bool searching = true;

        while (searching)
        {
            corner = maze.FindStartPoint();

            randRoomSize = new Vector2Int(UnityEngine.Random.Range(roomSizeMinMax.x, roomSizeMinMax.y),
                UnityEngine.Random.Range(roomSizeMinMax.x, roomSizeMinMax.y));

            for (int i = 0; i < randRoomSize.x; i++) 
            {
                for (int j = 0; j < randRoomSize.y; j++) 
                {
                    if (corner.x + i < (int)(maze.TerrainSize.x / maze.tileSize) -2 && corner.y + j < (int)(maze.TerrainSize.z / maze.tileSize)-2)
                    {
                        if (maze.foundation[corner.x + i, corner.y + j]) 
                        {
                            area++;
                        }
                    }
                }
            }

            //if too small, the size is set to zero
            if (area > (randRoomSize.x * randRoomSize.y) / 2) {
                searching = false;
            }
            else
            {
                randRoomSize = Vector2Int.zero;
            }
        }


        for (int i = 0; i < randRoomSize.x; i++)
        {
            for (int j = 0; j < randRoomSize.y; j++)
            {
                if (corner.x + i < (int) (maze.TerrainSize.x / maze.tileSize) &&
                    corner.y + j < (int) (maze.TerrainSize.z / maze.tileSize))
                {
                    if (maze.foundation[corner.x + i, corner.y + j]) 
                    {
                        //set all walls to false

                        Vector2Int point = new Vector2Int(corner.x + i, corner.y + j);

                        maze.grid[point.x, point.y].DrawCell(false, false, false, false);
                        maze.grid[point.x, point.y].CellOfType = Cell.CellType.room;
                        maze.grid[point.x, point.y].gameObject.transform.Find("Floor").gameObject
                            .GetComponent<MeshRenderer>().sharedMaterial = maze.roomMat;

                        maze.grid[point.x, point.y].Nutrition = UnityEngine.Random.Range(5, 20);

                        Cell[] neighbors = maze.GetNeighbors(point);

                        for (int k = 0; k < 4; k++)
                        {
                            maze.grid[point.x, point.y].SetWall((Cell.Wall) k, neighbors[k] == null);

                        }


                        cells.Add(maze.grid[point.x, point.y].Location);
                    }
                }
            }
        }

        this.gameObject.transform.SetPositionAndRotation(new Vector3(corner.x,0,corner.y) * maze.tileSize, Quaternion.identity);
    }


    public void AddDoors()
    {
        Cell[] neighbors = new Cell[4];
        for (int i = 0; i < cells.Count; i++)
        {
            neighbors = maze.GetNeighbors(cells[i]);

            for (int j = 0; j < 4; j++)
            {
                if (neighbors[j] != null)
                {
                    if (neighbors[j].CellOfType == Cell.CellType.hallway) 
                    {
                        

                        if (UnityEngine.Random.Range(0,1f) < doorChance)
                        {
                            Debug.Log("door");
                            maze.grid[cells[i].x, cells[i].y].SetWall((Cell.Wall) j, false);
                            neighbors[j].SetDirection( (neighbors[j].Location - cells[i]));
                        }
                        else
                        {
                            Debug.Log("notdoor");
                        }




                    }
                }
            }
        }
    }



}
