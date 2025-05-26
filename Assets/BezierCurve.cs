using System;
using UnityEngine;

public class BezierCurve
{
    public Vector3[] Points;

    public Vector3 StartPositon
    {
        get
        {
            return Points[0];
        }
    }

    public Vector3 EndPositon
    {
        get
        {
            return Points[3];
        }
    }

    public BezierCurve()
    {
        Points = new Vector3[4];
    }

    public Vector3 GetSegment(float time)
    {
        time = Mathf.Clamp01(time);
        float inverseTime = 1 - time;

        // Applying Cubic Bezier Curve formula
        return 
            (Mathf.Pow(inverseTime, 3) * Points[0]) +
            3 * Mathf.Pow(inverseTime, 2) * time * Points[1] +
            3 * inverseTime * Mathf.Pow(time, 2) * Points[2] +
            Mathf.Pow(time, 3) * Points[3];
    }

    public Vector3[] GetSegments(int subdivisions)
    {
        Vector3[] segments = new Vector3[subdivisions];

        float time;

        for (int i = 0; i < subdivisions; i++)
        {
            time = (float)i / subdivisions;
            segments[i] = GetSegment(time);
        }

        return segments;
    }
}
