using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assembler;
using Valve.VR.InteractionSystem;
using Valve.VR;

public class DeleterTool : MonoBehaviour
{
    public LineRenderer laserbeam;
    public LineRenderer aimer;

    //float brightness = 1;

    //[HideInInspector]
    public DeleterStatus status; 

    public AudioClip au_error;
    public AudioClip au_select;
    public AudioClip au_zap;

    public AudioSource au;

    public GameObject confirmer;

    public Transform ptr;

    public SteamVR_ActionSet setToActivate;
    public SteamVR_Action_Boolean actionSelect;
    public SteamVR_Action_Boolean actionDelete;


    private Rigidbody body;
    private IEnumerator cycler;

    private AssemblerComponent comp;
    private AssemblerComponent compb;

    private Transform hitTr;
    private Interactable interactable;

    private bool used;

    private Vector3 lockedPosition;


    private void Start()
    {
        interactable = GetComponent<Interactable>();
        body = GetComponent<Rigidbody>();
        interactable.onAttachedToHand += Interactable_onAttachedToHand;
        interactable.onDetachedFromHand += Interactable_onDetachedFromHand;
    }

    private void Interactable_onDetachedFromHand(Hand hand)
    {
        setToActivate.Deactivate();
    }

    private void Interactable_onAttachedToHand(Hand hand)
    {
        setToActivate.ActivatePrimary();
    }

    private void Update()
    {
        used = interactable.attachedToHand;

        if (used)
        {
            transform.parent = null;
            body.isKinematic = false;

            if (cycler == null)
            {
                cycler = CycleModes();
                StartCoroutine(cycler);
            }

            float dist = 200;
            RaycastHit hit;
            bool didHit = Physics.Raycast(aimer.transform.position, aimer.transform.forward, out hit);
            if (didHit)
            {
                dist = hit.distance;
            }

            confirmer.SetActive(status == DeleterStatus.Locked);
            
            if (status == DeleterStatus.Aiming)
            {
                laserbeam.transform.rotation = aimer.transform.rotation;
                if (didHit)
                {
                    if (hit.collider.GetComponentInParent<AssemblerComponent>())
                    {
                        if (hit.collider.GetComponentInParent<AssemblerComponent>().enabled)
                        {
                            comp = hit.collider.GetComponentInParent<AssemblerComponent>();
                            hitTr = hit.transform;
                            lockedPosition = hit.transform.InverseTransformPoint(hit.point);
                            comp.PreviewOneDelete();
                        }
                    }
                }
                else
                {
                    comp = null;
                }

                laserbeam.gameObject.SetActive(false);
                aimer.gameObject.SetActive(true);
                aimer.SetPosition(1, Vector3.forward * dist / aimer.transform.lossyScale.z);

            }
            
            if (status == DeleterStatus.Locked)
            {
                Vector3 worldLock = hitTr.transform.TransformPoint(lockedPosition);
                laserbeam.gameObject.SetActive(true);
                laserbeam.transform.rotation = Quaternion.LookRotation(worldLock - laserbeam.transform.position);

                laserbeam.SetPosition(1, laserbeam.transform.InverseTransformPoint(worldLock));
                aimer.gameObject.SetActive(false);
                comp.PreviewDelete();

                if (Vector3.Angle(aimer.transform.forward, worldLock - laserbeam.transform.position) > 30) // angle to break lock
                {
                    status = DeleterStatus.Aiming; //reset
                    au.PlayOneShot(au_error);
                }
            }

            if (status == DeleterStatus.Zapping)
            {
                laserbeam.gameObject.SetActive(true);
                laserbeam.SetPosition(1, Vector3.forward * dist / aimer.transform.lossyScale.z);
                aimer.gameObject.SetActive(false);
            }
            
            if (status == DeleterStatus.Cancelling)
            {
                laserbeam.gameObject.SetActive(true);
                laserbeam.SetPosition(1, Vector3.forward * dist / aimer.transform.lossyScale.z);
                aimer.gameObject.SetActive(false);
            }
        }
        else
        {
            comp = null;
            if (cycler != null)
            {
                StopCoroutine(cycler);
                cycler = null;
            }

            status = DeleterStatus.Aiming;
            laserbeam.gameObject.SetActive(false);
            aimer.gameObject.SetActive(false);
            confirmer.SetActive(false);
        }
    }

    private IEnumerator CycleModes()
    {
        while (true)
        {
            while (status == DeleterStatus.Aiming)
            {
                if (interactable.attachedToHand)
                {
                    if (actionSelect.GetState(interactable.attachedToHand.handType))
                    {
                        if (comp != null)
                        {
                            status = DeleterStatus.Locked; // lock on
                            au.PlayOneShot(au_select);
                        }
                        else
                        {
                            status = DeleterStatus.Cancelling; // cancel
                            au.PlayOneShot(au_error);
                        }
                    }
                }
                yield return null;
            }

            while (status == DeleterStatus.Locked)
            {
                if (interactable.attachedToHand)
                {
                    if (actionDelete.GetState(interactable.attachedToHand.handType))
                    {
                        comp.Delete();
                        comp = null;
                        status = 0; // reset
                        au.PlayOneShot(au_zap);
                    }
                }
                yield return null;
            }

            while (status == DeleterStatus.Zapping)
            {
                float zapTime = 2;
                while (zapTime > 0)
                {
                    zapTime -= Time.deltaTime * 4;
                    SetColor(zapTime);
                    yield return null;
                }
                status = DeleterStatus.Aiming; // reset
                yield return null;
                SetColor(1);
            }
            
            while (status == DeleterStatus.Cancelling)
            {
                float cancelTime = 1;
                while (cancelTime > 0)
                {
                    cancelTime -= Time.deltaTime * 4;
                    SetColor(cancelTime);
                    yield return null;
                }
                status = DeleterStatus.Aiming; //reset
                yield return null;
                SetColor(1);
            }

            while (interactable.attachedToHand != null && actionSelect.GetState(interactable.attachedToHand.handType))
            {
                yield return null;
            }
        }
    }

    private void OnDestroy()
    {
        setToActivate.Deactivate();
        
        if (interactable != null)
        {
            interactable.onAttachedToHand -= Interactable_onAttachedToHand;
            interactable.onDetachedFromHand -= Interactable_onDetachedFromHand;
        }
    }

    private void SetColor(float f)
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white * f, 0.0f), new GradientColorKey(Color.white * f, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1, 0.0f), new GradientAlphaKey(1, 1.0f) }
        );
        laserbeam.colorGradient = gradient;
    }

    public enum DeleterStatus
    {
        Aiming,
        Locked,
        Zapping,
        Cancelling,
    }
}