using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public string ButtonType;
    private DontDestroy PersistentInfo;

    private void Awake()
    {
        PersistentInfo = GameObject.FindWithTag("Persistent").GetComponent<DontDestroy>();
        Button thisButton = GetComponent<Button>();
        if (ButtonType == "Save")
            thisButton.onClick.AddListener(PersistentInfo.SaveStats);
        else if (ButtonType == "Load")
            thisButton.onClick.AddListener(PersistentInfo.LoadGame);
        else if (ButtonType == "Resume")
            thisButton.onClick.AddListener(PersistentInfo.UnPauseGame);
        else if (ButtonType == "Menu")
            thisButton.onClick.AddListener(PersistentInfo.ToMainMenu);
        else if (ButtonType == "NewGame")
            thisButton.onClick.AddListener(PersistentInfo.NewGame);
        else if (ButtonType == "Options")
            thisButton.onClick.AddListener(this.LoadOptions);
        else if (ButtonType == "Quit")
            thisButton.onClick.AddListener(PersistentInfo.QuitGame);
        else if (ButtonType == "Difficulty")
            thisButton.onClick.AddListener(PersistentInfo.SelectDifficulty);
        else if (ButtonType == "Play")
            thisButton.onClick.AddListener(PersistentInfo.PlayGame);
    }

    public void LoadOptions()
    {
        // activate options screen here
    }
}
