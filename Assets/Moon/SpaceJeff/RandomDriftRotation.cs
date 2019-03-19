using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDriftRotation : MonoBehaviour
{
    public float MaxSpeed = 270;

    Vector3 rotation;

    public Space rotationSpace;

    private void Start()
    {
        rotation = new Vector3(Random.Range(-MaxSpeed, MaxSpeed), Random.Range(-MaxSpeed, MaxSpeed), Random.Range(-MaxSpeed, MaxSpeed));
    }

    private void Update()
    {
        transform.Rotate(rotation * Time.deltaTime, rotationSpace);
    }
}
