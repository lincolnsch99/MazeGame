using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer
{
    public float[][] matrix;
    string activation;

    public Layer(int inputDimension, int outputDimension, string activationType = "tanh")
    {
        activation = activationType;
        matrix = new float[outputDimension][];
        for (int i = 0; i < outputDimension; i++)
        {
            matrix[i] = new float[inputDimension];
            for (int j = 0; j < inputDimension + 1; j++)
            {
                matrix[i][j] = Random.Range(-2f, 2f);
            }
        }
    }

    public Vector LayerOutput(Vector input)
    {
        Vector output = new Vector(matrix.Length);
        for(int i = 0; i < matrix.Length; i++)
        {
            if (matrix[i].Length - 1 == input.Length())
            {
                for(int j = 0; j < matrix[i].Length - 1; j++)
                {
                    float weightedValue = input.values[j] * matrix[i][j];
                    if(weightedValue != weightedValue)
                    {
                        weightedValue = 0;
                    }
                    output.values[i] += 1f * matrix[i][matrix[i].Length - 1];
                    output.values[i] = Activations.Tanh(output.values[i]);
                }
            }
            else
            {
                throw new System.Exception(($"Matrix column length ({ matrix[i].Length }) is not the same as ({ input.Length() })"));
            }
        }
        return output;
    }
}
