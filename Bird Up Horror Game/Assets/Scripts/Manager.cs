using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{

    public GameObject boomerPrefab;
    public GameObject player;
    public Slider timeScaleSlider;
    public Text generationDisplay;
    private MazeGenerator mazeInfo;

    private bool isTraning = false;
    private int populationSize = 50;
    private int generationNumber = 0;
    private int[] layers = new int[] { 1, 10, 10, 1 }; //1 input and 1 output
    private List<NeuralNetwork> nets;
    private List<EnemyController> boomerangList = null;


    private void Start()
    {
        mazeInfo = GameObject.FindWithTag("Maze").GetComponent<MazeGenerator>();
        player = GameObject.FindWithTag("Player");
        Time.timeScale = 5f;
    }

    void Timer()
    {
        isTraning = false;
    }

    public void SetTimeScale()
    {
        Time.timeScale = timeScaleSlider.value;
    }


    void Update()
    {
        if (isTraning == false)
        {
            if (generationNumber == 0)
            {
                InitBoomerangNeuralNetworks();
            }
            else
            {
                nets.Sort();
                for (int i = 0; i < populationSize / 2; i++)
                {
                    nets[i] = new NeuralNetwork(nets[i + (populationSize / 2)]);
                    nets[i].Mutate();

                    nets[i + (populationSize / 2)] = new NeuralNetwork(nets[i + (populationSize / 2)]); //too lazy to write a reset neuron matrix values method....so just going to make a deepcopy lol
                }

                for (int i = 0; i < populationSize; i++)
                {
                    nets[i].SetFitness(0f);
                }
            }


            generationNumber++;
            generationDisplay.text = "Generation: " + generationNumber;
            isTraning = true;
            Invoke("Timer", 15f);
            CreateBoomerangBodies();
        }
    }


    private void CreateBoomerangBodies()
    {
        if (boomerangList != null)
        {
            for (int i = 0; i < boomerangList.Count; i++)
            {
                GameObject.Destroy(boomerangList[i].gameObject);
            }

        }

        boomerangList = new List<EnemyController>();

        for (int i = 0; i < populationSize; i++)
        {
            EnemyController boomer = Instantiate(boomerPrefab, transform).GetComponent<EnemyController>();
            float spawnX = 6f * (mazeInfo.numColumns - 1);
            float spawnZ = 6f * (mazeInfo.numRows - 1);
            boomer.transform.position = new Vector3(Random.Range(-2f, 2f) + spawnX, 3, Random.Range(-2f, 2f) + spawnZ);
            boomer.Init(nets[i], player.transform);
            boomerangList.Add(boomer);
        }

    }

    void InitBoomerangNeuralNetworks()
    {
        //population must be even, just setting it to 20 incase it's not
        if (populationSize % 2 != 0)
        {
            populationSize = 20;
        }

        nets = new List<NeuralNetwork>();


        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetwork net = new NeuralNetwork(layers);
            net.Mutate();
            nets.Add(net);
        }
    }
}
