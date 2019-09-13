using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Vector : IEquatable<Vector>
{
    public float[] values;
    public Vector(int length, bool randomize = false)
    {
        values = new float[length];
        if(randomize)
        {
            for (int i = 0; i < length; i++)
                values[i] = UnityEngine.Random.Range(-1f, 1f);
        }
    }

    public Vector(float[] copy)
    {
        values = copy;
    }

    public void JoinVector(Vector join)
    {
        float[] newValues = new float[values.Length + join.values.Length];
        for(int i = 0; i < values.Length; i++)
        {
            newValues[i] = values[i];
        }
        for(int i = 0; i < join.values.Length; i++)
        {
            newValues[i + values.Length] = join.values[i];
        }
    }

    public bool Equals(Vector compare)
    {
        if (compare.values.Length != values.Length)
            return false;
        for(int i = 0; i < values.Length; i++)
        {
            if (compare.values[i] != values[i])
                return false;
        }
        return true;
    }

    public int Length()
    {
        return values.Length;
    }
}
