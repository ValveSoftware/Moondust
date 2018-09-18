using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHead : MonoBehaviour
{
    public float maxEyeRot;

    public float multiplier;

    public float smoothing = 10;

    public float bigMoveThreshold;

    public Transform eye;


    private Quaternion oldRot;

    private Vector3 dRot;

    private float rSpd;

    private Vector3 lookDir;

    private float lookTimer;
    private float rlt;

    private Quaternion grel;


    private void Update()
    {
        Quaternion relative = Quaternion.Inverse(Quaternion.Inverse(transform.rotation) * oldRot);
        grel = Quaternion.Inverse(transform.rotation) * oldRot;

        dRot = relative.eulerAngles;
        dRot = new Vector3(AngleSnap(dRot.x), AngleSnap(dRot.y), 0);

        rSpd = Mathf.Lerp(rSpd, (dRot / Time.deltaTime).magnitude, Time.deltaTime * 2);

        dRot = dRot / Time.deltaTime * multiplier;

        Vector3 r = new Vector3(dRot.x, dRot.y, 0);
        r = Vector3.ClampMagnitude(r, maxEyeRot);
        
        if (IsNaN(r) == false)
        {
            Quaternion rQ = Quaternion.Euler(r);
            if (IsNaN(rQ) == false)
            {
                eye.localRotation = Quaternion.Slerp(eye.localRotation, rQ, Time.deltaTime * smoothing);
            }
        }

        oldRot = transform.rotation;
    }

    private Vector3 FindLookAngle()
    {
        float d = 0.06f;
        Vector3 dir = transform.forward + new Vector3(Random.Range(-d, d), Random.Range(-d, d), Random.Range(-d, d));
        return Vector3.Lerp(dir, Quaternion.SlerpUnclamped(Quaternion.identity, grel, 1) * dir, 0.8f);
    }

    private float AngleSnap(float angle)
    {
        if (angle > 180)
        {
            angle -= 360;
        }
        return angle;
    }

    private bool IsNaN(Quaternion q)
    {
        return float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z) || float.IsNaN(q.w);
    }

    private bool IsNaN(Vector3 v)
    {
        return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z);
    }
}