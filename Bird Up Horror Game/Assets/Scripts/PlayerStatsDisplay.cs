using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsDisplay : MonoBehaviour
{
    private DontDestroy PersistentData;
    // Start is called before the first frame update
    private void Start()
    {
        PersistentData = GameObject.FindWithTag("Persistent").GetComponent<DontDestroy>();
        GetComponent<Text>().text = "Mazes Escaped: " + PersistentData.mazesEscaped +
            "\nRocks Found: " + PersistentData.totalRocksFound + "\nDeaths: " + (PersistentData.mazesRan - PersistentData.mazesEscaped);
    }
}
