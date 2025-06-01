using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
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
    [SerializeField] private GameObject bindingPointPrefab;
    [SerializeField] private Joint joint;
    private List<CapsuleCollider> boundColliders;
    private List<CapsuleCollider> collidersOnCooldown = new List<CapsuleCollider>();
    public bool isBinding;
    public bool unBindCooldown;
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
            List<CapsuleCollider> hitColliders = new List<CapsuleCollider>();

            for (int i = 0; i < ropeRenderer.ropeNodes.Count - 1; i++)
            {
                if (Physics.Raycast(ropeRenderer.ropeNodes[i].position, (ropeRenderer.ropeNodes[i + 1].position - ropeRenderer.ropeNodes[i].position).normalized, out RaycastHit hitinfo, (ropeRenderer.ropeNodes[i + 1].position - ropeRenderer.ropeNodes[i].position).magnitude, 64))
                {
                    if (hitinfo.collider.GetType() == typeof(CapsuleCollider) && !boundColliders.Contains((CapsuleCollider)hitinfo.collider) )
                    {
                        hitColliders.Add((CapsuleCollider)hitinfo.collider);

                        if (!collidersOnCooldown.Contains((CapsuleCollider)hitinfo.collider))
                        {
                            CreateBindNode((CapsuleCollider)hitinfo.collider, i);
                            break;
                        }   
                    }

                    
                    
                }

            }

            List<CapsuleCollider> tempCollidersOnCooldown = new List<CapsuleCollider>(collidersOnCooldown);

            foreach (CapsuleCollider collider in tempCollidersOnCooldown)
            {
                if (!hitColliders.Contains(collider))
                {
                    collidersOnCooldown.Remove(collider);
                }
                
            }
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
            if ((transform.position - ropeEnd.transform.position).sqrMagnitude >= sqrDistanceBeforeNewNode && ropeRenderer.ropeNodes.Count <= maxNodes)
            {
                CreateNewNode();

            }


            //Check for binding
            bool somethingOnChain = false;

            for (int i = 0; i < ropeRenderer.ropeNodes.Count - 1; i++)
            {
                if (Physics.Raycast(ropeRenderer.ropeNodes[i].position, (ropeRenderer.ropeNodes[i + 1].position - ropeRenderer.ropeNodes[i].position).normalized, out RaycastHit hitinfo, (ropeRenderer.ropeNodes[i + 1].position - ropeRenderer.ropeNodes[i].position).magnitude, 64))
                {
                    if (unBindCooldown == false)
                    {
                        if (hitinfo.collider.GetType() == typeof(CapsuleCollider))
                        {
                            StartBind((CapsuleCollider)hitinfo.collider);
                        }
                    }

                    somethingOnChain = true;

                    break;
                }

            }

            if (unBindCooldown == true && somethingOnChain == false)
            {
                unBindCooldown = false;
            }
        }

    }

    public void UnbindNode(GameObject node, bool isOpposite, CapsuleCollider boundCollider)
    {
        int indexOfNode = ropeRenderer.ropeNodes.IndexOf(node.transform);
        if (indexOfNode - 1 > 0)
        {
            ropeRenderer.ropeNodes[indexOfNode - 1].SendMessage("OnSetNextNode", ropeRenderer.ropeNodes[indexOfNode + 1]);
        }
        if (indexOfNode + 1 <  ropeRenderer.ropeNodes.Count - 1)
        {
            ropeRenderer.ropeNodes[indexOfNode + 1].SendMessage("OnSetPrevNode", ropeRenderer.ropeNodes[indexOfNode - 1]);
        }
        ropeRenderer.ropeNodes.Remove(node.transform);
        boundColliders.Remove(boundCollider);
        collidersOnCooldown.Add(boundCollider);
        Destroy(node);
        if (ropeRenderer.ropeNodes.Count < 3)
        {
            CreateNewRope();
            isBinding = false;
            unBindCooldown = true;

            if (isOpposite == false)
            {
                Debug.Log("Chainbind");
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

    private void StartBind(CapsuleCollider boundCollider)
    {
        for (int i = 1; ropeRenderer.ropeNodes[i] != transform; )
        {
            Destroy(ropeRenderer.ropeNodes[i].gameObject);
            ropeRenderer.ropeNodes.RemoveAt(i);
        }

        ropeRenderer.InitializeNodeList(transform);
        boundColliders = new List<CapsuleCollider>();
        CreateBindNode(boundCollider, 0);
        isBinding = true;
    }

    private void CreateBindNode(CapsuleCollider boundCollider, int prevNodeIndex)
    {
        GameObject newBindingNode = Instantiate(bindingPointPrefab);
        newBindingNode.SendMessage("OnSetCollider", boundCollider);
        newBindingNode.SendMessage("OnSetPrevNode", ropeRenderer.ropeNodes[prevNodeIndex]);
        newBindingNode.SendMessage("OnSetNextNode", ropeRenderer.ropeNodes[prevNodeIndex + 1]);
        newBindingNode.SendMessage("OnSetRopeGenerator", this);
        if (prevNodeIndex > 0)
        {
            ropeRenderer.ropeNodes[prevNodeIndex].SendMessage("OnSetNextNode", newBindingNode.transform);
        }
        ropeRenderer.ropeNodes.Insert(prevNodeIndex + 1, newBindingNode.transform);
        boundColliders.Add(boundCollider);
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
        ropeRenderer.InitializeNodeList(transform);
        ropeEnd = ropeRenderer.gameObject;
        joint.connectedBody = ropeEnd.GetComponent<Rigidbody>();


        // Loops creating new nodes evenly until last node is close enough to end
        float sqrDistance = (transform.position - ropeRenderer.transform.position).sqrMagnitude;
        while (sqrDistance > sqrDistanceBeforeNewNode && ropeRenderer.ropeNodes.Count <= maxNodes)
        {
            CreateNewNode(ropeEnd.transform.position + ((transform.position - ropeEnd.transform.position).normalized * distanceBeforeNewNode));
            sqrDistance = (transform.position - ropeEnd.transform.position).magnitude;
        }

    }
}
