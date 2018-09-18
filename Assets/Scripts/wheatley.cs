using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wheatley : MonoBehaviour
{
    public AudioSource au;

    public AudioClip[] hellos;

    public float helloDist;

    public float helloAngle;


    private float helloTime;
    private float minTime = 8;


    private void Update()
    {
        Vector3 offset = (transform.position - Camera.main.transform.position);

        if (offset.sqrMagnitude < helloDist * helloDist && Vector3.Angle(offset, Camera.main.transform.forward) < helloAngle && helloTime > minTime)
        {
            helloTime = 0;
            au.PlayOneShot(hellos[Random.Range(0, hellos.Length)]);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, helloDist);
    }
}