using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class resetDropped : MonoBehaviour
{
    public float distance;

    public float height;

    public Transform origin;

    public ResetPortal resetter;


    private Interactable interactable;


    private void Start()
    {
        interactable = GetComponent<Interactable>();
    }
    
    private void Update()
    {
        if (!interactable.attachedToHand && !resetter.resetting)
        {
            if ((transform.position - origin.position).sqrMagnitude > distance * distance)
            {
                resetter.ResetNow();
            }

            if ((transform.position.y < origin.position.y + height))
            {
                resetter.ResetNow();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin.position, distance);
        Gizmos.DrawWireCube(origin.position + height * Vector3.up, new Vector3(10, 0, 10));
    }
}