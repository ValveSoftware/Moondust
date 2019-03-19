using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

[Serializable]
public class ActionCanvasElementFinger : RawInputCanvasElement
{
    protected SteamVR_Action_Skeleton skeleton;

    protected bool initialStateSet = false;

    protected float lastValue;
    protected float currentValue;

    protected int index;

    public override void Initialize(ActionCanvasBase actionCanvasBase)
    {
        base.Initialize(actionCanvasBase);

        skeleton = SteamVR_Input.GetAction<SteamVR_Action_Skeleton>("Skeleton" + actionCanvas.handType.ToString());

        index = int.Parse(actionName.Replace("finger", ""));
    }

    public override void Update()
    {
        lastValue = currentValue;
        currentValue = skeleton.fingerCurls[index];

        bool changed = Mathf.Abs(lastValue - currentValue) > 0.001f;

        if (changed || initialStateSet == false)
        {
            SetFillAmount(currentValue);
            SetText(currentValue);

            initialStateSet = true;
        }
    }

    protected void SetText(float newValue)
    {
        texts[1].text = string.Format("{0:0.000}", newValue);
    }
}