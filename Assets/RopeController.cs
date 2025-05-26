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
    private List<Transform> ropeNodes = new List<Transform>();
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
            lineRenderer.positionCount = curves.Length * smoothingSegments;

            //Set up first curve
            curves[0] = new BezierCurve();
            SetupCurve(ropeNodes[0].position, ropeNodes[1].position, curves[0]);


            //Set up other curves
            for (int i = 1; i< curves.Length; i++)
            {
                curves[i] = new BezierCurve();
                SetupCurve(ropeNodes[i - 1].position, ropeNodes[i].position, ropeNodes[i + 1].position, curves[i]);
            }


            //Add segments to line renderer
            int index = 0;

            foreach (BezierCurve curve in curves)
            {
                Vector3[] segments = curve.GetSegments(smoothingSegments);
                foreach (Vector3 segment in segments)
                {
                    lineRenderer.SetPosition(index, segment);
                    index++;
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
        Vector3 lastDirection = (position - prevPosition).normalized;
        Vector3 nextDirection = (nextPosition - prevPosition).normalized;

        Vector3 startTangent = (lastDirection + nextDirection) * smoothingLength;
        Vector3 endTangent = (nextDirection + lastDirection) * -smoothingLength;

        curve.Points[0] = position;
        curve.Points[1] = position+startTangent;
        curve.Points[2] = nextPosition + endTangent;
        curve.Points[3] = nextPosition;

    }

    private void SetupCurve(Vector3 position, Vector3 nextPosition, BezierCurve curve)
    {
        SetupCurve(position, position, nextPosition, curve);
    }
}
