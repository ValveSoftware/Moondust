using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

//places this transform on the nearest point in the playspace.
//applies in local space, so it needs to be a child of the camerarig.

public class PlaceAtPlayspaceEdge : MonoBehaviour
{
    [Tooltip("defaults to parent")]
    public Transform playarea;

    void Start()
    {
        if (playarea == null) playarea = transform.parent;
        ApplyPlacement();
    }

    public void ApplyPlacement()
    {
        var rect = new HmdQuad_t();
        if (SteamVR_PlayArea.GetBounds(SteamVR_PlayArea.Size.Calibrated, ref rect))
        {
            var corners = new HmdVector3_t[] { rect.vCorners0, rect.vCorners1, rect.vCorners2, rect.vCorners3 };
            Bounds playareaBounds = new Bounds(Vector3.zero, new Vector3(Mathf.Abs(corners[0].v0 - corners[1].v0), 0, Mathf.Abs(corners[0].v2 - corners[3].v2)));
            Vector3 closestPt = playareaBounds.ClosestPoint(playarea.InverseTransformPoint(transform.position));
            transform.position = playarea.TransformPoint(closestPt);
        }
    }
}
