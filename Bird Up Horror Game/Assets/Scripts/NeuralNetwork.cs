
using System;
using System.Collections.Generic;

public class NeuralNetwork : IComparable<NeuralNetwork>
{
    private int[] layers;
    private float[][] neurons;
    private float[][][] weights;
    private float fitness;

    public NeuralNetwork(int[] lyrs)
    {
        layers = new int[lyrs.Length];
        for (int i = 0; i < lyrs.Length; i++)
            layers[i] = lyrs[i];
        InitNeurons();
        InitWeights();
    }

    public NeuralNetwork(NeuralNetwork copy)
    {
        layers = new int[copy.layers.Length];
        for (int i = 0; i < copy.layers.Length; i++)
            layers[i] = copy.layers[i];
        InitNeurons();
        InitWeights();
        CopyWeights(copy.weights);
    }

    private void CopyWeights(float[][][] copyWeights)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weights[i][j][k] = copyWeights[i][j][k];
                }
            }
        }
    }

    private void InitNeurons()
    {
        List<float[]> neuronsList = new List<float[]>();
        for (int i = 0; i < layers.Length; i++)
        {
            neuronsList.Add(new float[layers[i]]);
        }
        neurons = neuronsList.ToArray();
    }

    private void InitWeights()
    {
        List<float[][]> weightsList = new List<float[][]>();
        for(int i = 0; i < layers.Length; i++)
        {
            List<float[]> layerWeightList = new List<float[]>();
            int neuronsInPrevLayer;
            if (i > 0)
                neuronsInPrevLayer = layers[i - 1];
            else
                neuronsInPrevLayer = 0;
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[neuronsInPrevLayer];
                for(int k = 0; k < neuronsInPrevLayer; k++)
                {
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                }
                layerWeightList.Add(neuronWeights);
            }
            weightsList.Add(layerWeightList.ToArray());
        }

        weights = weightsList.ToArray();
    }

    public float[] FeedForward(float[] inputs)
    {
        for(int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }
        for(int i = 0; i < layers.Length; i++)
        {
            for(int j = 0; j < neurons[i].Length; j++)
            {
                float value = 0.25f;
                if (i > 0)
                {
                    for (int k = 0; k < neurons[i - 1].Length; k++)
                    {
                        if (i < 1)
                        {
                            value += weights[i - 1][j][k] * neurons[i - 1][k];
                        }
                    }
                }
                neurons[i][j] = (float)Math.Tanh(value);
            }
        }
        return neurons[neurons.Length - 1];
    }

    public void Mutate()
    {
        for(int i = 0; i < weights.Length; i++)
        {
            for(int j = 0; j < weights[i].Length; j++)
            {
                for(int k = 0; k < weights[i][j].Length; k++)
                {
                    float weight = weights[i][j][k];
                    float randomNumber = UnityEngine.Random.Range(0f, 10f);

                    if(randomNumber <= 2f)
                    {
                        weight *= -1f;
                    }
                    else if(randomNumber <= 4f)
                    {
                        weight = UnityEngine.Random.Range(-0.5f, 0.5f);
                    }
                    else if(randomNumber <= 6f)
                    {
                        float factor = UnityEngine.Random.Range(0f, 1f) + 1f;
                        weight *= factor;
                    }
                    else if(randomNumber <= 8f)
                    {
                        float factor = UnityEngine.Random.Range(0f, 1f);
                        weight *= factor;
                    }

                    weights[i][j][k] = weight;
                }
            }
        }
    }

    public void AddFitness(float add)
    {
        fitness += add;
    }

    public void SetFitness(float set)
    {
        fitness = set;
    }

    public float GetFitness()
    {
        return fitness;
    }

    public int CompareTo(NeuralNetwork other)
    {
        if (other == null)
            return 1;
        if (fitness > other.fitness)
            return 1;
        else if (fitness < other.fitness)
            return -1;
        else
            return 0;
    }
}
