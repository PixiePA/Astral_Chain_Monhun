using UnityEngine;

public class ropeGenerator : MonoBehaviour
{
    public GameObject ropeEnd;
    public RopeController ropeController;
    public float distanceBeforeNewNode = 2f;
    public int maxNodes = 10;
    [SerializeField] private GameObject ropeSegmentPrefab;
    [SerializeField] private SpringJoint joint; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ropeController.ropeNodes.Add(transform);
    }

    // Update is called once per frame
    void Update()
    {
        //Generates new rope notes if far enough away
        if ((transform.position - ropeEnd.transform.position).magnitude > distanceBeforeNewNode && ropeController.ropeNodes.Count <= maxNodes)
        {
            CreateNewNode();

        }
    }

    private void CreateNewNode()
    {
        GameObject newRopeNode = Instantiate(ropeSegmentPrefab, transform.position, Quaternion.identity);
        newRopeNode.GetComponent<SpringJoint>().connectedBody = ropeEnd.GetComponent<Rigidbody>();
        ropeController.ropeNodes.Remove(this.transform);
        ropeController.ropeNodes.Add(newRopeNode.transform);
        ropeController.ropeNodes.Add(this.transform);
        joint.connectedBody = newRopeNode.GetComponent<Rigidbody>();
        ropeEnd = newRopeNode;
    }
}
