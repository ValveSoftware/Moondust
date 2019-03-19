using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoeJeffCarriable : MonoBehaviour
{
    [SerializeField]
    Vector3 posOffset;
    [SerializeField]
    Vector3 rotOffset;

    public bool carried
    {
        get { return carrier != null; }
    }

    [HideInInspector]
    public Rigidbody rigidbody;

    [HideInInspector]
    public JoeJeffCarrier carrier = null;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public Vector3 GetPositionTarget(Transform anchor)
    {
        return anchor.TransformPoint(posOffset);
    }
    public Quaternion GetRotationTarget(Transform anchor)
    {
        return anchor.rotation * Quaternion.Euler(rotOffset);
    }

    public void Pickup(JoeJeffCarrier c)
    {
        if (carried) carrier.Drop(); // drop from old carrier
        carrier = c;
    }

    public void Drop()
    {
        carrier = null;
    }
}
