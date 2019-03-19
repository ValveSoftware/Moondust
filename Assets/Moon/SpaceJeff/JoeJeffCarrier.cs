using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoeJeffCarrier : MonoBehaviour
{

    public Transform carryAnchor;

    [HideInInspector]
    public JoeJeffCarriable carrying;

    public bool canPickup = true;

    public GameObject holdHighlight;

    public AudioSource audio;

    public AudioClip au_pickup;
    public AudioClip au_throw;

    private void OnTriggerEnter(Collider other)
    {
        if (canPickup == false) return;
        //if we detect a carriable, 
        JoeJeffCarriable carry = other.GetComponent<JoeJeffCarriable>();
        if (carry != null)
        {
            if (carry.carried == false)
            {
                Pickup(carry);
            }
        }
    }

    private void Update()
    {
        if (holdHighlight != null)
        {
            holdHighlight.SetActive(carrying!=null);
        }
    }

    private void FixedUpdate()
    {
        if (carrying)
        {
            UpdateCarrying();
        }
    }

    public void Pickup(JoeJeffCarriable carry)
    {
        audio.PlayOneShot(au_pickup);
        carrying = carry;
        carrying.Pickup(this);
        carrying.transform.position = carrying.GetPositionTarget(carryAnchor);
        carrying.transform.rotation = carrying.GetRotationTarget(carryAnchor);
    }

    void UpdateCarrying()
    {
        carrying.rigidbody.velocity = Vector3.zero;
        carrying.rigidbody.MovePosition(carrying.GetPositionTarget(carryAnchor));
        carrying.rigidbody.angularVelocity = Vector3.zero;
        carrying.rigidbody.MoveRotation(carrying.GetRotationTarget(carryAnchor));
    }

    public void Throw(Vector3 vec)
    {
        audio.PlayOneShot(au_throw);

        //move up a bit so it doesn't collide with head
        carrying.rigidbody.MovePosition(carrying.rigidbody.position + Vector3.up * 0.1f);
        carrying.rigidbody.velocity = vec;
        Drop();
    }

    public void Drop()
    {
        if (carrying != null)
        {
            carrying.Drop();
            carrying = null;
        }
    }

    private void OnDestroy()
    {
        Drop();
    }
}
