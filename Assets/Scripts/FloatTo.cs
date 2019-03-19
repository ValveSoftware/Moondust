using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatTo : MonoBehaviour
{
    public Transform target;

    public float speed;

    // allow racetracks to tell table to float to a certain point
    public bool allowRacetrackOverride = false;
    
    private void Update()
    {
        Transform activeTarget = target;

        if (allowRacetrackOverride)
        {
            if (Race.activeRace != null) activeTarget = Race.activeRace.floatingTableTarget;
        }

        if (speed == 0)
        {
            transform.position = activeTarget.position;
            transform.rotation = activeTarget.rotation;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, activeTarget.position, Time.deltaTime * speed);
            transform.rotation = Quaternion.Lerp(transform.rotation, activeTarget.rotation, Time.deltaTime * speed);
        }
    }
}