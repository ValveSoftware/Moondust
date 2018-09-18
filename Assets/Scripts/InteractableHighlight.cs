using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableHighlight : MonoBehaviour
{
    [Tooltip("Set whether or not you want this interactible to highlight when hovering over it")]
    public bool highlightOnHover = true;

    [Tooltip("An array of child gameObjects to not render a highlight for. Things like transparent parts, vfx, etc.")]
    public GameObject[] hideHighlight;

    private MeshFilter[] highlightRenderers;
    private SkinnedMeshRenderer[] highlightSkinnedRenderers;

    private static Material highlightMat;

    private void Start()
    {
        highlightRenderers = GetComponentsInChildren<MeshFilter>();
        highlightSkinnedRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        if (highlightMat == null)
            highlightMat = (Material)Resources.Load("SteamVR_HoverHighlight", typeof(Material));

        if (highlightMat == null)
            Debug.LogError("Hover Highlight Material is missing. Please create a material named 'SteamVR_HoverHighlight' and place it in a Resources folder");
    }

    private void HandHoverUpdate()
    {
        if (highlightOnHover)
        {
            if (highlightRenderers != null)
            {
                for (int highlightIndex = 0; highlightIndex < highlightRenderers.Length; highlightIndex++)
                {
                    MeshFilter mesh = highlightRenderers[highlightIndex];
                    if (CheckHighlights(mesh))
                    {
                        Matrix4x4 matrix = Matrix4x4.TRS(mesh.transform.position, mesh.transform.rotation, mesh.transform.lossyScale);
                        for (int subIndex = 0; subIndex < mesh.sharedMesh.subMeshCount; subIndex++)
                        {
                            Graphics.DrawMesh(mesh.sharedMesh, matrix, highlightMat, 0, null, subIndex);
                        }
                    }
                }
            }

            if (highlightSkinnedRenderers != null)
            {
                for (int highlightIndex = 0; highlightIndex < highlightSkinnedRenderers.Length; highlightIndex++)
                {
                    SkinnedMeshRenderer mesh = highlightSkinnedRenderers[highlightIndex];
                    if (CheckHighlights(mesh))
                    {
                        Matrix4x4 matrix = Matrix4x4.TRS(mesh.transform.position, mesh.transform.rotation, mesh.transform.lossyScale);
                        for (int subIndex = 0; subIndex < mesh.sharedMesh.subMeshCount; subIndex++)
                        {
                            Graphics.DrawMesh(mesh.sharedMesh, matrix, highlightMat, 0, null, subIndex);
                        }
                    }
                }
            }
        }
    }

    private bool CheckHighlights(Component comp)
    {
        for (int highlightIndex = 0; highlightIndex < hideHighlight.Length; highlightIndex++)
            if (comp.gameObject == hideHighlight[highlightIndex])
                return false;

        return true;
    }
}