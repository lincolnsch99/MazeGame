using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DontDestroy : MonoBehaviour
{
    //Player Stats/Info
    public List<float> scores;

    private GameObject player, loadScreen;
    private float saveTimer;
    private int levelCount;
    private string currentLevel;
    private Slider loadScreenProgress;

    private void Awake()
    {
        SaveData.CreateNewGameFile();
        DontDestroyOnLoad(this.gameObject);
        if (SceneManager.GetActiveScene().buildIndex == 0)
            SceneManager.LoadScene("Menu");
        player = GameObject.FindWithTag("Player");
        GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i].tag == "LoadScreen")
            {
                loadScreen = objects[i];
                i = objects.Length;
            }
        }
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i].tag == "LoadScreenSlider")
            {
                loadScreenProgress = objects[i].GetComponent<Slider>();
                i = objects.Length;
            }
        }
        saveTimer = 0;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        DontDestroyOnLoad(this.gameObject);

        if (SceneManager.GetActiveScene().name != "InitScene")
        {
            player = GameObject.FindWithTag("Player");
            GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>();
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].tag == "LoadScreen")
                {
                    loadScreen = objects[i];
                    i = objects.Length;
                }
            }
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].tag == "LoadScreenSlider")
                {
                    loadScreenProgress = objects[i].GetComponent<Slider>();
                    i = objects.Length;
                }
            }
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        saveTimer += Time.deltaTime;
        if (saveTimer > 120F)
        {
            saveTimer = 0;
            SaveStats();
            Debug.Log("GameSaved");
        }
    }

    //Saves the current player stats in the local save file
    public void SaveStats()
    {
        SaveData.Save(this);
    }

    //Loads the stats from the save file, if found
    public void LoadGame()
    {
        try
        {
            GameData loadedData = SaveData.Load();

            scores = loadedData.scores;
        }
        catch
        {
            Debug.LogError("Error in loading save file");
        }
    }

    //Sets stats and values to a New Game status, using a stored "New Game" file
    public void NewGame()
    {
        try
        {
            GameData loadedData = SaveData.NewGame();
            scores = loadedData.scores;
        }
        catch(UnityException e)
        {
            Debug.LogError("Error in loading new game");
            Debug.LogError(e.ToString());
        }
    }

    //Sets levelCount back to 1
    public void ResetLevel()
    {
        levelCount = 1;
    }

    //Increments the levelCount
    public void IncrementLevel()
    {
        levelCount++;
    }

    //Sets the name of the current level
    public void SetLevelName(string set)
    {
        currentLevel = set;
    }

    //Returns the level floor number, in corrent syntax. Ex:
    // Level : 0
    public string LevelToString()
    {
        if (currentLevel == "Camp")
            return currentLevel;
        return currentLevel + "\nLevel : " + levelCount.ToString();
    }

    //Returns the name of the current level
    public string CurrentLevelName()
    {
        return currentLevel;
    }

    //Loads the desired scene
    public void LoadScene(string sceneName)
    {
        if (currentLevel == sceneName)
        {
            IncrementLevel();
        }
        else
            ResetLevel();
        SetLevelName(sceneName);
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    //Handles the fade in and such with the loading screen
    IEnumerator LoadSceneAsync(string sceneName)
    {
        if (loadScreen != null)
            loadScreen.SetActive(true);
        Color temp = loadScreen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color;
        temp.a = 0F;
        loadScreen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = temp;

        while (loadScreen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color.a < 1.0F)
        {
            Color curTransparency = loadScreen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color;
            curTransparency.a += (Time.deltaTime / 1F) / 1.0F;
            if (curTransparency.a > 1.0F)
                curTransparency.a = 1.0F;
            loadScreen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = curTransparency;
            yield return null;
        }

        loadScreen.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);


        AsyncOperation operation =  SceneManager.LoadSceneAsync(sceneName);
        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9F);
            if(loadScreenProgress != null)
                loadScreenProgress.value = progress;
            yield return null;
        }
        if(player != null)
            player.transform.position = new Vector3(0f, 0F, 0F);
    }

    public void QuitGame()
    {
        SaveStats();
        Application.Quit();
    }

    public void PlayGame()
    {
        LoadScene("Game");
    }
}

