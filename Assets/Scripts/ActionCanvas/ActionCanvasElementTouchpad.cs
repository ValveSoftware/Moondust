using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

[Serializable]
public class ActionCanvasElementTouchpad : ActionCanvasElementButton
{
    protected SteamVR_Action_Vector2 actionAxis;

    protected bool initialPositionSet = false;

    public override void Initialize(ActionCanvasBase actionCanvasBase)
    {
        base.Initialize(actionCanvasBase);

        actionAxis = SteamVR_Input.GetAction<SteamVR_Action_Vector2>(actionName + "Axis");
    }

    public override void Update()
    {
        base.Update();

        bool changed = actionAxis[actionCanvas.handType].changed;

        if (changed || initialPositionSet == false)
        {
            SetPosition(actionAxis[actionCanvas.handType].axis);
            SetText(actionAxis[actionCanvas.handType].axis);

            initialPositionSet = true;
        }
    }


    protected void SetText(Vector2 newValue)
    {
        texts[1].text = string.Format("({0:0.00}, {1:0.000})", newValue.x, newValue.y);
    }
}