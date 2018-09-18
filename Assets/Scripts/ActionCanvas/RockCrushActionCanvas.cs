using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class RockCrushActionCanvas : ActionCanvasBase
{
    public ActionCanvasElementSingle squeeze;

    public ActionCanvasElementBoolean crush;


    protected override void Awake()
    {
        base.Awake();

        squeeze.Initialize(this);
        crush.Initialize(this);
    }

    protected override void Update()
    {
        base.Update();

        squeeze.Update();
        crush.Update();
    }
}