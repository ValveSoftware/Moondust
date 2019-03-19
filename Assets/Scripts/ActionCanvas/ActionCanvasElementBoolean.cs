using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

[Serializable]
public class ActionCanvasElementBoolean : ActionCanvasElement
{
    public SteamVR_Action_Boolean action;

    protected bool elementState;

    protected bool initialStateSet = false;

    public override void Initialize(ActionCanvasBase actionCanvasBase)
    {
        base.Initialize(actionCanvasBase);

        SetFillColor(actionCanvasBase.offColor);
    }

    public override void Update()
    {
        CheckActive(action, actionCanvas.handType);

        bool actionState = action.GetState(actionCanvas.handType);

        if (elementState != actionState || initialStateSet == false)
        {
            Color newColor = actionState ? actionCanvas.onColor : actionCanvas.offColor;

            SetFillColor(newColor);

            elementState = actionState;

            initialStateSet = true;
        }
    }
}