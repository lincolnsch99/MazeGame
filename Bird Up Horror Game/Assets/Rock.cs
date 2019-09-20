using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    private DontDestroy PersistentData;
    private bool visible;

    private void Awake()
    {
        PersistentData = GameObject.FindWithTag("Persistent").GetComponent<DontDestroy>();
        int rand = Random.Range(0, transform.childCount - 1);
        transform.GetChild(rand).gameObject.SetActive(true);
        transform.position = new Vector3(Random.Range(0, PersistentData.mazeNumColumns) * 6f, 2f, Random.Range(0, PersistentData.mazeNumRows) * 6f);
        visible = false;
    }

    private void Update()
    {
        if (!PersistentData.PAUSED)
        {
            if (visible)
            {
                bool playerIsClose = false;
                Collider[] objectsHit = Physics.OverlapSphere(transform.position, 2.5f);
                for (int i = 0; i < objectsHit.Length; i++)
                {
                    if (objectsHit[i].tag == "Player")
                    {
                        playerIsClose = true;
                        transform.GetChild(transform.childCount - 1).gameObject.SetActive(true);
                    }
                }
                if (!playerIsClose)
                    transform.GetChild(transform.childCount - 1).gameObject.SetActive(false);
                else
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        PersistentData.IncrementRocksFound();
                        this.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    private void OnBecameVisible()
    {
        visible = true;
    }
    private void OnBecameInvisible()
    {
        visible = false;
        transform.GetChild(transform.childCount - 1).gameObject.SetActive(false);
    }
}
