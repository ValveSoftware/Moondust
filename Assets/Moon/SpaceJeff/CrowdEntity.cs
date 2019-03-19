using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CrowdEntity : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent nav;

    [HideInInspector]
    public float prevWanderTime;
    [HideInInspector]
    public float nextWanderTime;
    [HideInInspector]
    public bool wandering = true;

    [HideInInspector]
    public JoeJeffCrowdSim crowd;

    [HideInInspector]
    public CrowdTarget currentTarget;

    public bool Fleeing { get; private set; }
    public bool Chasing { get; private set; }

    [Tooltip("set to something reasonable to the agent scale and speed")]
    public float fleeIncrement = 5;


    private void Awake()
    {
        nav = gameObject.GetComponent<NavMeshAgent>();
        InvokeRepeating("PerformTargeting", 1, Random.Range(0.5f, 1.0f));
    }


    void PerformTargeting()
    {
        Fleeing = false;
        Chasing = false;

        if (currentTarget == null || wandering == false || nav.enabled == false)
            return;

        if (JoeJeffCrowdSim.CheckRange(transform.position, currentTarget.transform.position, currentTarget.radius) == false)
        {
            //have moved out of range of target
            currentTarget = null;
            return;
        }
        if (currentTarget.flee == true) // flee from target
        {
            Fleeing = true;
            Vector3 targetDir = (currentTarget.transform.position - transform.position);
            targetDir.y = 0;
            targetDir.Normalize();
            nav.SetDestination(crowd.SampleNavMesh(transform.position - targetDir * fleeIncrement, 8));
        }
        if (currentTarget.flee == false) // chase target
        {
            Chasing = true;
            nav.SetDestination(crowd.SampleNavMesh(currentTarget.transform.position));
        }

    }


    public void WanderEvent(Vector3 target, float nextTime)
    {
        if (nav != null && nav.isActiveAndEnabled && nav.isOnNavMesh)
        {
            nav.SetDestination(target);
        }

        prevWanderTime = Time.time;
        nextWanderTime = nextTime;
    }
}
