using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class grenade : MonoBehaviour
{
    public bool impact;

    public float timeTimer = 3;

    public bool init;

    public GameObject explo;
    public AudioSource au_blip;
    public GameObject blip;

    public SteamVR_ActionSet actionSet;
    public SteamVR_Action_Boolean a_prime;

    private bool exploded = false;
    private float age;
    private float timer;
    private Interactable interactable;

    private float minVelocityToPrimeWhileThrowing = 2;

    private void Start()
    {
        interactable = GetComponent<Interactable>();
        if (interactable != null)
        {
            interactable.onAttachedToHand += Interactable_onAttachedToHand;
            interactable.onDetachedFromHand += Interactable_onDetachedFromHand;
        }
        init = impact;
    }

    private void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.onAttachedToHand -= Interactable_onAttachedToHand;
            interactable.onDetachedFromHand -= Interactable_onDetachedFromHand;
        }
    }

    private void Interactable_onDetachedFromHand(Hand hand)
    {
        if (hand.otherHand.currentAttachedObject != null && hand.otherHand.currentAttachedObject.GetComponent<grenade>() != null)
            return;

        actionSet.Deactivate();
    }

    private void Interactable_onAttachedToHand(Hand hand)
    {
        actionSet.ActivatePrimary();
    }

    public void Prime()
    {
        init = true;

        if (interactable != null && interactable.attachedToHand != null)
        {
            interactable.attachedToHand.TriggerHapticPulse(500);

            StartCoroutine(DoHapticBuzz(20, 2000));
        }
    }

    public void Throw()
    {
        if (GetComponent<Rigidbody>().velocity.magnitude > minVelocityToPrimeWhileThrowing)
        {
            //Prime();
        }
    }

    public void CancelInit()
    {
        init = false;
        timer = 0;
        blip.SetActive(false);
    }
    
    private void Update()
    {
        if (interactable != null)
        {
            if (interactable.attachedToHand)
            {
                if (a_prime.GetStateDown(interactable.attachedToHand.handType) && !init)
                {
                    Prime();
                }
            }
        }

        age += Time.deltaTime;
        if (init)
        {
            timer += Time.deltaTime;
            if (timer > timeTimer)
                Explode();
        }
        else
        {
            timer = 0;
        }

        if (blip)
        {
            if (!impact)
            {
                au_blip.enabled = init;
            }
            float pitch = (timer / timeTimer) + 1;
            pitch = Mathf.Round(Mathf.Pow(pitch, 2));
            au_blip.pitch = pitch;
            blip.SetActive(au_blip.time < 0.15f && au_blip.enabled);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (impact)
        {
            collision.gameObject.SendMessage("GrenadeHit", SendMessageOptions.DontRequireReceiver);
            Explode();
        }
    }

    public void Explode()
    {
        if (exploded == false)
        {
            exploded = true;
            Instantiate(explo, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private IEnumerator DoHapticBuzz(int length, int amplitude)
    {
        for (int hapticLength = 0; hapticLength < length; hapticLength++)
        {
            if (init == true && interactable != null && interactable.attachedToHand != null)
                interactable.attachedToHand.TriggerHapticPulse((ushort)amplitude);

            yield return null;
        }
    }

    public void Exploded() // another grenade exploded near me
    {
        if (age > 5)
        {
            Prime();
            timer = Random.Range(timeTimer * 0.90f, timeTimer * 0.99f);
        }
    }
}