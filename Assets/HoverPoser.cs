using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class HoverPoser : MonoBehaviour
{
    Interactable interactable;
    SteamVR_Skeleton_Poser poser;

    private Hand currentHand;

    private void Start()
    {
        interactable = GetComponent<Interactable>();
        poser = GetComponent<SteamVR_Skeleton_Poser>();
    }

    protected virtual void OnHandHoverBegin(Hand hand)
    {
        currentHand = hand;
        hand.skeleton.BlendToPoser(poser);
    }

    //-------------------------------------------------
    protected virtual void OnHandHoverEnd(Hand hand)
    {
        hand.skeleton.BlendToSkeleton();
        currentHand = null;
    }
}
