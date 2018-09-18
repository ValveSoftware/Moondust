using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;


public class TouchHighlight : MonoBehaviour
{
    private static Material touchHighlightMaterial;

    private Hand hand;

    private void Start()
    {
        hand = GetComponent<Hand>();
    }

    private void Update()
    {
        if (hand == null)
            return;

        if (hand.hoveringInteractable && !hand.currentAttachedObject)
        {
            Interactable hoverTarget = hand.hoveringInteractable;
            if (hoverTarget != null)
                Highlight(hoverTarget.gameObject);
        }
    }

    public void Highlight(GameObject go)
    {
        if (touchHighlightMaterial == null)
            touchHighlightMaterial = (Material)Resources.Load("TouchHighlight", typeof(Material));

        foreach (MeshFilter mesh in go.GetComponentsInChildren<MeshFilter>())
        {
            Matrix4x4 matrix = Matrix4x4.TRS(mesh.transform.position, mesh.transform.rotation, mesh.transform.lossyScale);
            Graphics.DrawMesh(mesh.mesh, matrix, touchHighlightMaterial, 0);
        }
    }
}