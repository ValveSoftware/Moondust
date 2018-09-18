using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class BuggyBuddyActionCanvas : ActionCanvasBase
{
    public ActionCanvasElementBoolean teleport;
    public ActionCanvasElementBoolean brake;
    public ActionCanvasElementBoolean reset;
    public ActionCanvasElementSingle throttle;
    public ActionCanvasElementVector2 steer;

    protected override void Awake()
    {
        base.Awake();

        teleport.Initialize(this);
        brake.Initialize(this);
        reset.Initialize(this);
        throttle.Initialize(this);
        steer.Initialize(this);
    }

    protected override void Update()
    {
        base.Update();

        teleport.Update();
        brake.Update();
        reset.Update();
        throttle.Update();
        steer.Update();
    }
}