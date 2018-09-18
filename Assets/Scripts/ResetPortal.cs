using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPortal : MonoBehaviour
{
    public Transform render;

    public Transform spawnPos;

    public Rigidbody resetter;

    public float shootForce = 4;

    public float disableColsTime = 2;

    [HideInInspector]
    public bool resetting;

    public AudioSource au;

    public GameObject fizzler;

    public bool alwaysOpen = false;

    public AudioClip exitSound;

    public bool disableCollisionsWith = false;
    public Collider[] disableCollisionsWithList;
    
    private void Start()
    {
        if (!alwaysOpen)
        {
            render.gameObject.SetActive(false);
        }
    }

    public IEnumerator DoDisableColliders(float waitTime)
    {
        Collider[] colliders = resetter.GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }

        yield return new WaitForSeconds(waitTime);

        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }
        yield return null;
    }

    public IEnumerator DoDisableCollisionsWithList(float waitTime)
    {
        Collider[] colliders = resetter.GetComponentsInChildren<Collider>();

        for (int listIndex = 0; listIndex < disableCollisionsWithList.Length; listIndex++)
        {
            for (int itemIndex = 0; itemIndex < colliders.Length; itemIndex++)
            {
                Physics.IgnoreCollision(disableCollisionsWithList[listIndex], colliders[itemIndex], true);
            }
        }

        yield return new WaitForSeconds(waitTime);

        for (int listIndex = 0; listIndex < disableCollisionsWithList.Length; listIndex++)
        {
            for (int itemIndex = 0; itemIndex < colliders.Length; itemIndex++)
            {
                Physics.IgnoreCollision(disableCollisionsWithList[listIndex], colliders[itemIndex], false);
            }
        }
        yield return null;
    }

    public virtual void ResetNow()
    {
        StartCoroutine(DoReset());
    }

    private IEnumerator DoReset()
    {
        resetting = true;

        Fizzle();
        float scaleOverTime = 0;
        if (!alwaysOpen)
        {

            yield return new WaitForSeconds(1);

            render.localScale = Vector3.zero;
            render.gameObject.SetActive(true);
            au.Play();
            while (scaleOverTime < 1)
            {
                scaleOverTime += Time.deltaTime / 0.3f;
                render.localScale = Vector3.one * Mathf.SmoothStep(0, 1, scaleOverTime);
                yield return null;
            }
            render.localScale = Vector3.one;
        }

        ResetObject();

        if (disableCollisionsWith == false)
            StartCoroutine(DoDisableColliders(disableColsTime));
        else
            StartCoroutine(DoDisableCollisionsWithList(disableColsTime));

        if (!alwaysOpen)
        {
            yield return new WaitForSeconds(1);

            while (scaleOverTime > 0)
            {
                scaleOverTime -= Time.deltaTime / 0.5f;
                render.localScale = Vector3.one * Mathf.SmoothStep(0, 1, scaleOverTime);
                yield return null;
            }

            render.gameObject.SetActive(false);
        }
        resetting = false;
    }

    public void Fizzle()
    {
        if (fizzler != null)
        {
            GameObject fiz = (GameObject)Instantiate(fizzler, resetter.transform.position, resetter.transform.rotation);
            fiz.GetComponent<Rigidbody>().velocity = resetter.velocity;
            fiz.GetComponent<Rigidbody>().angularVelocity = resetter.angularVelocity;
        }

        resetter.transform.position = Vector3.down * 1000;
    }

    public virtual void ResetObject()
    {
        resetter.transform.position = spawnPos.position;
        resetter.transform.rotation = spawnPos.rotation;
        resetter.angularVelocity = Vector3.zero;
        resetter.velocity = transform.forward * shootForce;

        if (exitSound)
        {
            au.PlayOneShot(exitSound, 0.5f);
        }
    }
}