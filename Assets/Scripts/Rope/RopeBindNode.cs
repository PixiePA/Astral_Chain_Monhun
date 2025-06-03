using System.Runtime.CompilerServices;
using UnityEngine;

public class RopeBindNode : MonoBehaviour
{
    public CapsuleCollider boundCollider;
    public Transform prevNode;
    public Transform nextNode;
    public ropeGenerator ropeGenerator;
    public bool canUnbind;
    public Vector3 BoundColliderCentre
    {
        get
        {
            return boundCollider.transform.position + boundCollider.center;
        }
    }

    private Vector3 InterceptPoint
    {
        get
        {
            return prevNode.position + Vector3.Project(BoundColliderCentre - prevNode.position, nextNode.position - prevNode.position);
        }
    }

    public Vector3 DesiredPosition
    {
        get
        {
            return BoundColliderCentre + (BoundColliderCentre - boundCollider.ClosestPoint(InterceptPoint));
        }
    }

    public bool isInsideCollider
    {
        get
        {
            return (boundCollider.ClosestPoint(InterceptPoint) - InterceptPoint).sqrMagnitude == 0;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (canUnbind )
        {
            if (isInsideCollider )
            {
                ropeGenerator.UnbindNode(this.gameObject, Vector3.Dot(prevNode.position - transform.position, nextNode.position-transform.position) < 0, boundCollider);
            }
        }
        else
        {
            canUnbind = !isInsideCollider;
        }

        transform.position = DesiredPosition;
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
        Debug.DrawLine(InterceptPoint, boundCollider.ClosestPoint(InterceptPoint), Color.cyan);
        Gizmos.DrawSphere(BoundColliderCentre, 0.1F);
        Debug.DrawLine(prevNode.position, nextNode.position, Color.magenta);
        Gizmos.DrawWireSphere(boundCollider.ClosestPoint(InterceptPoint), 0.1f);
    }
}
