using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class SpaceActionCanvas : ActionCanvasBase
{
    public ActionCanvasElementBoolean deleterSelect;
    public ActionCanvasElementBoolean deleterDelete;

    protected override void Awake()
    {
        base.Awake();

        deleterSelect.Initialize(this);
        deleterDelete.Initialize(this);
    }

    protected override void Update()
    {
        base.Update();

        deleterSelect.Update();
        deleterDelete.Update();
    }
}