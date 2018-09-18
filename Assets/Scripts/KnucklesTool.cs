using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class KnucklesTool : MonoBehaviour
{
    public Transform[] antiFlip;
    public WhichHand defaultHand = WhichHand.Right;

    private Vector3 scale;
    private Interactable interactable;


    private void Start()
    {
        scale = transform.localScale;
        interactable = GetComponent<Interactable>();
    }

    private void Update()
    {
        if (interactable.attachedToHand)
        {
            Vector3 scl = scale;
            if ((interactable.attachedToHand.handType == SteamVR_Input_Sources.RightHand && defaultHand == WhichHand.Right) || (interactable.attachedToHand.handType == SteamVR_Input_Sources.LeftHand && defaultHand == WhichHand.Left))
            {
                scl.x *= 1;
                foreach (Transform t in antiFlip)
                {
                    t.localScale = new Vector3(1, 1, 1);
                }
            }
            else
            {
                scl.x *= -1;
                foreach (Transform t in antiFlip)
                {
                    t.localScale = new Vector3(-1, 1, 1);
                }
            }
            transform.localScale = scl;
        }
    }

    public enum WhichHand
    {
        Left,
        Right
    }
}

