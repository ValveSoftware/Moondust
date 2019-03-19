using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class grenadeShooter : MonoBehaviour
{
    public GameObject prefab;

    public Interactable interactable;
    
    public SteamVR_Action_Boolean shootAction;

    public float fireVelocity;
    public float randomAngular;

    public new PlaySound audio;

    private void Start()
    {
        if (interactable == null)
            interactable = this.GetComponentInParent<Interactable>();
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