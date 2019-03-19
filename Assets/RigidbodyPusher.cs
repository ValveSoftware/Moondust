using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyPusher : MonoBehaviour
{
    List<Rigidbody> touching = new List<Rigidbody>();

    public Space forceSpace;
    public ForceMode mode;
    public Vector3 force;

    public bool nosleep;

    private void FixedUpdate()
    {
        for (int i = touching.Count-1; i >= 0; i--)
        {
            if (touching[i] == null) { touching.RemoveAt(i); continue; }
            Vector3 f = forceSpace == Space.Self ? transform.TransformDirection(force.normalized) * force.magnitude : force;
            touching[i].AddForce(f, mode);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            touching.Add(other.attachedRigidbody);
            if (nosleep) other.attachedRigidbody.sleepThreshold = 0;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody != null) if(touching.Contains(other.attachedRigidbody)) touching.Remove(other.attachedRigidbody);
    }
}
