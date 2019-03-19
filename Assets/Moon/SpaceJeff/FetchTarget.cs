using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FetchTarget : MonoBehaviour
{
    static FetchTarget instance;

    public static Transform Find()
    {
        if(instance == null) { Debug.LogError("No FetchTarget found in scene, add one."); return null; }
        return instance.transform;
    }

    private void Awake()
    {
        instance = this;
    }
}
