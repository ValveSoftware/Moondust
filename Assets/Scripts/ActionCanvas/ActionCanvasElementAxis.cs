using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

[Serializable]
public class ActionCanvasElementAxis : RawInputCanvasElement
{
    protected SteamVR_Action_Single actionAxis;

    protected bool initialFillSet = false;

    protected string initialText;

    public override void Initialize(ActionCanvasBase actionCanvasBase)
    {
        base.Initialize(actionCanvasBase);

        actionAxis = SteamVR_Input.GetAction<SteamVR_Action_Single>(actionName + "Axis");
    }

    public override void Update()
    {
        bool changed = actionAxis[actionCanvas.handType].changed;

        if (changed || initialFillSet == false)
        {
            SetFillAmount(actionAxis[actionCanvas.handType].axis);
            SetText(actionAxis[actionCanvas.handType].axis);

            initialFillSet = true;
        }
    }

    protected void SetText(float newValue)
    {
        texts[1].text = string.Format("{0:0.000}", newValue);
    }
}