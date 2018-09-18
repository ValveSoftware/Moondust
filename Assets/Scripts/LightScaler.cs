using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightScaler : MonoBehaviour
{
    public float magnitude;
    private Light lightToScale;

    private void Update()
    {
        if (lightToScale == null)
            lightToScale = GetComponent<Light>();
        else
            lightToScale.range = magnitude * transform.lossyScale.z;
    }
}