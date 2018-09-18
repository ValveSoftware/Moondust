using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assembler;

public class SatDish : MonoBehaviour
{
    public Transform dish;
    public ParticleSystem ps;
    public AudioSource au;

    public float maxAngle;

    private bool animating;
    private AssemblerComponent comp;
    

    private void Start()
    {
        animating = false;
        comp = GetComponent<AssemblerComponent>();
    }
    
    private void Update()
    {
        if (!animating && comp.connected)
        {
            StartCoroutine(DoAnimate());
            animating = true;
        }
    }

    private IEnumerator DoAnimate()
    {
        ps.Play();
        au.Play();

        while (true)
        {
            Vector3 rot = Vector3.forward;
            rot = Quaternion.Euler(Random.Range(-maxAngle, maxAngle), 0, 0) * rot;
            rot = Quaternion.Euler(0, 0, Random.Range(0, 360)) * rot;

            while (Vector3.Angle(rot, transform.InverseTransformDirection(dish.forward)) > 2)
            {
                dish.localRotation = Quaternion.Slerp(dish.localRotation, Quaternion.LookRotation(rot), Time.deltaTime * 2);
                yield return null;
            }

            ps.Play();
            au.Play();
            yield return new WaitForSeconds(Random.Range(4, 15));
        }
    }
}