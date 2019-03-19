using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem.Sample;

public class ForceDome : MonoBehaviour
{
    public Renderer r;

    public float radius;
    public float outerRadius;

    public float highBound;

    public float lowBound;

    public BuggyReset[] buggies;

    private void Update()
    {
        foreach (BuggyReset buggy in buggies)
        {
            float dist = (buggy.buggy.transform.position - transform.position).sqrMagnitude;
            if (dist > radius * radius && buggy.buggy.transform.position.y > lowBound && buggy.buggy.transform.position.y < highBound)
            {
                buggy.ResetBuggy();
                Debug.Log("reset");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.DrawWireSphere(transform.position, outerRadius);
        Vector3 pos = transform.position;
        pos.y = (highBound + lowBound) / 2;
        Gizmos.DrawWireCube(pos, new Vector3(radius, highBound - lowBound, radius));
    }
}