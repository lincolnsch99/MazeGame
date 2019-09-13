using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject floorPrefab;

    public int numRows;
    public int numColumns;
    public SingleSpace[,] grid;

    public enum Difficulty
    {
        EASY,
        MEDIUM,
        HARD
    }

    public Difficulty difficulty;

    private void Start()
    {

        switch(difficulty)
        {
            case Difficulty.EASY:
                numColumns = 10;
                numRows = 10;
                break;
            case Difficulty.MEDIUM:
                numColumns = 20;
                numRows = 20;
                break;
            case Difficulty.HARD:
                numColumns = 35;
                numRows = 35;
                break;
            default:
                numColumns = 0;
                numRows = 0;
                break;
        }
        grid = new SingleSpace[numColumns, numRows];
        CreateBaseGrid();
        GenerateMaze();
    }

    /**
     * Generates a grid of cells, which have walls on all four sides and a floor. This creates a base
     * platform for the random path generator to do its thing.
     */
    private void CreateBaseGrid()
    {
        InitializeGrid();
        for (int i = 0; i < numColumns; i++)
        {
            for (int j = 0; j < numRows; j++)
            {
                // All cells generate a floor, south wall, and east wall
                grid[i, j].floor = GameObject.Instantiate(floorPrefab, transform);
                grid[i, j].floor.transform.position = new Vector3(i * 6, 0, j * 6);
                grid[i, j].floor.name = "Floor: " + i + " , " + j;

                grid[i, j].southWall = GameObject.Instantiate(wallPrefab, transform);
                grid[i, j].southWall.transform.position = new Vector3(i * 6, 3, (j * 6) - 3);
                grid[i, j].southWall.transform.localEulerAngles = new Vector3(0, 90, 0);
                grid[i, j].southWall.name = "SouthWall: " + i + " , " + j;

                grid[i, j].eastWall = GameObject.Instantiate(wallPrefab, transform);
                grid[i, j].eastWall.transform.position = new Vector3((i * 6) + 3, 3, j * 6);
                grid[i, j].eastWall.name = "EastWall: " + i + " , " + j;

                // The cells on the west and north edges generate a west wall and north wall
                if (i == 0)
                {
                    grid[i, j].westWall = GameObject.Instantiate(wallPrefab, transform);
                    grid[i, j].westWall.transform.position = new Vector3((i * 6) - 3, 3, j * 6);
                    grid[i, j].westWall.name = "WestWall: " + i + " , " + j;
                }

                if (j == numRows - 1)
                {
                    grid[i, j].northWall = GameObject.Instantiate(wallPrefab, transform);
                    grid[i, j].northWall.transform.position = new Vector3(i * 6, 3, (j * 6) + 3);
                    grid[i, j].northWall.transform.localEulerAngles = new Vector3(0, 90, 0);
                    grid[i, j].northWall.name = "NorthWall: " + i + " , " + j;
                }
            }
        }
    }

    /**
     * Initializes all the cells to an empty cell.
     */
    private void InitializeGrid()
    {
        for (int i = 0; i < numColumns; i++)
        {
            for (int j = 0; j < numRows; j++)
            {
                grid[i, j] = new SingleSpace();
            }
        }
    }

    private void GeneratePath(int startX, int startY)
    {
        grid[startX, startY].available = false;

        // 1 = North, 2 = West, 3 = South, 4 = East
        int random;
        bool validPath = true;
        List<int> untestedCell = new List<int>(4);

        while (validPath)
        {
            untestedCell.Clear();
            for (int i = 0; i < 4; i++)
                untestedCell.Insert(i, i + 1);
            bool validCell = false;
            int range = 4;
            while (!validCell)
            {
                if (untestedCell.Count > 0)
                {
                    random = Random.Range(0, range);
                    int direction = untestedCell[random];
                    untestedCell.RemoveAt(random);
                    range--;
                    switch (direction)
                    {
                        case 1:
                            if (startY < numRows - 1)
                            {
                                if (grid[startX, startY + 1].available)
                                {
                                    validCell = true;
                                    grid[startX, startY + 1].available = false;
                                    GoNorth(startX, startY);
                                    startY++;
                                }
                            }
                            break;
                        case 2:
                            if (startX > 0)
                            {
                                if (grid[startX - 1, startY].available)
                                {
                                    validCell = true;
                                    grid[startX - 1, startY].available = false;
                                    GoWest(startX, startY);
                                    startX--;
                                }
                            }
                            break;
                        case 3:
                            if (startY > 0)
                            {
                                if (grid[startX, startY - 1].available)
                                {
                                    validCell = true;
                                    grid[startX, startY - 1].available = false;
                                    GoSouth(startX, startY);
                                    startY--;
                                }
                            }
                            break;
                        case 4:
                            if (startX < numColumns - 1)
                            {
                                if (grid[startX + 1, startY].available)
                                {
                                    validCell = true;
                                    grid[startX + 1, startY].available = false;
                                    GoEast(startX, startY);
                                    startX++;
                                }
                            }
                            break;
                        default:
                            Debug.Log("Random out of range");
                            break;
                    }
                }
                else
                {
                    validPath = false;
                    break;
                }
            }
        }
    }

    private void GoNorth(int startX, int startY)
    {
        if (grid[startX, startY + 1].southWall != null)
        {
            Destroy(grid[startX, startY + 1].southWall);
            grid[startX, startY + 1].southWall = null;
        }
        else if (grid[startX, startY].northWall != null)
        {
            Destroy(grid[startX, startY].northWall);
            grid[startX, startY].northWall = null;
        }
        else
            Debug.LogError("Ya done fucked up");
    }

    private void GoWest(int startX, int startY)
    {
        if (grid[startX - 1, startY].eastWall != null)
        {
            Destroy(grid[startX - 1, startY].eastWall);
            grid[startX - 1, startY].eastWall = null;
        }
        else if (grid[startX, startY].westWall != null)
        {
            Destroy(grid[startX, startY].westWall);
            grid[startX, startY].westWall = null;
        }
        else
            Debug.LogError("Ya done fucked up");
    }

    private void GoSouth(int startX, int startY)
    {
        if (grid[startX, startY - 1].northWall != null)
        {
            Destroy(grid[startX, startY - 1].northWall);
            grid[startX, startY - 1].northWall = null;
        }
        else if (grid[startX, startY].southWall != null)
        {
            Destroy(grid[startX, startY].southWall);
            grid[startX, startY].southWall = null;
        }
        else
            Debug.LogError("Ya done fucked up");
    }

    private void GoEast(int startX, int startY)
    {
        if (grid[startX + 1, startY].westWall != null)
        {
            Destroy(grid[startX + 1, startY].westWall);
            grid[startX + 1, startY].westWall = null;
        }
        else if (grid[startX, startY].eastWall != null)
        {
            Destroy(grid[startX, startY].eastWall);
            grid[startX, startY].eastWall = null;
        }
        else
            Debug.LogError("Ya done fucked up");
    }

    private bool ValidStartPoint(int x, int y)
    {
        bool valid = false;
        if (grid[x, y].available)
        {
            if (x > 1)
            {
                if (!grid[x - 1, y].available)
                    valid = true;
            }
            if (x < numColumns - 1)
            {
                if (!grid[x + 1, y].available)
                    valid = true;
            }
            if (y > 1)
            {
                if (!grid[x, y - 1].available)
                    valid = true;
            }
            if (y < numRows - 1)
            {
                if (!grid[x, y + 1].available)
                    valid = true;
            }
        }
        return valid;
    }

    private Vector2 FindNextStartPoint()
    {
        Vector2 nextPoint = new Vector2(-1, -1);

        for (int i = 0; i < numColumns; i++)
        {
            for (int j = 0; j < numRows; j++)
            {
                if (ValidStartPoint(i, j))
                {
                    if (i > 1)
                    {
                        if (!grid[i - 1, j].available)
                        {
                            nextPoint = new Vector2(i, j);
                            GoWest(i, j);
                        }
                    }
                    if (i < numColumns - 1)
                    {
                        if (!grid[i + 1, j].available)
                        {
                            nextPoint = new Vector2(i, j);
                            GoEast(i, j);
                        }
                    }
                    if (j > 1)
                    {
                        if (!grid[i, j - 1].available)
                        {
                            nextPoint = new Vector2(i, j);
                            GoSouth(i, j);
                        }
                    }
                    if (j < numRows - 1)
                    {
                        if (!grid[i, j + 1].available)
                        {
                            nextPoint = new Vector2(i, j);
                            GoNorth(i, j);
                        }
                    }
                }
                if (nextPoint != new Vector2(-1, -1))
                    return nextPoint;
            }
        }

        return nextPoint;
    }

    private void GenerateMaze()
    {
        GeneratePath(0, 0);

        Vector2 nextStartPoint = FindNextStartPoint();

        while (nextStartPoint != new Vector2(-1, -1))
        {
            GeneratePath((int)nextStartPoint.x, (int)nextStartPoint.y);
            nextStartPoint = FindNextStartPoint();
        }
    }
}