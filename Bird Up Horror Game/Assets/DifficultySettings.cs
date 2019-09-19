using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySettings : MonoBehaviour
{
    public InputField rowsInput;
    public InputField columnsInput;
    public InputField chaseSpeedInput;
    public InputField searchSpeedInput;
    public InputField numRocksInput;
    public GameObject InvalidInputMessage;

    private DontDestroy PersistentData;

    private void Awake()
    {
        PersistentData = GameObject.FindWithTag("Persistent").GetComponent<DontDestroy>();
        InvalidInputMessage.SetActive(false);
    }

    public void UpdateRows(int set = -1)
    {
        int value;
        if (set != -1)
            value = set;
        else
            int.TryParse(rowsInput.text, out value);

        if (value > 100)
            value = 100;
        else if(value < 10)
            value = 10;

        rowsInput.text = value.ToString();
        PersistentData.SetNumRows(value);
    }

    public void UpdateColumns(int set = -1)
    {
        int value;
        if (set != -1)
            value = set;
        else
            int.TryParse(columnsInput.text, out value);

        if (value > 100)
            value = 100;
        else if (value < 10)
            value = 10;

        columnsInput.text = value.ToString();
        PersistentData.SetNumColumns(value);
    }

    public void UpdateChaseSpeed(int set = -1)
    {
        int value;
        if (set != -1)
            value = set;
        else
            int.TryParse(chaseSpeedInput.text, out value);

        if (value > 25)
            value = 25;
        else if (value < 5)
            value = 5;

        chaseSpeedInput.text = value.ToString();
        PersistentData.SetChaseSpeed(value);
    }

    public void UpdateSearchSpeed(int set = -1)
    {
        int value;
        if (set != -1)
            value = set;
        else
            int.TryParse(searchSpeedInput.text, out value);

        if (value > 25)
            value = 25;
        else if (value < 5)
            value = 5;

        searchSpeedInput.text = value.ToString();
        PersistentData.SetSearchSpeed(value);
    }

    public void UpdateNumRocks(int set = -1)
    {
        int value;
        if (set != -1)
            value = set;
        else
            int.TryParse(numRocksInput.text, out value);

        if (value > 15)
            value = 15;
        else if (value < 3)
            value = 3;

        numRocksInput.text = value.ToString();
        PersistentData.SetNumRocks(value);
    }

    public void SetPremadeDifficulty(string difficulty)
    {
        switch (difficulty)
        {
            case "Easy":
                UpdateRows(10);
                UpdateColumns(10);
                UpdateChaseSpeed(10);
                UpdateSearchSpeed(5);
                UpdateNumRocks(3);
                break;
            case "Medium":
                UpdateRows(25);
                UpdateColumns(25);
                UpdateChaseSpeed(15);
                UpdateSearchSpeed(10);
                UpdateNumRocks(5);
                break;
            case "Hard":
                UpdateRows(50);
                UpdateColumns(50);
                UpdateChaseSpeed(20);
                UpdateSearchSpeed(15);
                UpdateNumRocks(8);
                break;
        }
    }

    public void TryPlay()
    {
        if (PersistentData.mazeNumRows == 0 || PersistentData.mazeNumColumns == 0
            || PersistentData.numRocks == 0 || PersistentData.enemyChaseSpeed == 0
            || PersistentData.enemySearchSpeed == 0)
        {
            InvalidInputMessage.SetActive(true);
        }
        else
            PersistentData.PlayGame();
    }
}
