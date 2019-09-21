using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsSettings : MonoBehaviour
{
    public AudioMixer mixer;

    [SerializeField]
    private Slider sensXInput;
    [SerializeField]
    private Slider sensYInput;
    [SerializeField]
    private Slider volumeInput;

    private DontDestroy PersistentData;

    private void Awake()
    {
        PersistentData = GameObject.FindWithTag("Persistent").GetComponent<DontDestroy>();
        sensXInput.value = PersistentData.mouseSensX;
        sensYInput.value = PersistentData.mouseSensY;
        volumeInput.value = PersistentData.volume;
    }

    public void UpdateSensX()
    {
        PersistentData.mouseSensX = sensXInput.value;
    }

    public void UpdateSensY()
    {
        PersistentData.mouseSensY = sensYInput.value;
    }

    public void UpdateVolume()
    {
        PersistentData.volume = volumeInput.value;
        mixer.SetFloat("MasterVolume", Mathf.Log10(PersistentData.volume) * 20);
    }
}
