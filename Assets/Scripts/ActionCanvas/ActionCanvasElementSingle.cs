using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

[Serializable]
public class ActionCanvasElementSingle : ActionCanvasElement
{
    public SteamVR_Action_Single action;

    protected float elementAxis;

    protected bool initialAxisSet = false;

    public override void Initialize(ActionCanvasBase actionCanvasBase)
    {
        base.Initialize(actionCanvasBase);

        SetFillAmount(0);
    }

    public override void Update()
    {
        bool active = CheckActive(action, actionCanvas.handType);

        if (active)
        {
            float actionAxis = action.GetAxis(actionCanvas.handType);

            if (elementAxis != actionAxis || initialAxisSet == false)
            {
                SetFillAmount(actionAxis);

                elementAxis = actionAxis;

                initialAxisSet = true;
            }
        }
    }
}