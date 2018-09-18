using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
public class KnucklesSFX : MonoBehaviour
{
    public AudioClip au_pickup;
    public AudioClip au_place;

    private AudioSource au;

    private Interactable interactable;
    
    private void Start()
    {
        au = GetComponent<AudioSource>();
        interactable = GetComponentInParent<Interactable>();

        interactable.onAttachedToHand += Interactable_onAttachedToHand;
        interactable.onDetachedFromHand += Interactable_onDetachedFromHand;
    }

    private void Interactable_onDetachedFromHand(Hand hand)
    {
        au.PlayOneShot(au_pickup);
    }

    private void Interactable_onAttachedToHand(Hand hand)
    {
        au.PlayOneShot(au_pickup);
    }
}