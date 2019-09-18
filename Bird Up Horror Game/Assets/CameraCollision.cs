using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraCollision : MonoBehaviour
{
    [SerializeField]
    private Text textPrompt;
    [SerializeField]
    private PlayerMovement player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Key")
        {
            textPrompt.gameObject.SetActive(true);
            textPrompt.text = "Press 'E' to take key";
        }
        else if (other.gameObject.tag == "Door")
        {
            textPrompt.gameObject.SetActive(true);
            
            textPrompt.text = "Press 'E' to open door";
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Key" || other.gameObject.tag == "Door")
        {
            textPrompt.gameObject.SetActive(false);
        }
    }
}
