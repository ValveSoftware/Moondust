using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneGravity : MonoBehaviour
{
    public Vector3 gravityToSet = new Vector3(0, -9.81f, 0);

    private Vector3 gravityCache;

    private void Awake()
    {
        gravityCache = Physics.gravity;

        Physics.gravity = gravityToSet;
    }

    private void OnDestroy()
    {
        Physics.gravity = gravityCache;
    }
}
