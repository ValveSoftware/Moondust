using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SceneConfig : MonoBehaviour
{
    public float shadowDist;

    public Vector3 gravity;

    public bool update;
    
    private void Start()
    {
        QualitySettings.shadowDistance = shadowDist;
        Physics.gravity = gravity;
    }
    
    private void Update()
    {
        if (update)
        {
            update = false;
            Start();
        }
    }
}