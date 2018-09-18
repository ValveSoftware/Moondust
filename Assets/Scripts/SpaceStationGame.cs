using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assembler;

public class SpaceStationGame : MonoBehaviour
{
    public AssemblerComponent core;
    public Canvas coreHint;

    private bool coreAttached;

    private void Update()
    {
        if (!coreAttached)
        {
            bool attach = false;

            foreach (AssemblerPoint point in core.pointColliders)
            {
                if (point.isConnected)
                {
                    attach = true;
                }
            }

            if (attach)
            {
                coreAttached = true;
                coreHint.gameObject.SetActive(false);
            }
        }
    }
}