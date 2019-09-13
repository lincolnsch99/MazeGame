using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    [SerializeField]
    private float chaseFrequency;

    public enum Direction
    {
        NORTH,
        SOUTH,
        EAST,
        WEST,
        UNDECIDED
    }

    private float positionVariability;
    private Stack<Vector2Int> movePath;
    private Vector2 nextPos;
    private MazeGenerator mazeInfo;
    private CharacterController enemyControl;
    private GameObject player;
    private float timeSinceChaseStart;
    private float moveCheck;
    private Vector2 lastPosition;
    private Coroutine currentCoroutine;
    private float speed;
    private float chaseSpeed;
    private float findSpeed;

    private float soundOffset;
    private float timeSinceSoundPlayed;

    private void Start()
    {
        mazeInfo = GameObject.FindWithTag("Maze").GetComponent<MazeGenerator>();
        player = GameObject.FindWithTag("Player");
        transform.position = new Vector3((mazeInfo.numColumns - 1) * 6, 4, (mazeInfo.numRows - 1) * 6);
        enemyControl = GetComponent<CharacterController>();
        positionVariability = 0.2f;
        timeSinceChaseStart = chaseFrequency;
        moveCheck = 0;
        timeSinceSoundPlayed = 0;
        soundOffset = 1;
        movePath = new Stack<Vector2Int>();
        lastPosition = new Vector2();
        nextPos = new Vector2();
        switch(mazeInfo.difficulty)
        {
            case MazeGenerator.Difficulty.EASY:
                findSpeed = 5;
                chaseSpeed = 8;
                break;
            case MazeGenerator.Difficulty.MEDIUM:
                findSpeed = 15;
                chaseSpeed = 12;
                break;
            case MazeGenerator.Difficulty.HARD:
                findSpeed = 20;
                chaseSpeed = 15;
                break;
        }
        speed = findSpeed;
    }

    private void Update()
    {
        timeSinceChaseStart += Time.deltaTime;
        moveCheck += Time.deltaTime;
        timeSinceSoundPlayed += Time.deltaTime;

        if (timeSinceChaseStart > chaseFrequency)
        {
            NewChase();
        }

        if (moveCheck > 0.5f)
        {
            Vector2 curPos = new Vector2(transform.position.x, transform.position.z);
            if (curPos == lastPosition)
            {
                Debug.Log("Correcting position");
                Vector2Int curCell = WorldToMazePoint(curPos);
                Vector2 newPos = MazeToWorldPoint(curCell);
                Debug.Log(newPos);
                transform.position = new Vector3(newPos.x, transform.position.y, newPos.y);
                NewChase();
            }
            lastPosition = new Vector2(transform.position.x, transform.position.z);
            moveCheck = 0;
        }

        if (movePath.Count > 0)
        {
            FollowPath(ref nextPos);
        }

        if (Vector3.Distance(transform.position, player.transform.position) < 3f)
        {
            Destroy(player);
            Destroy(this.gameObject);
        }

        if(timeSinceSoundPlayed > soundOffset)
        {
            GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
            timeSinceSoundPlayed = 0;
        }
    }

    private void NewChase()
    {
        Debug.Log("New Chase");
        movePath.Clear();
        movePath = GetPathToPlayer();
        nextPos = MazeToWorldPoint(movePath.Peek());
        timeSinceChaseStart = 0;
    }

    private Vector2Int WorldToMazePoint(Vector2 worldPos)
    {
        return new Vector2Int((int)(worldPos.x / 6f), (int)(worldPos.y / 6f));
    }

    private Vector2 MazeToWorldPoint(Vector2Int mazeCell)
    {
        return new Vector2((mazeCell.x * 6f), (mazeCell.y * 6f));
    }

    private bool moveTo(Vector2 pos)
    {
        if (Mathf.Abs(transform.position.x - pos.x) < positionVariability && Mathf.Abs(transform.position.z - pos.y) < positionVariability)
        {
            return true;
        }
        else
        {
            float moveX = 0, moveZ = 0;
            if (Mathf.Abs(transform.position.x - pos.x) < positionVariability)
            {
                moveX = 0;
            }
            else if (transform.position.x < pos.x)
            {
                moveX = speed;
            }
            else
            {
                moveX = -speed;
            }

            if (Mathf.Abs(transform.position.z - pos.y) < positionVariability)
            {
                moveZ = 0;
            }
            else if (transform.position.z < pos.y)
            {
                moveZ = speed;
            }
            else
            {
                moveZ = -speed;
            }

            Vector3 movement = new Vector3(moveX, 0, moveZ);
            movement = Vector3.ClampMagnitude(movement, speed);
            movement.y = Physics.gravity.y;

            movement *= Time.deltaTime;
            movement = transform.TransformDirection(movement);
            enemyControl.Move(movement);
            return false;
        }
    }

    private int GetPriority()
    {
        Vector2Int curCell = WorldToMazePoint(new Vector2(transform.position.x, transform.position.z));
        Vector2Int playerCell = WorldToMazePoint(new Vector2(player.transform.position.x, player.transform.position.z));
        int priority = 0;
        if (curCell.x < playerCell.x)
        {
            if (curCell.y < playerCell.y)
                priority = 1;
            else
                priority = 4;
        }
        else
        {
            if (curCell.y < playerCell.y)
                priority = 2;
            else
                priority = 3;
        }
        Debug.Log(priority);
        return priority;
    }

    /**
     * Grid cells are pushed into the stack to create a path, popped out if it reaches a dead end.
     * Ignore the fact that this one function is almost 200 lines long...not important...
     */
    private Stack<Vector2Int> GetPathToPlayer()
    {
        Stack<Vector2Int> path = new Stack<Vector2Int>();

        SingleSpace[,] grid = CreateDeepCopyGrid(mazeInfo.grid);

        Vector2Int curCell = WorldToMazePoint(new Vector2(transform.position.x, transform.position.z));
        Vector2Int playerCell = WorldToMazePoint(new Vector2(player.transform.position.x, player.transform.position.z));
        Vector2Int moveTo = curCell;
        grid[moveTo.x, moveTo.y].available = true;

        int priority = GetPriority();

        while (moveTo != playerCell)
        {
            int random;
            List<int> untestedCell = new List<int>(4);
            if (!CheckPrioritizedPaths(priority, ref moveTo, ref grid, ref path))
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
                                if (CheckNorth(moveTo, grid))
                                {
                                    validCell = true;
                                    grid[moveTo.x, moveTo.y + 1].available = true;
                                    moveTo.y++;
                                    path.Push(moveTo);
                                }
                                break;
                            case 2:
                                if (CheckWest(moveTo, grid))
                                {
                                    validCell = true;
                                    grid[moveTo.x - 1, moveTo.y].available = true;
                                    moveTo.x--;
                                    path.Push(moveTo);
                                }
                                break;
                            case 3:
                                if (CheckSouth(moveTo, grid))
                                {
                                    validCell = true;
                                    grid[moveTo.x, moveTo.y - 1].available = true;
                                    moveTo.y--;
                                    path.Push(moveTo);
                                }
                                break;
                            case 4:
                                if (CheckEast(moveTo, grid))
                                {
                                    validCell = true;
                                    grid[moveTo.x + 1, moveTo.y].available = true;
                                    moveTo.x++;
                                    path.Push(moveTo);
                                }
                                break;
                            default:
                                Debug.Log("Random out of range");
                                break;
                        }
                    }
                    else
                    {
                        path = Backtrack(grid, path);
                        moveTo = path.Peek();
                        validCell = true;
                    }
                }
            }
        }

        ResetGrid(); 

        // Reversing the order of the items in the stack
        Stack<Vector2Int> returnPath = new Stack<Vector2Int>();
        while (path.Count != 0)
        {
            returnPath.Push(path.Pop());
        }
        return returnPath;
    }

    private void ResetGrid()
    {
        for(int i = 0; i < mazeInfo.numColumns; i++)
        {
            for(int j = 0; j < mazeInfo.numRows; j++)
            {
                mazeInfo.grid[i, j].available = false;
            }
        }
    }

    private bool CheckPrioritizedPaths(int priority, ref Vector2Int check, ref SingleSpace[,] grid, ref Stack<Vector2Int> path)
    {
        bool completed = false;
        switch (priority)
        {
            case 1:
                if (Mathf.Max((Mathf.Abs(transform.position.x - player.transform.position.x)), (Mathf.Abs(transform.position.z - player.transform.position.z)))
                    == (Mathf.Abs(transform.position.z - player.transform.position.z)))
                {
                    if (CheckNorth(check, grid) && !completed)
                    {
                        grid[check.x, check.y + 1].available = true;
                        check.y++;
                        completed = true;
                        path.Push(check);
                    }
                    if (CheckEast(check, grid) && !completed)
                    {
                        grid[check.x + 1, check.y].available = true;
                        check.x++;
                        completed = true;
                        path.Push(check);
                    }
                }
                else
                {
                    if (CheckEast(check, grid) && !completed)
                    {
                        grid[check.x + 1, check.y].available = true;
                        check.x++;
                        completed = true;
                        path.Push(check);
                    }
                    if (CheckNorth(check, grid) && !completed)
                    {
                        grid[check.x, check.y + 1].available = true;
                        check.y++;
                        completed = true;
                        path.Push(check);
                    }
                }
                break;
            case 2:
                if (Mathf.Max((Mathf.Abs(transform.position.x - player.transform.position.x)), (Mathf.Abs(transform.position.z - player.transform.position.z)))
                    == (Mathf.Abs(transform.position.z - player.transform.position.z)))
                {
                    if (CheckNorth(check, grid) && !completed)
                    {
                        grid[check.x, check.y + 1].available = true;
                        check.y++;
                        completed = true;
                        path.Push(check);
                    }
                    if (CheckWest(check, grid) && !completed)
                    {
                        grid[check.x - 1, check.y].available = true;
                        check.x--;
                        completed = true;
                        path.Push(check);
                    }
                }
                else
                {
                    if (CheckWest(check, grid) && !completed)
                    {
                        grid[check.x - 1, check.y].available = true;
                        check.x--;
                        completed = true;
                        path.Push(check);
                    }
                    if (CheckNorth(check, grid) && !completed)
                    {
                        grid[check.x, check.y + 1].available = true;
                        check.y++;
                        completed = true;
                        path.Push(check);
                    }
                }
                break;
            case 3:
                if (Mathf.Max((Mathf.Abs(transform.position.x - player.transform.position.x)), (Mathf.Abs(transform.position.z - player.transform.position.z)))
                    == (Mathf.Abs(transform.position.z - player.transform.position.z)))
                {
                    if (CheckSouth(check, grid) && !completed)
                    {
                        grid[check.x, check.y - 1].available = true;
                        check.y--;
                        completed = true;
                        path.Push(check);
                    }
                    if (CheckWest(check, grid) && !completed)
                    {
                        grid[check.x - 1, check.y].available = true;
                        check.x--;
                        completed = true;
                        path.Push(check);
                    }
                }
                else
                {
                    if (CheckWest(check, grid) && !completed)
                    {
                        grid[check.x - 1, check.y].available = true;
                        check.x--;
                        completed = true;
                        path.Push(check);
                    }
                    if (CheckSouth(check, grid) && !completed)
                    {
                        grid[check.x, check.y - 1].available = true;
                        check.y--;
                        completed = true;
                        path.Push(check);
                    }
                }
                break;
            case 4:
                if (Mathf.Max((Mathf.Abs(transform.position.x - player.transform.position.x)), (Mathf.Abs(transform.position.z - player.transform.position.z)))
                    == (Mathf.Abs(transform.position.z - player.transform.position.z)))
                {
                    if (CheckSouth(check, grid) && !completed)
                    {
                        grid[check.x, check.y - 1].available = true;
                        check.y--;
                        completed = true;
                        path.Push(check);
                    }
                    if (CheckEast(check, grid) && !completed)
                    {
                        grid[check.x + 1, check.y].available = true;
                        check.x++;
                        completed = true;
                        path.Push(check);
                    }
                }
                else
                {
                    if (CheckEast(check, grid) && !completed)
                    {
                        grid[check.x + 1, check.y].available = true;
                        check.x++;
                        completed = true;
                        path.Push(check);
                    }
                    if (CheckSouth(check, grid) && !completed)
                    {
                        grid[check.x, check.y - 1].available = true;
                        check.y--;
                        completed = true;
                        path.Push(check);
                    }
                }
                break;
        }
        return completed;
    }

    private bool CheckNorth(Vector2Int checkCell, SingleSpace[,] grid)
    {
        if (checkCell.y < mazeInfo.numRows - 1)
        {
            if (!grid[checkCell.x, checkCell.y + 1].available && !grid[checkCell.x, checkCell.y].northWall
                && !grid[checkCell.x, checkCell.y + 1].southWall)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckSouth(Vector2Int checkCell, SingleSpace[,] grid)
    {
        if (checkCell.y > 0)
        {
            if (!grid[checkCell.x, checkCell.y - 1].available && !grid[checkCell.x, checkCell.y].southWall
                && !grid[checkCell.x, checkCell.y - 1].northWall)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckEast(Vector2Int checkCell, SingleSpace[,] grid)
    {
        if (checkCell.x < mazeInfo.numColumns - 1)
        {
            if (!grid[checkCell.x + 1, checkCell.y].available && !grid[checkCell.x, checkCell.y].eastWall
                && !grid[checkCell.x + 1, checkCell.y].westWall)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckWest(Vector2Int checkCell, SingleSpace[,] grid)
    {
        if (checkCell.x > 0)
        {
            if (!grid[checkCell.x - 1, checkCell.y].available && !grid[checkCell.x, checkCell.y].westWall
                && !grid[checkCell.x - 1, checkCell.y].eastWall)
            {
                return true;
            }
        }
        return false;
    }

    private Stack<Vector2Int> Backtrack(SingleSpace[,] grid, Stack<Vector2Int> path)
    {
        bool backToIntersection = false;
        Vector2Int testCell = new Vector2Int();
        while (!backToIntersection)
        {
            if (path.Count < 1)
                return new Stack<Vector2Int>();
            testCell = path.Pop();
            if (testCell.y < mazeInfo.numColumns - 1)
            {
                if (!grid[testCell.x, testCell.y].northWall && !grid[testCell.x, testCell.y + 1].southWall
                    && grid[testCell.x, testCell.y + 1].available == false)
                {
                    backToIntersection = true;
                    path.Push(testCell);
                }
            }
            if (testCell.y > 0 && !backToIntersection)
            {
                if (!grid[testCell.x, testCell.y].southWall && !grid[testCell.x, testCell.y - 1].northWall
                    && grid[testCell.x, testCell.y - 1].available == false)
                {
                    backToIntersection = true;
                    path.Push(testCell);
                }
            }
            if (testCell.x > 0 && !backToIntersection)
            {
                if (!grid[testCell.x, testCell.y].westWall && !grid[testCell.x - 1, testCell.y].eastWall
                    && grid[testCell.x - 1, testCell.y].available == false)
                {
                    backToIntersection = true;
                    path.Push(testCell);
                }
            }
            if (testCell.x < mazeInfo.numColumns - 1 && !backToIntersection)
            {
                if (!grid[testCell.x, testCell.y].eastWall && !grid[testCell.x + 1, testCell.y].westWall
                    && grid[testCell.x + 1, testCell.y].available == false)
                {
                    backToIntersection = true;
                    path.Push(testCell);
                }
            }
        }
        return path;
    }

    private void FollowPath(ref Vector2 nextPos)
    {
        if (transform.position.x != player.transform.position.x
            && transform.position.z != player.transform.position.z)
        {
            if (moveTo(nextPos))
                nextPos = MazeToWorldPoint(movePath.Pop());
            return;
        }
    }

    private SingleSpace[,] CreateDeepCopyGrid(SingleSpace[,] copyFrom)
    {
        SingleSpace[,] deepCopy = new SingleSpace[copyFrom.GetLength(0), copyFrom.GetLength(1)];
        for (int i = 0; i < copyFrom.GetLength(0); i++)
        {
            for (int j = 0; j < copyFrom.GetLength(1); j++)
            {
                deepCopy[i, j] = new SingleSpace(copyFrom[i, j]);
            }
        }
        return deepCopy;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            speed = chaseSpeed;
            soundOffset /= 2;
            NewChase();
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (timeSinceChaseStart > 2f)
                NewChase();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            chaseSpeed = findSpeed;
            soundOffset *= 2;
        }
    }
}
