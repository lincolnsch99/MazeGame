using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockDisplay : MonoBehaviour
{
    private DontDestroy PersistentData;
    private int localRocksFound;

    private void Awake()
    {
        PersistentData = GameObject.FindWithTag("Persistent").GetComponent<DontDestroy>();
        RemoveRockDisplays();
        SetDisplays(PersistentData.numRocks);
    }

    private void Update()
    {
        if(localRocksFound != PersistentData.rocksFound)
        {
            localRocksFound = PersistentData.rocksFound;
            DisplayRocksFound();
        }
    }

    private void RemoveRockDisplays()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
            transform.GetChild(i).GetChild(2).gameObject.SetActive(false);
        }
    }

    private void SetDisplays(int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void DisplayRocksFound()
    {
        for (int i = 0; i < localRocksFound; i++)
        {
            transform.GetChild(i).GetChild(2).gameObject.SetActive(true);
            transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
        }
    }
}
