using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DontDestroy : MonoBehaviour
{
    //Player Stats/Info
    public int mazesRan;
    public int mazesEscaped;
    public int totalRocksFound;
    public int rocksFound;
    public int mazeNumRows;
    public int mazeNumColumns;
    public int numRocks;
    public float enemyChaseSpeed;
    public float enemySearchSpeed;
    public int countdownTime;

    public bool PAUSED;

    [SerializeField]
    private GameObject GameOverScreenPrefab;
    [SerializeField]
    private GameObject GameWonScreenPrefab;

    private GameObject player, loadScreen;
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
        PAUSED = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (PAUSED)
            {
                UnPauseGame();
            }
            else
            {
                PauseGame();
            }
        }
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
            mazesRan = loadedData.mazesRan;
            mazesEscaped = loadedData.mazesEscaped;
            totalRocksFound = loadedData.totalRocksFound;
            rocksFound = loadedData.rocksFound;
            mazeNumRows = loadedData.mazeNumRows;
            mazeNumColumns = loadedData.mazeNumColumns;
            numRocks = loadedData.numRocks;
            enemyChaseSpeed = loadedData.enemyChaseSpeed;
            enemySearchSpeed = loadedData.enemySearchSpeed;
            countdownTime = loadedData.countdownTime;
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
            mazesRan = loadedData.mazesRan;
            mazesEscaped = loadedData.mazesEscaped;
            totalRocksFound = loadedData.totalRocksFound;
            rocksFound = loadedData.rocksFound;
            mazeNumRows = loadedData.mazeNumRows;
            mazeNumColumns = loadedData.mazeNumColumns;
            numRocks = loadedData.numRocks;
            enemyChaseSpeed = loadedData.enemyChaseSpeed;
            enemySearchSpeed = loadedData.enemySearchSpeed;
            countdownTime = loadedData.countdownTime;
        }
        catch(UnityException e)
        {
            Debug.LogError("Error in loading new game");
        }
    }

    //Loads the desired scene
    public void LoadScene(string sceneName)
    {
        UnPauseGame();
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
        loadScreenProgress.gameObject.SetActive(false);

        while (loadScreen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color.a < 1.0F)
        {
            Color curTransparency = loadScreen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color;
            curTransparency.a += (Time.deltaTime / 1F) / 1.0F;
            if (curTransparency.a > 1.0F)
                curTransparency.a = 1.0F;
            loadScreen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = curTransparency;
            yield return null;
        }

        loadScreenProgress.gameObject.SetActive(true);

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
        UnPauseGame();
        rocksFound = 0;
        LoadScene("Game");
    }

    public void SelectDifficulty()
    {
        GameObject.FindWithTag("MainMenu").transform.GetChild(0).gameObject.SetActive(false);
        GameObject.FindWithTag("MainMenu").transform.GetChild(1).gameObject.SetActive(true);
    }

    public void ToMainMenu()
    {
        if(SceneManager.GetActiveScene().name != "Menu")
        {
            UnPauseGame();
            Cursor.visible = true;
            LoadScene("Menu");
        }
        else
        {
            GameObject.FindWithTag("MainMenu").transform.GetChild(0).gameObject.SetActive(true);
            GameObject.FindWithTag("MainMenu").transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void IncrementMazesRan()
    {
        mazesRan++;
    }

    public void IncrementMazesEscaped()
    {
        mazesEscaped++;
    }

    public void IncrementRocksFound()
    {
        totalRocksFound++;
        rocksFound++;
        if (rocksFound >= numRocks)
            GameWon();
    }

    public void SetNumRows(int set)
    {
        mazeNumRows = set;
    }

    public void SetNumColumns(int set)
    {
        mazeNumColumns = set;
    }

    public void SetNumRocks(int set)
    {
        numRocks = set;
    }

    public void SetChaseSpeed(float set)
    {
        enemyChaseSpeed = set;
    }

    public void SetSearchSpeed(float set)
    {
        enemySearchSpeed = set;
    }

    public void SetTimeLimit(int set)
    {
        countdownTime = set;
    }

    public void PauseGame()
    {
        PAUSED = true;
        Time.timeScale = 0f;
    }

    public void UnPauseGame()
    {
        PAUSED = false;
        Time.timeScale = 1f;
    }

    public void GameLost()
    {
        StartCoroutine(DisplayGameOver());
    }

    public void GameWon()
    {
        StartCoroutine(DisplayWon());
    }

    IEnumerator DisplayGameOver()
    {
        float timeElapsed = 0.0f;
        GameObject screen = GameObject.Instantiate(GameOverScreenPrefab);
        Color temp = screen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color;
        temp.a = 0F;
        screen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = temp;
        screen.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);

        while (screen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color.a < 1.0F)
        {
            Color curTransparency = screen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color;
            curTransparency.a += (Time.deltaTime / 1F) / 1.0F;
            if (curTransparency.a > 1.0F)
                curTransparency.a = 1.0F;
            screen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = curTransparency;
            yield return null;
        }

        screen.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);

        while (timeElapsed < 3f)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        IncrementMazesRan();
        ToMainMenu();
    }

    IEnumerator DisplayWon()
    {
        float timeElapsed = 0.0f;
        GameObject screen = GameObject.Instantiate(GameWonScreenPrefab);
        Color temp = screen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color;
        temp.a = 0F;
        screen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = temp;
        screen.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);

        while (screen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color.a < 1.0F)
        {
            Color curTransparency = screen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color;
            curTransparency.a += (Time.deltaTime / 1F) / 1.0F;
            if (curTransparency.a > 1.0F)
                curTransparency.a = 1.0F;
            screen.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = curTransparency;
            yield return null;
        }

        screen.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);

        while (timeElapsed < 3f)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        IncrementMazesRan();
        IncrementMazesEscaped();
        ToMainMenu();
    }
}

