using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;

public class PocketPortal : MonoBehaviour
{
    public string switchTo;

    public float activeRadius;


    private Interactable interactable;

    private void Start()
    {
        interactable = GetComponent<Interactable>();

        if (switchTo == SceneManager.GetActiveScene().name)
        {
            transform.localScale *= 0.6f;
        }
    }

    private void Update()
    {
        bool used = false;
        if (interactable) used = interactable.attachedToHand;
        if (used)
        {
            float dist = (transform.position - Camera.main.transform.position).sqrMagnitude;
            float rad = activeRadius * transform.lossyScale.x;
            if (dist < rad * rad)
            {
                Switch();
            }
        }
    }

    private void Switch()
    {
        SceneManager.LoadScene(switchTo);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, activeRadius * transform.lossyScale.x);
    }
}
