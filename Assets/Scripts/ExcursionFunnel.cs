using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExcursionFunnel : MonoBehaviour
{
    public Vector2 Falloff;

    public float liftSpeed = 3;
    public float liftForce = 3;

    [HideInInspector]
    public int numRocks;

    private List<MoonRock> rocks;
    private Vector3 FallH;


    private void Start()
    {
        rocks = new List<MoonRock>();
    }

    private void Update()
    {
        FallH.x = transform.position.y + Falloff.x;
        FallH.y = transform.position.y + Falloff.y;

        for (int rockIndex = 0; rockIndex < rocks.Count; rockIndex++)
        {
            MoonRock rock = rocks[rockIndex];

            if (rock == null)
            {
                rocks.Remove(rock);
                rockIndex--;
            }
        }

        numRocks = rocks.Count;
    }

    private void OnTriggerStay(Collider collider)
    {
        Rigidbody attachedRigidbody = collider.attachedRigidbody;

        if (attachedRigidbody != null)
        {
            float h = attachedRigidbody.transform.position.y - transform.position.y;
            float up = Mathf.Clamp01(Mathf.InverseLerp(FallH.y, FallH.x, h));

            Vector3 velocity = (transform.position - attachedRigidbody.transform.position) * 4;
            velocity.y = up * liftSpeed;

            attachedRigidbody.velocity = Vector3.Lerp(attachedRigidbody.velocity, velocity, Time.deltaTime * liftForce);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        Rigidbody attachedRigidbody = collider.attachedRigidbody;

        if (attachedRigidbody != null)
        {
            if (collider.GetComponent<MoonRock>())
            {
                rocks.Add(collider.GetComponent<MoonRock>());
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        Rigidbody attachedRigidbody = collider.attachedRigidbody;

        if (attachedRigidbody != null)
        {
            if (collider.GetComponent<MoonRock>())
            {
                rocks.Remove(collider.GetComponent<MoonRock>());
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position + Vector3.up * Falloff.x, transform.position + Vector3.up * Falloff.y);
    }
}
