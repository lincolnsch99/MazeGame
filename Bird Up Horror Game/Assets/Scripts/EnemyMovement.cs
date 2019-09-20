using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private AudioSource birdUp;
    [SerializeField]
    private AudioSource buildUp;
    [SerializeField]
    private AudioSource baKaw;
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
    private PlayerMovement playerInfo;
    private float timeSinceChaseStart, timeSinceBirdUp, timeSinceBaKaw;
    private float moveCheck;
    private Coroutine currentCoroutine;
    private float speed;
    private float chaseSpeed;
    private float findSpeed;
    private float chaseFrequency;

    private float baKawOffset, baKawFrequency;
    private Animator animator;

    private DontDestroy PersistentData;

    private void Start()
    {
        PersistentData = GameObject.FindWithTag("Persistent").GetComponent<DontDestroy>();
        mazeInfo = GameObject.FindWithTag("Maze").GetComponent<MazeGenerator>();
        player = GameObject.FindWithTag("Player");
        playerInfo = player.GetComponent<PlayerMovement>();
        animator = transform.GetChild(0).GetComponent<Animator>();

        chaseFrequency = 10f;
        chaseSpeed = PersistentData.enemyChaseSpeed;
        findSpeed = PersistentData.enemySearchSpeed;

        speed = findSpeed;
        enemyControl = GetComponent<CharacterController>();
        positionVariability = 0.2f;
        timeSinceChaseStart = chaseFrequency;
        timeSinceBirdUp = 10f;
        moveCheck = 0;
        timeSinceBaKaw = 0;
        baKawFrequency = 5f;
        baKawOffset = Random.Range(0.5f, baKawFrequency);
        movePath = new Stack<Vector2Int>();
        nextPos = new Vector2();
        animator.SetInteger("animState", 1);
    }

    private void Update()
    {
        if (!PersistentData.PAUSED)
        {
            timeSinceChaseStart += Time.deltaTime;
            moveCheck += Time.deltaTime;
            timeSinceBirdUp += Time.deltaTime;
            timeSinceBaKaw += Time.deltaTime;

            if (movePath.Count > 0)
            {
                FollowPath(ref nextPos);
            }
            else
                NewChase("Random");

            if (Vector3.Distance(transform.position, player.transform.position) < 3f)
            {
                Destroy(this.gameObject);
                PersistentData.GameLost();
                Cursor.visible = true;
            }

            if (timeSinceBaKaw > baKawOffset)
            {
                baKaw.PlayOneShot(baKaw.clip);
                timeSinceBaKaw = 0;
                baKawOffset = Random.Range(baKawFrequency / 2f, baKawFrequency);
            }
        }
    }

    private void NewChase(string type)
    {
        ResetGrid();
        movePath.Clear();
        Debug.Log("New Chase");

        if (type.Equals("Random"))
        {
            speed = findSpeed;
            movePath = GetPathToPoint(new Vector2Int(Random.Range(0, mazeInfo.numColumns), Random.Range(0, mazeInfo.numRows)));
        }
        else if (type.Equals("Player"))
        {
            speed = chaseSpeed;
            movePath = GetPathToPoint(WorldToMazePoint(new Vector2(player.transform.position.x, player.transform.position.z)));
        }

        if (movePath.Count > 0)
            nextPos = MazeToWorldPoint(movePath.Peek());
        timeSinceChaseStart = 0;
    }

    private Vector2Int WorldToMazePoint(Vector2 worldPos)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPos.x / 6f), Mathf.RoundToInt(worldPos.y / 6f));
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
            float rotY = 0;
            if (Mathf.Abs(transform.position.x - pos.x) < positionVariability)
            {
                moveX = 0;
            }
            else if (transform.position.x < pos.x)
            {
                moveX = speed;
                rotY = 90;
            }
            else
            {
                moveX = -speed;
                rotY = -90;
            }

            if (Mathf.Abs(transform.position.z - pos.y) < positionVariability)
            {
                moveZ = 0;
            }
            else if (transform.position.z < pos.y)
            {
                moveZ = speed;
                rotY = 0;
            }
            else
            {
                moveZ = -speed;
                rotY = 180;
            }

            Vector3 movement = new Vector3(moveX, 0, moveZ);
            movement = Vector3.ClampMagnitude(movement, speed);
            movement.y = 0;

            movement *= Time.deltaTime;
            movement = transform.TransformDirection(movement);
            enemyControl.Move(movement);
            transform.GetChild(0).localEulerAngles = Vector3.RotateTowards(transform.localEulerAngles, new Vector3(0, rotY, 0), 0.1f, 360f);
            return false;
        }
    }

    private int GetPriority(Vector2Int point)
    {
        Vector2Int curCell = WorldToMazePoint(new Vector2(transform.position.x, transform.position.z));
        int priority = 0;
        if (curCell.x < point.x)
        {
            if (curCell.y < point.y)
                priority = 1;
            else
                priority = 4;
        }
        else
        {
            if (curCell.y < point.y)
                priority = 2;
            else
                priority = 3;
        }
        return priority;
    }

    public Stack<int> GetCheckOrder(int priority)
    {
        // 1: North, 2: West, 3: South, 4: East
        Stack<int> checkOrder = new Stack<int>();
        switch (priority)
        {
            case 1:
                if (Mathf.Max((Mathf.Abs(transform.position.x - player.transform.position.x)), (Mathf.Abs(transform.position.z - player.transform.position.z)))
                    == (Mathf.Abs(transform.position.z - player.transform.position.z)))
                {
                    checkOrder.Push(3);
                    checkOrder.Push(2);
                    checkOrder.Push(4);
                    checkOrder.Push(1);
                }
                else
                {
                    checkOrder.Push(2);
                    checkOrder.Push(3);
                    checkOrder.Push(1);
                    checkOrder.Push(4);
                }
                break;
            case 2:
                if (Mathf.Max((Mathf.Abs(transform.position.x - player.transform.position.x)), (Mathf.Abs(transform.position.z - player.transform.position.z)))
                    == (Mathf.Abs(transform.position.z - player.transform.position.z)))
                {
                    checkOrder.Push(3);
                    checkOrder.Push(4);
                    checkOrder.Push(2);
                    checkOrder.Push(1);
                }
                else
                {
                    checkOrder.Push(4);
                    checkOrder.Push(3);
                    checkOrder.Push(1);
                    checkOrder.Push(2);
                }
                break;
            case 3:
                if (Mathf.Max((Mathf.Abs(transform.position.x - player.transform.position.x)), (Mathf.Abs(transform.position.z - player.transform.position.z)))
                    == (Mathf.Abs(transform.position.z - player.transform.position.z)))
                {
                    checkOrder.Push(1);
                    checkOrder.Push(4);
                    checkOrder.Push(2);
                    checkOrder.Push(3);
                }
                else
                {
                    checkOrder.Push(4);
                    checkOrder.Push(1);
                    checkOrder.Push(3);
                    checkOrder.Push(2);
                }
                break;
            case 4:
                if (Mathf.Max((Mathf.Abs(transform.position.x - player.transform.position.x)), (Mathf.Abs(transform.position.z - player.transform.position.z)))
                    == (Mathf.Abs(transform.position.z - player.transform.position.z)))
                {
                    checkOrder.Push(1);
                    checkOrder.Push(2);
                    checkOrder.Push(4);
                    checkOrder.Push(3);
                }
                else
                {
                    checkOrder.Push(2);
                    checkOrder.Push(1);
                    checkOrder.Push(3);
                    checkOrder.Push(4);
                }
                break;
            default:
                checkOrder.Push(4);
                checkOrder.Push(3);
                checkOrder.Push(2);
                checkOrder.Push(1);
                break;
        }
        return checkOrder;
    }

    /**
     * Grid cells are pushed into the stack to create a path, popped out if it reaches a dead end.
     */
    private Stack<Vector2Int> GetPathToPoint(Vector2Int point)
    {
        ResetGrid();
        Stack<Vector2Int> path = new Stack<Vector2Int>();

        SingleSpace[,] grid = CreateDeepCopyGrid(mazeInfo.grid);

        Vector2Int curCell = WorldToMazePoint(new Vector2(transform.position.x, transform.position.z));
        Vector2Int moveTo = curCell;
        grid[moveTo.x, moveTo.y].available = true;
        path.Push(moveTo);

        int priority = GetPriority(point);

        while (moveTo != point)
        {
            bool validCell = false;
            Stack<int> checkOrder = GetCheckOrder(priority);
            while (!validCell)
            {
                if (checkOrder.Count > 0)
                {
                    int direction = checkOrder.Pop();
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
                    if (path.Count > 0)
                        moveTo = path.Peek();
                    else
                        moveTo = curCell;
                    validCell = true;
                }
            }

        }

        path.Push(point);
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
        for (int i = 0; i < mazeInfo.numColumns; i++)
        {
            for (int j = 0; j < mazeInfo.numRows; j++)
            {
                mazeInfo.grid[i, j].available = false;
            }
        }
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
        else
        {
            if (speed == chaseSpeed)
                NewChase("Player");
            else
                NewChase("Random");
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
        if (other.gameObject.tag == "Player" && !playerInfo.isCrouching)
        {
            if (timeSinceBirdUp > 3f)
            {
                birdUp.PlayOneShot(birdUp.clip);
                timeSinceBirdUp = 0;
                baKawFrequency = 0.75f;
            }
            buildUp.Play();
            speed = chaseSpeed;
            animator.SetInteger("animState", 3);
            NewChase("Player");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            baKawFrequency = 5f;
        }
    }
}
