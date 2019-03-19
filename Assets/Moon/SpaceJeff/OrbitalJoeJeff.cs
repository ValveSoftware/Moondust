using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalJoeJeff : MonoBehaviour
{
    [Range(0,1)]
    public float probability = 0.5f;
    void Start()
    {
        // disable this gameObject x% of the time
        if (Random.value > probability) gameObject.SetActive(false);
    }
}
