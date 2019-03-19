using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
public class StationInteractionSFX : MonoBehaviour
{
    public AudioClip au_pickup;
    public AudioClip au_place;

    private AudioSource au;

    private Interactable interactable;

    private Assembler.AssemblerComponent assembler;

    public bool useHaptics = true;
    public SteamVR_Action_Vibration action_haptics;
    
    private void Start()
    {
        au = GetComponent<AudioSource>();
        interactable = GetComponentInParent<Interactable>();
        assembler = GetComponentInParent<Assembler.AssemblerComponent>();

        interactable.onAttachedToHand += Interactable_onAttachedToHand;
        interactable.onDetachedFromHand += Interactable_onDetachedFromHand;
        if(assembler != null)
            assembler.onAttached += Assembler_onAttached;
    }

    private void Interactable_onDetachedFromHand(Hand hand)
    {
    }

    private void Interactable_onAttachedToHand(Hand hand)
    {

        au.PlayOneShot(au_place);
    }

    private void Assembler_onAttached(Hand hand)
    {
        au.PlayOneShot(au_place);
        if(useHaptics)
        action_haptics.Execute(0, 0.1f, 80, 0.8f, hand.handType);

    }
}