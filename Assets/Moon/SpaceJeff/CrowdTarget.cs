using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdTarget : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rigidbody;
    [Tooltip("chase if false, flee if true")]
    public bool flee;
    public float radius;

    public bool active = true;

    public void SetActive(bool active)
    {
        this.active = active;
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = flee ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }


    private void Start()
    {
        JoeJeffCrowdSim.RegisterCrowdTarget(this);
    }

    private void OnDestroy()
    {
        JoeJeffCrowdSim.DeRegisterCrowdTarget(this);
    }
}
