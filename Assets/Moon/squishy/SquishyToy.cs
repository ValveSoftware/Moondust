using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SquishyToy : MonoBehaviour
{
    public Interactable interactable;
    public SkinnedMeshRenderer rend;

    public bool affectMaterial = true;

    public float againstGrav;

    public SteamVR_ActionSet actionSet;

    public SteamVR_Action_Single gripSqueeze;
    public SteamVR_Action_Single pinchSqueeze;


    private Rigidbody rb;

    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        
        if (interactable == null)
            interactable = GetComponent<Interactable>();
        
        interactable.onAttachedToHand += Interactable_onAttachedToHand;
        interactable.onDetachedFromHand += Interactable_onDetachedFromHand;
    }

    private void Interactable_onDetachedFromHand(Hand hand)
    {
        if (hand.otherHand.currentAttachedObject != null && (hand.otherHand.currentAttachedObject.GetComponent<SquishyToy>() != null || hand.otherHand.currentAttachedObject.GetComponent<turret>() != null))
            return;

        actionSet.Deactivate();
    }

    private void Interactable_onAttachedToHand(Hand hand)
    {
        actionSet.ActivatePrimary();
    }

    private void OnDestroy()
    {
        if (interactable == null)
        {
            interactable.onAttachedToHand -= Interactable_onAttachedToHand;
            interactable.onDetachedFromHand -= Interactable_onDetachedFromHand;
        }
    }

    private void Update()
    {
        float grip = 0;
        float pinch = 0;

        if (interactable.attachedToHand)
        {
            grip = gripSqueeze.GetAxis(interactable.attachedToHand.handType);
            pinch = pinchSqueeze.GetAxis(interactable.attachedToHand.handType);
        }

        rend.SetBlendShapeWeight(0, Mathf.Lerp(rend.GetBlendShapeWeight(0), grip * 150, Time.deltaTime * 10));

        if (rend.sharedMesh.blendShapeCount > 1) // make sure there's a pinch blend shape
            rend.SetBlendShapeWeight(1, Mathf.Lerp(rend.GetBlendShapeWeight(1), pinch * 200, Time.deltaTime * 10));

        if (affectMaterial)
        {
            rend.material.SetFloat("_Deform", Mathf.Pow(grip * 1.5f, 0.5f));
            if (rend.material.HasProperty("_PinchDeform"))
            {
                rend.material.SetFloat("_PinchDeform", Mathf.Pow(pinch * 2.0f, 0.5f));
            }
        }
    }

    private void FixedUpdate()
    {
        rb.AddForce(-Physics.gravity * againstGrav, ForceMode.Acceleration);
    }
}