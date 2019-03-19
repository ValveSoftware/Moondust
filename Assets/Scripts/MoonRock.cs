using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class MoonRock : MonoBehaviour
{
    public GameObject BrokenPrefab;

    [HideInInspector]
    public Interactable interactable;

    public Color glowColor;

    public float shakingAmount = 1;

    [System.NonSerialized]
    public bool crushable = true;

    public SteamVR_Action_Boolean actionCrush;
    public SteamVR_Action_Single actionSqueeze;

    private new Rigidbody rigidbody;
    private Renderer[] renderers;


    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        interactable = GetComponent<Interactable>();
        rigidbody = interactable.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (interactable.attachedToHand == null || crushable == false)
            return;

        SteamVR_Input_Sources hand = interactable.attachedToHand.handType;

        float squeeze = actionSqueeze.GetAxis(hand);
        foreach (Renderer rend in renderers)
        {
            rend.material.SetColor("_EmissionColor", (glowColor * Mathf.Pow(squeeze * 2.5f, 2)));
        }

        bool crush = actionCrush.GetStateDown(hand);

        if (crush)
        {
            Instantiate(BrokenPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else
        {
            float stress = Mathf.Pow(squeeze, 2);

            if (squeeze > 0.3f)
                interactable.attachedToHand.TriggerHapticPulse((ushort)Mathf.Lerp(200, 1000, squeeze * 2));

            Vector3 shake = new Vector3(Random.Range(-shakingAmount, shakingAmount), Random.Range(-shakingAmount, shakingAmount), Random.Range(-shakingAmount, shakingAmount));
            shake *= stress;
            rigidbody.AddForce(shake * 20000, ForceMode.Impulse);
        }
    }
}