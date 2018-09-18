using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

[Serializable]
public class ActionCanvasElementVector2 : ActionCanvasElement
{
    public SteamVR_Action_Vector2 action;

    protected Vector2 elementAxis;

    protected bool initialAxisSet = false;

    public override void Initialize(ActionCanvasBase actionCanvasBase)
    {
        base.Initialize(actionCanvasBase);

        SetPosition(Vector2.zero);
    }

    public override void Update()
    {
        bool active = CheckActive(action, actionCanvas.handType);

        if (active)
        {
            Vector2 actionAxis = action.GetAxis(actionCanvas.handType);

            if (elementAxis != actionAxis || initialAxisSet == false)
            {
                Color newColor;
                if (actionAxis == Vector2.zero)
                    newColor = actionCanvas.offColor;
                else
                    newColor = actionCanvas.onColor;

                SetFillColor(newColor);

                SetPosition(actionAxis);

                elementAxis = actionAxis;

                initialAxisSet = true;
            }
        }
    }
}