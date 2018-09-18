using System.Collections;
using UnityEngine;

public class copyPosition : MonoBehaviour
{
    public Transform[] targets;
    Transform target;
    public Vector3 offset;
    public float positionSmooth;
    public bool rotation;
    public float rotationSmooth;
    public bool local;
    int i;
    // Use this for initialization
    void Start()
    {

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            i += 1;
            i = Mathf.RoundToInt(Mathf.Repeat(i, targets.Length));
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        target = targets[i];
        if (local)
        {
            if (positionSmooth != 0)
            {
                transform.position = Vector3.Lerp(transform.position, target.position + (target.rotation * (offset)), Time.unscaledDeltaTime * positionSmooth);
            }
            else
            {
                transform.position = target.position + (target.rotation * (offset));
            }

        }
        else
        {
            if (positionSmooth != 0)
            {
                transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.unscaledDeltaTime * positionSmooth);
            }
            else
            {
                transform.position = target.position + offset;
            }

        }
        //print (target.right);

        if (rotation)
        {
            Quaternion rot = target.rotation;
            if (rotationSmooth != 0)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.unscaledDeltaTime * rotationSmooth);
            }
            else
            {
                transform.rotation = target.rotation;

            }
        }


    }

    public void Snap()
    {
        FixedUpdate();
        if (local)
        {
            transform.position = target.position + (target.rotation * (offset));
        }
        else
        {
            transform.position = target.position + offset;
        }

        if (rotation)
        {
            Quaternion rot = target.rotation;
            transform.rotation = target.rotation;
        }
    }
}
