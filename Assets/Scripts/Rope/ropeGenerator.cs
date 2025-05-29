using Mono.Cecil.Cil;
using UnityEngine;

[RequireComponent(typeof(Joint))]
public class ropeGenerator : MonoBehaviour
{
    public GameObject ropeEnd;
    public RopeRenderer ropeRenderer;
    public float distanceBeforeNewNode = 2f;
    public float nodeKillDistance = 0.3f;
    public float reelForce = 300f;
    public int maxNodes = 10;
    [SerializeField] private GameObject ropeSegmentPrefab;
    [SerializeField] private Joint joint;
    public bool isBinding;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float sqrDistanceBeforeNewNode
    {
        get
        {
            return distanceBeforeNewNode * distanceBeforeNewNode;
        }
    }

    public float sqrNodeKillDistance
    {
        get
        {
            return nodeKillDistance * nodeKillDistance;
        }
    }

    private void Awake()
    {
        if (!joint)
        {
            joint = GetComponent<Joint>();
        }

        if (!ropeRenderer)
        {
            ropeRenderer = ropeEnd.GetComponent<RopeRenderer>();
        }
    }

    void Start()
    {
        CreateNewRope();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (isBinding)
        {

        }
        // If rope has slack, normal rope behaviour
        else
        {
            
            //Shortens rope if next node can be easily reached
            if (ropeRenderer.ropeNodes.Count > 3)
            {
                bool ropeTooLong = false;
                for (int i = 0; i < ropeRenderer.ropeNodes.Count - 2; i++)
                {
                    if ((transform.position - ropeRenderer.ropeNodes[i].position).sqrMagnitude < sqrDistanceBeforeNewNode)
                    {
                        ropeTooLong = true;
                        break;
                    }
                }

                if (ropeTooLong)
                {
                    ReelInRope();
                }
            }

            //Generates new rope notes if far enough away
            if ((transform.position - ropeEnd.transform.position).sqrMagnitude > sqrDistanceBeforeNewNode && ropeRenderer.ropeNodes.Count <= maxNodes)
            {
                CreateNewNode();

            }
        }

    }

    private void CreateNewNode()
    {
        CreateNewNode(transform.position);
    }

    private void CreateNewNode(Vector3 position)
    {
        GameObject newRopeNode = Instantiate(ropeSegmentPrefab, position, Quaternion.identity);
        newRopeNode.GetComponent<Joint>().connectedBody = ropeEnd.GetComponent<Rigidbody>();
        ropeRenderer.ropeNodes.Remove(this.transform);
        ropeRenderer.ropeNodes.Add(newRopeNode.transform);
        ropeRenderer.ropeNodes.Add(this.transform);
        joint.connectedBody = newRopeNode.GetComponent<Rigidbody>();
        ropeEnd = newRopeNode;
    }

    private GameObject GetSecondLastNode()
    {
        if (ropeRenderer.ropeNodes.Count > 2)
        {
            return ropeRenderer.ropeNodes[ropeRenderer.ropeNodes.Count - 3].gameObject;
        }

        return null;

    }

    private GameObject GetLastNode()
    {
        if (ropeRenderer.ropeNodes.Count > 1)
        {
            return ropeRenderer.ropeNodes[ropeRenderer.ropeNodes.Count - 2].gameObject;
        }

        return null;
    }

    private void ReelInRope()
    {
        Debug.Log("reeling");
        // Pulls in last node
        ropeEnd.GetComponent<Rigidbody>().AddForce((transform.position - ropeEnd.transform.position).normalized * reelForce, ForceMode.Acceleration);

        //Deletes last node if close enough
        if ((transform.position - ropeEnd.transform.position).sqrMagnitude < sqrNodeKillDistance && ropeRenderer.ropeNodes.Count > 3)
        {
            ropeRenderer.ropeNodes.Remove(ropeEnd.transform);
            Destroy(ropeEnd);
            ropeEnd = GetLastNode();
            ropeEnd.GetComponent<Joint>().connectedBody = GetSecondLastNode().GetComponent<Rigidbody>();
            joint.connectedBody = ropeEnd.GetComponent<Rigidbody>();
        }
    }

    private void CreateNewRope()
    {
        // set up intial state
        ropeRenderer.InitializeNodeList();
        ropeEnd = ropeRenderer.gameObject;
        ropeRenderer.ropeNodes.Add(transform);
        joint.connectedBody = ropeEnd.GetComponent<Rigidbody>();


        // Loops creating new nodes evenly until last node is close enough to end
        float sqrDistance = (transform.position - ropeRenderer.transform.position).sqrMagnitude;
        while (sqrDistance > sqrDistanceBeforeNewNode)
        {
            CreateNewNode(ropeEnd.transform.position + ((transform.position - ropeEnd.transform.position).normalized * distanceBeforeNewNode));
            sqrDistance = (transform.position - ropeEnd.transform.position).magnitude;
        }

    }
}
