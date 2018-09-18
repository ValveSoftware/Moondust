using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class ThrowingActionCanvas : ActionCanvasBase
{
    public ActionCanvasElementBoolean prime;
    public ActionCanvasElementBoolean fire;
    public ActionCanvasElementSingle gripSqueeze;
    public ActionCanvasElementSingle pinchSqueeze;

    protected override void Awake()
    {
        base.Awake();

        prime.Initialize(this);
        fire.Initialize(this);
        gripSqueeze.Initialize(this);
        pinchSqueeze.Initialize(this);
    }

    protected override void Update()
    {
        base.Update();

        prime.Update();
        fire.Update();
        gripSqueeze.Update();
        pinchSqueeze.Update();
    }
}