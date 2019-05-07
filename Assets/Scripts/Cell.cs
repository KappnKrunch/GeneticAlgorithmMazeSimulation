using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Cell : MonoBehaviour
{
    //north is positive x direction
    public bool North { get; private set; }
    public bool South { get; private set; }
    public bool East { get; private set; }
    public bool West { get; private set; }

    public float Nutrition { get; set; }

    public Vector2Int Location { get; set; }

    public bool[] validThroughPut = new bool[4];

    public CellType CellOfType { get; set; }

    public enum Wall { north, south, east, west }
    public enum CellType { empty, hallway, room, feature }


    public GameObject n, s, e, w;

    public void DrawCell(bool north, bool south, bool east, bool west)
    {
        this.North = north;
        this.South = south;
        this.East = east;
        this.West = west;

        n.SetActive(north);
        s.SetActive(south);
        e.SetActive(east);
        w.SetActive(west);

        CellOfType = CellType.empty;

        for (int i = 0; i < 4; i++)
        {
            validThroughPut[i] = false;
        }
    }

    public bool GetWall(Wall whichWall)
    {
        bool test = false;

        switch (whichWall)
        {
            case Wall.north:
                test = North;
                break;
            case Wall.south:
                test = South;
                break;
            case Wall.east:
                test = East;
                break;
            case Wall.west:
                test = West;
                break;
        }

        return test;
    }

    public void SetWall(Wall whichWall, bool active) {

        switch (whichWall) 
        {
            case Wall.north:
                n.SetActive(active);
                validThroughPut[(int)Wall.north] = active;
                North = active;
                break;
            case Wall.south:
                s.SetActive(active);
                validThroughPut[(int)Wall.south] = active;
                South = active;
                break;
            case Wall.east:
                e.SetActive(active);
                validThroughPut[(int)Wall.east] = active;
                East = active;
                break;
            case Wall.west:
                w.SetActive(active);
                validThroughPut[(int)Wall.west] = active;
                West = active;
                break;
        }
    }

    public void SetDirection(Vector2Int dir)
    {
        switch (dir.x)
        {
            case 1:
                SetWall(Cell.Wall.west,false);
                validThroughPut[(int) Wall.east] = true;
                break;
            case -1:
                SetWall(Cell.Wall.east,false);
                validThroughPut[(int)Wall.west] = true;
                break;
            default:
                break;
        }
        switch (dir.y) 
        {
            case 1:
                SetWall(Cell.Wall.south,false);
                validThroughPut[(int)Wall.north] = true;
                break;
            case -1:
                SetWall(Cell.Wall.north,false);
                validThroughPut[(int)Wall.south] = true;
                break;
            default:
                break;
        }
    }

    
}
