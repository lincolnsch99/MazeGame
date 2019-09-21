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
    public int totalRocksFound;
    [SerializeField]
    public float mouseSensX;
    [SerializeField]
    public float mouseSensY;
    [SerializeField]
    public float volume;

    public GameData(DontDestroy info)
    {
        mazesRan = info.mazesRan;
        mazesEscaped = info.mazesEscaped;
        totalRocksFound = info.totalRocksFound;
        mouseSensX = info.mouseSensX;
        mouseSensY = info.mouseSensY;
        volume = info.volume;
    }

    public GameData()
    {
        mazesRan = 0;
        mazesEscaped = 0;
        totalRocksFound = 0;
        mouseSensX = 5f;
        mouseSensX = 5f;
        volume = 1f;
    }
}
