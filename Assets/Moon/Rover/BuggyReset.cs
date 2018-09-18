using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuggyReset : ResetPortal
{
    public BuggyBuddy buggy;

    public Transform[] ppos;
    
    private void Start()
    {
        CalibratePpos();
    }

    private void CalibratePpos()
    {
        for (int portalIndex = 0; portalIndex < ppos.Length; portalIndex++)
        {
            RaycastHit hit;
            Transform portal = ppos[portalIndex];
            if (Physics.Raycast(portal.position, -portal.forward, out hit))
            {
                portal.position = hit.point + portal.forward * 0.1f;
                portal.rotation = Quaternion.LookRotation(hit.normal, portal.up);
            }
        }
    }

    public void ResetBuggy()
    {
        FindSpot(buggy.transform.position);
        ResetNow();
    }

    private void FindSpot(Vector3 target)
    {
        int closestPortalIndex = 0;
        float closestDistance = Mathf.Infinity;
        for (int portalIndex = 0; portalIndex < ppos.Length; portalIndex++)
        {
            Transform portal = ppos[portalIndex];
            float distance = (portal.position - target).sqrMagnitude;
            if (distance < closestDistance)
            {
                closestPortalIndex = portalIndex;
                closestDistance = distance;
            }
        }

        transform.position = ppos[closestPortalIndex].position;
        transform.rotation = ppos[closestPortalIndex].rotation;
    }

    public override void ResetNow()
    {
        FindSpot(buggy.transform.position);
        base.ResetNow();
    }

    public override void ResetObject()
    {
        buggy.ResetStranded();
        buggy.transform.position = spawnPos.position;
        buggy.transform.rotation = spawnPos.rotation;
        buggy.body.angularVelocity = Vector3.zero;
        buggy.body.velocity = transform.forward * shootForce;
    }
}