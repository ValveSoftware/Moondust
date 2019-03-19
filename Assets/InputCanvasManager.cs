using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;

public class InputCanvasManager : MonoBehaviour
{
    public static bool drawRawInput = false;

    public static bool drawActionInput = true;

    public Toggle ui_drawRawInput;
    public Toggle ui_drawActionInput;

    public GameObject ui_rawInput;
    public GameObject ui_actionInput;

    public SteamVR_ActionSet rawActionSet;


    private void Start()
    {
        ui_drawRawInput.isOn = drawRawInput;
        ui_drawActionInput.isOn = drawActionInput;
        UpdateDisplay();
    }


    public void UpdateDrawRawInput(bool value)
    {
        drawRawInput = value;
        UpdateDisplay();
    }

    public void UpdateDrawActionInput(bool value)
    {
        drawActionInput = value;
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        ui_actionInput.SetActive(drawActionInput);
        ui_rawInput.SetActive(drawRawInput);

        if (drawRawInput)
            rawActionSet.Activate();
        else
            rawActionSet.Deactivate();
    }
}
