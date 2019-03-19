using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceCheckpoint : MonoBehaviour
{
    [SerializeField]
    private Material mat_active;

    [SerializeField]
    private Material mat_next;

    [SerializeField]
    private Material mat_inactive;

    public Renderer renderer;

    public enum CheckpointStates
    {
        Active,     // highlighted, next checkpoint
        Next,       // is next checkpoint        
        Inactive,   // not highlighted, background
        Disabled    // hidden
    }
    public CheckpointStates state;

    public float radius;

    float scale
    {
        get
        {
            return transform.lossyScale.z;
        }
    }

    private void Start()
    {
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius * scale);
    }

    public bool HasPassedCheckpoint(Vector3 oldPos, Vector3 newPos)
    {
        Plane plane = new Plane(transform.forward, transform.position);
        // if the object has passed the checkpoint plane in the last frame
        if(plane.SameSide(oldPos, newPos) == false)
        {

            // check if it passed the plane inside the checkpoint
            Vector3 intersection = GetLinePlaneIntersection(oldPos, newPos, plane);
            if (CheckDistance(intersection, transform.position, radius * scale))
            {
                return true;
            }
        }
        // otherwise,
        return false;
    }

    Vector3 GetLinePlaneIntersection(Vector3 a, Vector3 b, Plane plane)
    {
        float dist;
        if(plane.Raycast(new Ray(a, b-a), out dist))
        {
            return a + (b - a).normalized * dist;
        }
        else
        {
            return Vector3.zero;
        }
    }

    bool CheckDistance(Vector3 a, Vector3 b, float dist)
    {
        return (a - b).sqrMagnitude < dist * dist;
    }

    private void Update()
    {
        renderer.material = state == CheckpointStates.Active ? mat_active : state == CheckpointStates.Next ? mat_next : mat_inactive;
        renderer.enabled = state != CheckpointStates.Disabled;
    }
}
