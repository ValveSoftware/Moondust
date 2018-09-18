using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArgesPlayer : MonoBehaviour
{
    public float scale;

    public Camera mainCam;

    public static ArgesPlayer player;


    public void SetScale(float newScale)
    {
        scale = newScale;
        transform.localScale = Vector3.one * newScale;
        mainCam.nearClipPlane = 0.1f * newScale;
    }

    private void Start()
    {
        player = this;
        SetScale(scale);
    }
}