using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thrown : MonoBehaviour
{
    public new Rigidbody rigidbody;
    public bool processed { get; private set; }

    private void Awake()
    {
        rigidbody = this.GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        processed = true;
    }
}
