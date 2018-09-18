using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earth : MonoBehaviour
{
    public static bool destroyed = false;
    
    private void Start()
    {
        GetComponent<Renderer>().sharedMaterial.renderQueue = 2998;

        if (destroyed)
        {
            gameObject.SetActive(false);
        }
    }
}