using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPortal : MonoBehaviour
{
    public Transform render;

    public Transform spawnPos;

    public Rigidbody resetter;

    public float shootForce = 4;

    public float disableColsTime = 0.5f;

    [HideInInspector]
    public bool resetting;

    public AudioSource au;

    public GameObject fizzler;

    public bool alwaysOpen = false;

    public AudioClip exitSound;
    public AudioClip portalSound;

    [Tooltip("disable collisions based on collision bounds, waiting until the object has fully left the portal.")]
    public bool disableCollidersSmart;
    
    private void Start()
    {
        if (!alwaysOpen)
        {
            render.gameObject.SetActive(false);
        }
    }

    public IEnumerator DoDisableColliders(float waitTime)
    {
        if (disableCollidersSmart)
        {
            //get full bounds of rigidbody
            Collider[] cols = resetter.GetComponentsInChildren<Collider>();
            Bounds bounds = new Bounds(resetter.transform.position, Vector3.zero);
            for(int i = 0; i < cols.Length; i++)
            {
                bounds.Encapsulate(cols[i].bounds);
            }
            float longestSide = Mathf.Max(new float[] { bounds.size.x, bounds.size.y, bounds.size.z });

            resetter.detectCollisions = false;
            bool fullyEjected = false;
            while (!fullyEjected)
            {
                yield return null;
                // wait until object has cleared the portal fully
                fullyEjected = InverseTransformPointUnscaled(transform, resetter.worldCenterOfMass).z > longestSide / 2 * 1.1f;
            }
            resetter.detectCollisions = true;
            
        }
        else // just use timing
        {
            resetter.detectCollisions = false;

            yield return new WaitForSeconds(waitTime);

            resetter.detectCollisions = true;
        }
    }

    public static Vector3 InverseTransformPointUnscaled(Transform t, Vector3 position)
    {
        var worldToLocalMatrix = Matrix4x4.TRS(t.position, t.rotation, Vector3.one).inverse;
        return worldToLocalMatrix.MultiplyPoint3x4(position);
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

            if (portalSound)
            {
                au.PlayOneShot(portalSound, 0.5f);
            }

            while (scaleOverTime < 1)
            {
                scaleOverTime += Time.deltaTime / 0.3f;
                render.localScale = Vector3.one * Mathf.SmoothStep(0, 1, scaleOverTime);
                yield return null;
            }
            render.localScale = Vector3.one;
        }

        ResetObject();

        StartCoroutine(DoDisableColliders(disableColsTime));

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

    public void CancelReset()
    {
        StopAllCoroutines();
        resetter.detectCollisions = true;
        render.gameObject.SetActive(false);
    }

    protected virtual void ResetObject()
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