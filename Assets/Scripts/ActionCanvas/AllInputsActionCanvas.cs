using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class AllInputsActionCanvas : ActionCanvasBase
{
    public Color buttonDefaultColor;
    public Color buttonTouchColor;
    public Color buttonPressColor;

    public ActionCanvasElementButton a;
    public ActionCanvasElementButton b;
    public ActionCanvasElementTrigger trigger;
    public ActionCanvasElementTouchpad touchpad;
    public ActionCanvasElementTouchpad thumbstick;
    public ActionCanvasElementAxis topForce;
    public ActionCanvasElementAxis gripForce;
    public ActionCanvasElementFinger index;
    public ActionCanvasElementFinger middle;
    public ActionCanvasElementFinger ring;
    public ActionCanvasElementFinger pinky;

    protected List<ActionCanvasElement> elements = new List<ActionCanvasElement>();

    protected override void Awake()
    {
        elements.Add(a);
        elements.Add(b);
        elements.Add(trigger);
        elements.Add(touchpad);
        elements.Add(thumbstick);
        elements.Add(topForce);
        elements.Add(gripForce);
        elements.Add(index);
        elements.Add(middle);
        elements.Add(ring);
        elements.Add(pinky);

        for (int index = 0; index < elements.Count; index++)
            elements[index].Initialize(this);
    }

    protected override void Update()
    {
        for (int index = 0; index < elements.Count; index++)
            elements[index].Update();
    }
}