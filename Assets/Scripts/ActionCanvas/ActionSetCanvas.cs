using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class ActionSetCanvas : MonoBehaviour
{
    private Text text;
    private void Start()
    {
        if (Debug.isDebugBuild == false)
        {
            Destroy(this.transform.parent.gameObject);
            return;
        }

        text = this.GetComponent<Text>();
        SteamVR_ActionSet_Manager.updateDebugTextInBuilds = true;
    }

    private void Update()
    {
        text.text = SteamVR_ActionSet_Manager.debugActiveSetListText;
    }
}
