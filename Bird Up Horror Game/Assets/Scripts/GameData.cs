using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameData
{
    [SerializeField]
    public List<float> scores;

    public GameData(DontDestroy info)
    {
        scores = info.scores;
    }

    public GameData()
    {
        scores = new List<float>();
    }
}
