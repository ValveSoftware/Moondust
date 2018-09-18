using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatTo : MonoBehaviour
{
    public Transform target;

    public float speed;
    
    private void Update()
    {
        if (speed == 0)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * speed);
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * speed);
        }
    }
}