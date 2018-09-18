using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnHit : MonoBehaviour
{
    public GameObject explodee;

    public LayerMask hitMask;

    void OnCollisionEnter(Collision collider)
    {
        if (hitMask == (hitMask | (1 << collider.gameObject.layer)))
        {
            Instantiate(explodee, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}