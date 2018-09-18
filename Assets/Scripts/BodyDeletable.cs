using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyDeletable : MonoBehaviour
{
    public bool ifearth;

    public GameObject check;

    private void Update()
    {
        if (check == null)
        {
            if (ifearth)
            {
                Earth.destroyed = true;
            }
        }
    }
}