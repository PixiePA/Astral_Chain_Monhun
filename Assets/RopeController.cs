using NUnit.Framework;
using System.Collections.Generic;
using TreeEditor;
using UnityEditor.Rendering;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeController : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    public int smoothingSegments = 10;
    public float smoothingLength = 2f;
    public List<Transform> ropeNodes = new List<Transform>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Start()
    {
        ropeNodes = new List<Transform>(GetComponentsInChildren<Transform>());
    }

    // Update is called once per frame
    void Update()
    {

        //Giving line renderer points
        if (ropeNodes.Count > 2)
        {
            BezierCurve[] curves = new BezierCurve[ropeNodes.Count - 1];
            lineRenderer.positionCount = (curves.Length - 1) * (smoothingSegments - 1) + smoothingSegments;

            //Set up first curve
            curves[0] = new BezierCurve(GetPoints(ropeNodes[0].position, ropeNodes[1].position));


            //Set up other curves
            for (int i = 1; i < curves.Length; i++)
            {
                curves[i] = new BezierCurve(GetPoints(ropeNodes[i - 1].position, ropeNodes[i].position, ropeNodes[i + 1].position));
            }

            Vector3 nextDirection = (curves[1].EndPositon - curves[1].StartPositon).normalized;
            Vector3 prevDirection = (curves[0].EndPositon - curves[0].StartPositon).normalized;
            curves[0].Points[2] = curves[0].Points[3] + (prevDirection + nextDirection) * -smoothingLength;


            //Add segments to line renderer
            int index = 0;

            for (int i = 0; i < curves.Length; i++)
            {
                Vector3[] segments = curves[i].GetSegments(smoothingSegments);
                for (int j = 0; j < segments.Length; j++)
                {
                    if (j < segments.Length - 1 || i == curves.Length - 1)
                    {
                        lineRenderer.SetPosition(index, segments[j]);
                        index++;
                    }
                }
            }
        }
        else
        {
            lineRenderer.positionCount = ropeNodes.Count;
            for (int i = 0; i < ropeNodes.Count; i++)
            {
                lineRenderer.SetPosition(i, ropeNodes[i].position);
            }
        }
    }

    private void SetupCurve(Vector3 prevPosition, Vector3 position, Vector3 nextPosition, BezierCurve curve)
    {
        Vector3[] points = GetPoints(prevPosition, position, nextPosition);

        curve.Points[0] = points[0];
        curve.Points[1] = points[1];
        curve.Points[2] = points[2];
        curve.Points[3] = points[3];

    }

    private void SetupCurve(Vector3 position, Vector3 nextPosition, BezierCurve curve)
    {
        SetupCurve(position, position, nextPosition, curve);
    }

    private Vector3[] GetPoints(Vector3 prevPosition, Vector3 position, Vector3 nextPosition)
    {
        Vector3 lastDirection = (position - prevPosition).normalized;
        Vector3 nextDirection = (nextPosition - position).normalized;

        Vector3 startTangent = (lastDirection + nextDirection) * smoothingLength;
        Vector3 endTangent = (lastDirection + nextDirection) * -smoothingLength;

        return new Vector3[] { position, position + startTangent, nextPosition + endTangent, nextPosition };
    }

    private Vector3[] GetPoints(Vector3 position, Vector3 nextPosition)
    {
        return GetPoints(position, position, nextPosition);
    }
}
