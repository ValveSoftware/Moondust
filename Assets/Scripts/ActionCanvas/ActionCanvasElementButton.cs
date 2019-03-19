using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

[Serializable]
public class ActionCanvasElementButton : RawInputCanvasElement
{
    protected ButtonStates lastButtonState;
    protected ButtonStates buttonState;

    protected SteamVR_Action_Boolean actionTouch;
    protected SteamVR_Action_Boolean actionPress;

    protected bool initialStateSet = false;

    protected FillImageOptions fillType = FillImageOptions.Inside;

    public override void Initialize(ActionCanvasBase actionCanvasBase)
    {
        base.Initialize(actionCanvasBase);

        actionTouch = SteamVR_Input.GetAction<SteamVR_Action_Boolean>(actionName + "Touch");
        actionPress = SteamVR_Input.GetAction<SteamVR_Action_Boolean>(actionName + "Press");
    }

    public override void Update()
    {
        lastButtonState = buttonState;

        bool touched = actionTouch[actionCanvas.handType].state;
        bool pressed = actionPress[actionCanvas.handType].state;

        if (pressed)
            buttonState = ButtonStates.Press;
        else if (touched)
            buttonState = ButtonStates.Touch;
        else
            buttonState = ButtonStates.None;

        if (buttonState != lastButtonState || initialStateSet == false)
        {
            Color newColor;

            if (pressed)
                newColor = allInputsActionCanvas.buttonPressColor;
            else if (touched)
                newColor = allInputsActionCanvas.buttonTouchColor;
            else
                newColor = allInputsActionCanvas.buttonDefaultColor;

            if (fillType == FillImageOptions.Inside)
                SetFillColor(newColor);
            else if (fillType == FillImageOptions.Border)
            {
                newColor.a = 1;
                SetBorderColor(newColor);
            }

            initialStateSet = true;
        }
    }
}

public enum ButtonStates
{
    None,
    Touch,
    Press,
}
public enum FillImageOptions
{
    None,
    Inside,
    Border,
}