using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSpace
{
    public bool available = true;
    public GameObject northWall, southWall, eastWall, westWall, floor;

    public SingleSpace()
    {
        available = true;
        northWall = southWall = eastWall = westWall = floor = null;
    }

    public SingleSpace(SingleSpace copy)
    {
        available = copy.available;
        northWall = copy.northWall;
        southWall = copy.southWall;
        eastWall = copy.eastWall;
        westWall = copy.westWall;
    }
}
