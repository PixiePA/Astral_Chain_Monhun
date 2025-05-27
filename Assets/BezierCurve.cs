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

    /// <summary>
    /// Creates a new bezier curve with points using given values
    /// <param name="prevPosition">Position of the last point<param>  
    /// <param name="position">Position of the current point</param>
    /// <param name="nextPosition">Position of the next point<param>  
    /// <param name="smoothingLength">how extreme should the curve be</param>
    /// </summary> 
    public BezierCurve(Vector3 prevPosition, Vector3 position, Vector3 nextPosition, float smoothingLength)
    {
        Points = GetPoints(prevPosition, position, nextPosition, smoothingLength);
    }


    /// <summary>
    /// Creates a new bezier curve when theres no previous point
    /// <param name="position">Position of the current point</param>
    /// <param name="nextPosition">Position of the next point<param>  
    /// <param name="smoothingLength">how extreme should the curve be</param>  
    /// </summary> 
    public BezierCurve(Vector3 position, Vector3 nextPosition, float smoothingLength)
    {
        Points = GetPoints(position, position, nextPosition, smoothingLength);
    }

    public BezierCurve(Vector3[] points)
    {
        if (points.Length > 4)
        {
            throw new ArgumentException("Bezier array length must be 4");
        }
        else
        {
            Points = points;
        }
    }
    
    //Gets point along bezier curve
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


    //Generates array of points along bezier curve
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

    //Generates points along curve
    private Vector3[] GetPoints(Vector3 prevPosition, Vector3 position, Vector3 nextPosition, float smoothingLength)
    {
        Vector3 lastDirection = (position - prevPosition).normalized;
        Vector3 nextDirection = (nextPosition - position).normalized;

        Vector3 startTangent = (lastDirection + nextDirection) * smoothingLength;
        Vector3 endTangent = (lastDirection + nextDirection) * -smoothingLength;

        return new Vector3[] { position, position + startTangent, nextPosition + endTangent, nextPosition };
    }
}
