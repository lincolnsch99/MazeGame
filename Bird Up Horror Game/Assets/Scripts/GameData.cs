using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameData
{
    [SerializeField]
    public int mazesRan;
    [SerializeField]
    public int mazesEscaped;
    [SerializeField]
    public int rocksFound;
    [SerializeField]
    public int mazeNumRows;
    [SerializeField]
    public int mazeNumColumns;
    [SerializeField]
    public int numRocks;
    [SerializeField]
    public float enemyChaseSpeed;
    [SerializeField]
    public float enemySearchSpeed;

    public GameData(DontDestroy info)
    {
        mazesRan = info.mazesRan;
        mazesEscaped = info.mazesEscaped;
        rocksFound = info.rocksFound;
        mazeNumRows = info.mazeNumRows;
        mazeNumColumns = info.mazeNumColumns;
        numRocks = info.numRocks;
        enemyChaseSpeed = info.enemyChaseSpeed;
        enemySearchSpeed = info.enemySearchSpeed;
    }

    public GameData()
    {
        mazesRan = 0;
        mazesEscaped = 0;
        rocksFound = 0;
        mazeNumRows = 0;
        mazeNumColumns = 0;
        numRocks = 0;
        enemyChaseSpeed = 0;
        enemySearchSpeed = 0;
    }
}
