using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

[Serializable]
public abstract class RawInputCanvasElement : ActionCanvasElement
{
    public string actionName;

    protected AllInputsActionCanvas allInputsActionCanvas;

    public override void Initialize(ActionCanvasBase actionCanvasBase)
    {
        base.Initialize(actionCanvasBase);
        allInputsActionCanvas = (AllInputsActionCanvas)actionCanvasBase;

        if (string.IsNullOrEmpty(actionName))
            actionName = element.name;
    }

    protected override bool CheckActive(ISteamVR_Action_In action, SteamVR_Input_Sources source)
    {
        return true;
    }
}