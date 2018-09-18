using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class ActionCanvasBase : MonoBehaviour
{
    public SteamVR_Input_Sources handType;

    public ActionCanvasElementBoolean grip;

    public ActionCanvasElementBoolean pinch;

    public Color onColor = new Color(0.55f, 0.8f, 0.25f, 1);
    public Color offColor = new Color(0.5f, 0.5f, 0.5f, 0.2f);

    [Range(0, 1)]
    public float activeOpacity = 0.75f;

    [Range(0, 1)]
    public float inactiveOpacity = 0.2f;

    protected virtual void Awake()
    {
        grip.Initialize(this);
        pinch.Initialize(this);

        if (onColor.a > activeOpacity)
            onColor.a = activeOpacity;

        if (offColor.a > activeOpacity)
            offColor.a = activeOpacity;
    }

    protected virtual void Update()
    {
        grip.Update();
        pinch.Update();
    }
}