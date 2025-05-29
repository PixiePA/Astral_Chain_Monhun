using UnityEngine;

[RequireComponent(typeof(Joint))]
public class ropeGenerator : MonoBehaviour
{
    public GameObject ropeEnd;
    public RopeRenderer ropeController;
    public float distanceBeforeNewNode = 2f;
    public float reelForce = 50f;
    public int maxNodes = 10;
    [SerializeField] private GameObject ropeSegmentPrefab;
    [SerializeField] private Joint joint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        if (!joint)
        {
            joint = GetComponent<Joint>();
        }
    }

    void Start()
    {
        ropeController.ropeNodes.Add(transform);
        joint.connectedBody = ropeEnd.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Shortens rope if next node can be easily reached
        if ((transform.position - GetSecondLastNode().transform.position).magnitude < distanceBeforeNewNode && ropeController.ropeNodes.Count > 2)
        {
            ReelInRope();
        }

        //Generates new rope notes if far enough away
        if ((transform.position - ropeEnd.transform.position).magnitude > distanceBeforeNewNode && ropeController.ropeNodes.Count <= maxNodes)
        {
            CreateNewNode();

        }

    }

    private void CreateNewNode()
    {
        GameObject newRopeNode = Instantiate(ropeSegmentPrefab, transform.position, Quaternion.identity);
        newRopeNode.GetComponent<Joint>().connectedBody = ropeEnd.GetComponent<Rigidbody>();
        ropeController.ropeNodes.Remove(this.transform);
        ropeController.ropeNodes.Add(newRopeNode.transform);
        ropeController.ropeNodes.Add(this.transform);
        joint.connectedBody = newRopeNode.GetComponent<Rigidbody>();
        ropeEnd = newRopeNode;
    }

    private GameObject GetSecondLastNode()
    {
        return ropeController.ropeNodes[ropeController.ropeNodes.Count - 2].gameObject;

    }

    private void ReelInRope()
    {
        // Pulls in last node
        ropeEnd.GetComponent<Rigidbody>().AddForce((transform.position - ropeEnd.transform.position).normalized * reelForce, ForceMode.Acceleration);


        //Deletes last node if close enough
        if ((transform.position - ropeEnd.transform.position).magnitude < 0.3f)
        {
            ropeController.ropeNodes.Remove(ropeEnd.transform);
            Destroy(ropeEnd);
            ropeEnd = GetSecondLastNode();
            joint.connectedBody = ropeEnd.GetComponent<Rigidbody>();
        }
    }
}
