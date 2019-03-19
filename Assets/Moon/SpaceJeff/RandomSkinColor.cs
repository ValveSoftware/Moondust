using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSkinColor : MonoBehaviour
{
    public Renderer skinRenderer;

    [HideInInspector]
    public Color skinColor;

    void Start()
    {
        skinColor = Random.ColorHSV(0, 1f, 0.1f, 0.8f, 0.4f, 1.0f, 1, 1);
        MaterialPropertyBlock props = new MaterialPropertyBlock();
        props.SetColor("_Color", skinColor);
        skinRenderer.SetPropertyBlock(props);
    }
}
