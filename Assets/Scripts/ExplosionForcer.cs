using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionForcer : MonoBehaviour
{
    public float power;
    public float radius;

    public bool boostAudio;
    
    private void Start()
    {
        if (boostAudio)
        {
            GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip, 3);
        }
        Explode();
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        List<Rigidbody> rigidbodyList = new List<Rigidbody>();
        foreach (Collider collider in colliders)
        {
            Rigidbody attachedRigidbody = collider.attachedRigidbody;

            if (attachedRigidbody != null)
            {
                if (rigidbodyList.Contains(attachedRigidbody) == false)
                {
                    rigidbodyList.Add(attachedRigidbody);
                }
            }
        }

        foreach (Rigidbody rigidbody in rigidbodyList)
        {
            rigidbody.AddExplosionForce(power, transform.position, radius, 0.6f);
            rigidbody.SendMessage("Exploded", SendMessageOptions.DontRequireReceiver);
        }
    }
}