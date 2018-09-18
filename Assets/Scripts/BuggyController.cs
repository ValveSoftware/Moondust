using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class BuggyController : MonoBehaviour
{
    public Transform Joystick;
    public float joystickRot = 20;

    public Transform Trigger;
    public float triggerRot = 20;

    public BuggyBuddy buggy;

    public BuggyReset resetter;

    public Transform buttonA;
    public Transform buttonB;

    public Transform holoBuggyMatcher;

    public float turboTime = 5;

    //ui stuff

    public Canvas ui_Canvas;
    public Image ui_rpm;
    public Image ui_speed;
    public RectTransform ui_steer;

    public float ui_steerangle;

    public Vector2 ui_fillAngles;
    public Image ui_turboBar;

    public SteamVR_ActionSet actionSet;

    public SteamVR_Action_Vector2 actionSteer;
    public SteamVR_Action_Boolean actionTurbo;
    public SteamVR_Action_Single actionThrottle;

    public SteamVR_Action_Boolean actionBrake;
    public SteamVR_Action_Boolean actionReset;


    private Interactable interactable;
    private Quaternion trigSRot;
    private Quaternion joySRot;

    private float turbofill;
    private bool turboing;

    private float usteer;

    private void Start()
    {
        turbofill = 1;
        joySRot = Joystick.localRotation;
        trigSRot = Trigger.localRotation;

        interactable = GetComponent<Interactable>();
        interactable.onAttachedToHand += Interactable_onAttachedToHand;
        interactable.onDetachedFromHand += Interactable_onDetachedFromHand;
        StartCoroutine(DoBuzz());
        buggy.controllerReference = transform;
    }

    private void Interactable_onDetachedFromHand(Hand hand)
    {
        if (hand.otherHand.currentAttachedObject != null && hand.otherHand.currentAttachedObject.GetComponent<BuggyController>() != null)
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
        Vector2 steer = Vector2.zero;
        float throttle = 0;
        float brake = 0;

        bool reset = false;

        bool b_a = false;
        bool b_b = false;

        if (holoBuggyMatcher != null)
        {
            holoBuggyMatcher.gameObject.SetActive(interactable.attachedToHand);
        }

        if (interactable.attachedToHand)
        {
            SteamVR_Input_Sources hand = interactable.attachedToHand.handType;

            steer = actionSteer.GetAxis(hand);

            if (actionTurbo.GetStateDown(hand) && turbofill >= 1)
            {
                buggy.StartCoroutine(buggy.turboGo(turboTime));
                turboing = true;
            }

            throttle = actionThrottle.GetAxis(hand);
            b_a = actionBrake.GetState(hand);
            b_b = actionReset.GetState(hand);
            brake = b_a ? 1 : 0;
            reset = actionReset.GetStateDown(hand);

            if (turboing)
            {
                turbofill -= Time.deltaTime / turboTime;
                if (turbofill <= 0)
                    turboing = false;
            }
            else
            {
                turbofill += Time.deltaTime / turboTime / 3;
            }
            turbofill = Mathf.Clamp01(turbofill);
        }

        if (reset)
        {
            buggy.Reset();
        }

        if (ui_Canvas != null)
        {
            ui_Canvas.gameObject.SetActive(interactable.attachedToHand);

            usteer = Mathf.Lerp(usteer, steer.x, Time.deltaTime * 9);
            ui_steer.localEulerAngles = Vector3.forward * usteer * -ui_steerangle;
            ui_rpm.fillAmount = Mathf.Lerp(ui_rpm.fillAmount, Mathf.Lerp(ui_fillAngles.x, ui_fillAngles.y, throttle), Time.deltaTime * 4);
            float speedLim = 40;
            ui_speed.fillAmount = Mathf.Lerp(ui_fillAngles.x, ui_fillAngles.y, 1 - (Mathf.Exp(-buggy.speed / speedLim)));

            ui_turboBar.fillAmount = turbofill;
        }

        Joystick.localRotation = joySRot;
        Joystick.Rotate(steer.y * -joystickRot, steer.x * -joystickRot, 0, Space.Self);

        Trigger.localRotation = trigSRot;
        Trigger.Rotate(throttle * -triggerRot, 0, 0, Space.Self);
        buttonA.localScale = new Vector3(1, 1, b_a ? 0.3f : 1.0f);
        buttonB.localScale = new Vector3(1, 1, b_b ? 0.3f : 1.0f);

        buggy.steer = steer;
        buggy.throttle = throttle;
        buggy.handBrake = brake;
        buggy.controllerReference = transform;
    }

    private void LateUpdate()
    {
        if (holoBuggyMatcher != null)
        {
            holoBuggyMatcher.rotation = buggy.transform.rotation;
        }
    }

    private float buzztimer;
    private IEnumerator DoBuzz()
    {
        while (true)
        {
            while (buzztimer < 1)
            {
                buzztimer += Time.deltaTime * buggy.mvol * 70;
                yield return null;
            }
            buzztimer = 0;
            if (interactable.attachedToHand)
            {
                interactable.attachedToHand.TriggerHapticPulse((ushort)Mathf.RoundToInt(300 * Mathf.Lerp(1.0f, 0.6f, buggy.mvol)));
            }
        }
    }
}