using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

public class GravityMachine : MonoBehaviour
{
    public Vector2 Gravs;

    public LinearDrive slider;

    public Text gravText;

    private float grav;

    private float movg;
    private float oldg;

    private float lastN;

    private Interactable interactable;

    private void Start()
    {
        interactable = slider.GetComponent<Interactable>();
    }

    private void Update()
    {
        grav = Mathf.Lerp(Gravs.x, Gravs.y, slider.linearMapping.value);

        Physics.gravity = Vector3.down * grav;

        float n = (Mathf.Round(grav * 10) / 10);

        if (Mathf.Abs(n - oldg) >= 0.1f)
        {
            if (interactable.attachedToHand)
                interactable.attachedToHand.TriggerHapticPulse(900);

            oldg = n;
        }

        if (lastN != n)
        {
            string num = n.ToString();
            if (num.Length == 1)
                num = num + ".0";

            gravText.text = num + " m/s²";

            lastN = n;
        }
    }
}