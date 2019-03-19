using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoeJeffPortal : MonoBehaviour
{
    public Transform outsidePortal;
    public Transform insidePortal;

    private Vector3 initialScale;

    private float suckToPortalSpeed = 10f;

    private IEnumerator Start()
    {
        initialScale = this.transform.localScale;

        Vector3 startScale = initialScale * 0.1f;
        Vector3 endScale = initialScale;

        yield return StartCoroutine(DoScale(this.gameObject, startScale, endScale, 0.25f));
    }

    private IEnumerator DoScale(GameObject scaleObject, Vector3 startScale, Vector3 endScale, float overTime)
    {
        scaleObject.transform.localScale = startScale;

        float startTime = Time.time;
        float endTime = startTime + overTime;

        while (Time.time <= endTime)
        {
            scaleObject.transform.localScale = Vector3.Slerp(startScale, endScale, (Time.time - startTime) / overTime);
            yield return null;
        }

        scaleObject.transform.localScale = endScale;
    }

    public void EatJoeJeffRagdoll(GameObject joeJeffRagdoll, GameObject spine)
    {
        StartCoroutine(DoEatJoeJeff(joeJeffRagdoll, spine));
    }

    private float eatTime = 0.5f;

    private IEnumerator DoEatJoeJeff(GameObject joeJeffRagdoll, GameObject spine)
    {
        Rigidbody[] rigidbodies = joeJeffRagdoll.GetComponentsInChildren<Rigidbody>(true);

        yield return StartCoroutine(DoSuckToLocation(spine, outsidePortal.position, suckToPortalSpeed));

        for (int rigidIndex = 0; rigidIndex < rigidbodies.Length; rigidIndex++)
            rigidbodies[rigidIndex].detectCollisions = false;

        StartCoroutine(DoSuckToLocation(spine, insidePortal.position, suckToPortalSpeed / 3));
        StartCoroutine(DoScale(spine, spine.transform.localScale, spine.transform.localScale / 2f, eatTime));

        yield return new WaitForSeconds(eatTime + 0.1f);

        Destroy(joeJeffRagdoll);

        yield return new WaitForSeconds(0.1f);

        JoeJeffCrowdSim.instance.SpawnFromPortal(insidePortal.position, insidePortal.forward);

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(DoScale(this.gameObject, initialScale, initialScale * 0.1f, 0.25f));

        Destroy(this.gameObject);
    }

    private IEnumerator DoSuckToLocation(GameObject joeJeff, Vector3 toLocation, float speed)
    {
        float distance = Vector3.Distance(joeJeff.transform.position, toLocation);

        float overTime = distance / speed;
        float startTime = Time.time;
        float endTime = startTime + overTime;

        Vector3 startPosition = joeJeff.transform.position;

        while (Time.time <= endTime)
        {
            joeJeff.transform.position = Vector3.Slerp(startPosition, toLocation, (Time.time - startTime) / overTime);
            yield return null;
        }
    }
}