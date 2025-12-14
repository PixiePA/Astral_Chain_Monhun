using System.Runtime.CompilerServices;
using UnityEngine;

public class RopeBindNode : MonoBehaviour
{
    public CapsuleCollider boundCollider;
    public Transform prevNode;
    public Transform nextNode;
    public ropeGenerator ropeGenerator;
    public bool canUnbind;
    public bool hasBound;
    public Vector3 BoundColliderCentre
    {
        get
        {
            return boundCollider.transform.position + boundCollider.center;
        }
    }

    public Vector3 prevNodeToColliderDirection
    {
        get
        {
            return (boundCollider.ClosestPoint(prevNode.position) - prevNode.position).normalized;
        }
    }

    public Vector3 nextNodeToColliderDirection
    {
        get
        {
            return (boundCollider.ClosestPoint(nextNode.position) - nextNode.position).normalized;
        }
    }

    public Vector3 DesiredPosition
    {
        get
        {
            Vector3 averageDirection = ((prevNodeToColliderDirection + nextNodeToColliderDirection)).normalized;

            //Determine height or radius is higher for accurate raycast onto surface
            float rayCastStartProjectionDistance = boundCollider.height + 1;
            if (boundCollider.radius > boundCollider.height)
            {
                rayCastStartProjectionDistance = boundCollider.radius + 1;
            }

            //Find 2 possible points to act as connection point
            boundCollider.Raycast(new Ray(BoundColliderCentre + averageDirection * rayCastStartProjectionDistance, -averageDirection), out RaycastHit point1Info, rayCastStartProjectionDistance);
            boundCollider.Raycast(new Ray(BoundColliderCentre - averageDirection * rayCastStartProjectionDistance, averageDirection), out RaycastHit point2Info, rayCastStartProjectionDistance);


            //Use the closer one
            if (Mathf.Abs((transform.position - point1Info.point).magnitude) < Mathf.Abs((transform.position - point2Info.point).magnitude))
            {
                return point1Info.point;
            }

            return point2Info.point;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = DesiredPosition;

        Vector3 directionToCentre = (BoundColliderCentre - transform.position).normalized;
        Vector3 directionToPrevNode = (prevNode.position - transform.position).normalized;
        Vector3 directionToNextNode = (nextNode.position - transform.position).normalized;
        Vector3 averageDirectionToNodes = (directionToPrevNode + directionToNextNode).normalized;

        //If the average of directions of directions to nodes points away, detach
        if (Vector3.Dot(averageDirectionToNodes, directionToCentre) < 0)
        {
           ropeGenerator.UnbindNode(this.gameObject, true, boundCollider);
        }
        else if (Vector3.Dot(directionToPrevNode, directionToNextNode) > 0.95f)
        {
           ropeGenerator.UnbindNode(this.gameObject, false, boundCollider);
        }


    }

    public void OnSetCollider(CapsuleCollider coll)
    {
        boundCollider = coll;
    }

    public void OnSetPrevNode(Transform node)
    {
        prevNode = node;
    }
    public void OnSetNextNode(Transform node)
    {
        nextNode = node;
    }

    public void OnSetRopeGenerator(ropeGenerator generator)
    {
        ropeGenerator = generator;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(DesiredPosition, prevNode.position, Color.red);
        Debug.DrawLine (DesiredPosition, nextNode.position, Color.red);
        Gizmos.DrawSphere(BoundColliderCentre, 0.1F);
        Debug.DrawLine(prevNode.position, nextNode.position, Color.magenta);
    }
}
