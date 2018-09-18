using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour
{
    public Vector3 axis;
    public float speed;
    
    public bool local = true;


    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.localPosition;
    }
    
    private void Update()
    {
        if (local)
        {
            transform.RotateAround(transform.position, transform.TransformDirection(axis), speed * Time.deltaTime);
        }
        else
        {
            transform.RotateAround(transform.position, axis, speed * Time.deltaTime);
        }

        transform.localPosition = initialPosition;
    }
}