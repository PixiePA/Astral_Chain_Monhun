using NUnit.Framework;
using System.Collections.Generic;
using TreeEditor;
using UnityEditor.Rendering;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    public int smoothingSegments = 10;
    public float smoothingLength = 2f;
    public List<Transform> ropeNodes = new List<Transform>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        if (!lineRenderer)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        ropeNodes = new List<Transform>(GetComponentsInChildren<Transform>());
    }

    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {

        //Giving line renderer points
        if (ropeNodes.Count > 2)
        {
            DrawBezierCurves(GenerateCurvesAlongRopeNodes());
        }
        else
        {
            //Draw between the 0 to 2 points with no smoothing
            lineRenderer.positionCount = ropeNodes.Count;
            for (int i = 0; i < ropeNodes.Count; i++)
            {
                lineRenderer.SetPosition(i, ropeNodes[i].position);
            }
        }
    }

    private void DrawBezierCurves(BezierCurve[] curves)
    {
        //Add segments to line renderer
        int index = 0;

        for (int i = 0; i < curves.Length; i++)
        {
            Vector3[] segments = curves[i].GetSegments(smoothingSegments);
            for (int j = 0; j < segments.Length; j++)
            {
                //if statement prevents repeat points
                if (j < segments.Length - 1 || i == curves.Length - 1)
                {
                    lineRenderer.SetPosition(index, segments[j]);
                    index++;
                }
            }
        }
    }

    private BezierCurve[] GenerateCurvesAlongRopeNodes()
    {
        if (ropeNodes.Count > 1)
        {
            //Setting up bezier curves through nodes
            BezierCurve[] curves = new BezierCurve[ropeNodes.Count - 1];
            lineRenderer.positionCount = (curves.Length - 1) * (smoothingSegments - 1) + smoothingSegments;

            //Set up first curve
            curves[0] = new BezierCurve(ropeNodes[0].position, ropeNodes[1].position, smoothingLength);


            //Set up other curves
            for (int i = 1; i < curves.Length; i++)
            {
                curves[i] = new BezierCurve(ropeNodes[i - 1].position, ropeNodes[i].position, ropeNodes[i + 1].position, smoothingLength);
            }

            //Lookahead for first bezier curve
            Vector3 nextDirection = (curves[1].EndPositon - curves[1].StartPositon).normalized;
            Vector3 prevDirection = (curves[0].EndPositon - curves[0].StartPositon).normalized;
            curves[0].Points[2] = curves[0].Points[3] + (prevDirection + nextDirection) * -smoothingLength;

            return curves;
        }

        return new BezierCurve[] { };
    }
}
