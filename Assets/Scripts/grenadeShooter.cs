using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class grenadeShooter : MonoBehaviour
{
    public GameObject prefab;

    public Interactable interactable;

    public SteamVR_ActionSet actionSet;
    public SteamVR_Action_Boolean shootAction;

    public float fireVelocity;
    public float randomAngular;

    public new PlaySound audio;

    private void Start()
    {
        if (interactable == null)
            interactable = this.GetComponentInParent<Interactable>();

        interactable.onAttachedToHand += Interactable_onAttachedToHand;
        interactable.onDetachedFromHand += Interactable_onDetachedFromHand;
    }

    private void OnDestroy()
    {
        if (interactable == null)
        {
            interactable.onAttachedToHand -= Interactable_onAttachedToHand;
            interactable.onDetachedFromHand -= Interactable_onDetachedFromHand;
        }
    }

    private void Interactable_onDetachedFromHand(Hand hand)
    {
        if (hand.otherHand.currentAttachedObject != null && hand.otherHand.currentAttachedObject.GetComponent<grenadeShooter>() != null)
            return;

        actionSet.Deactivate();
    }

    private void Interactable_onAttachedToHand(Hand hand)
    {
        actionSet.ActivatePrimary();
    }

    private void Update()
    {
        if (interactable.attachedToHand)
        {
            if (shootAction.GetStateDown(interactable.attachedToHand.handType))
            {
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        audio.Play();
        Rigidbody newG = Instantiate(prefab, transform.position, transform.rotation).GetComponent<Rigidbody>();
        newG.velocity = transform.forward * fireVelocity;
        newG.angularVelocity = new Vector3(Random.Range(-randomAngular, randomAngular), Random.Range(-randomAngular, randomAngular), Random.Range(-randomAngular, randomAngular));
    }
}