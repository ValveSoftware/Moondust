using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
public class PortalEater : MonoBehaviour
{
    public Transform targetEatPoint;

    private void OnCollisionEnter(Collision collision)
    {
        Interactable interactable = collision.rigidbody.GetComponent<Interactable>();
        if (interactable != null && Vector3.Dot(collision.rigidbody.velocity, transform.forward)>0)
        {
            var turret = interactable.GetComponent<turret>();
            var gren = interactable.GetComponent<grenade>();

            if (turret || gren)
            {
                interactable.GetComponent<Rigidbody>().isKinematic = true;

                Destroy(gren);
                Destroy(turret);

                Collider[] colliders = interactable.GetComponentsInChildren<Collider>();
                foreach (Collider collider in colliders)
                    if (collider != null)
                        Destroy(collider);

                StartCoroutine(DoEat(interactable));
            }
        }
    }

    private IEnumerator DoEat(Interactable interactable)
    {
        float startTime = Time.time;
        float overTime = 0.5f;
        float endTime = startTime + overTime;

        Vector3 startPosition = interactable.transform.position;
        Vector3 startScale = interactable.transform.localScale;
        Quaternion startRotation = interactable.transform.rotation;

        Vector3 endPosition = targetEatPoint.position;
        Vector3 endScale = startScale / 2f;
        Quaternion endRotation = targetEatPoint.rotation;

        while (Time.time < endTime)
        {
            yield return null;

            if (interactable == null)
                yield break;

            float lerp = Mathf.InverseLerp(startTime, endTime, Time.time);
            interactable.transform.position = Vector3.Slerp(startPosition, endPosition, lerp);
            interactable.transform.localScale = Vector3.Lerp(startScale, endScale, lerp);
            interactable.transform.rotation = Quaternion.Slerp(startRotation, endRotation, lerp);
        }

        Destroy(interactable.gameObject);
    }
}
