using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public float speed = 1;

    public Vector3 dir = Vector3.forward;

    public bool global = false;

    public float force = 2;


    void OnTriggerStay(Collider collider)
    {
        Rigidbody rigidbody = collider.attachedRigidbody;
        if (rigidbody != null)
        {
            Vector3 velocity = transform.TransformDirection(dir);
            if (global)
            {
                velocity = dir;
            }
            velocity *= speed;
            rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, velocity, Time.deltaTime * force);
        }
    }
}