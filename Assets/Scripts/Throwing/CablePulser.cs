using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CablePulser : MonoBehaviour
{
    public Rotate rotatorMain;
    public Rotate rotatorReverse;

    public Valve.VR.InteractionSystem.LinearDrive lever;

    public new Renderer renderer;

    public Transform[] particles;

    private Color initialEmissionColor;
    private Color emissionColor;
    private int emissionID;

    public float maxRotateSpeed = 100f;
    public float maxReverseSpeed = 50f;

    private float time;

    private void Awake()
    {
        if (renderer == null)
            renderer = this.GetComponent<Renderer>();

        emissionID = Shader.PropertyToID("_EmissionColor");
        initialEmissionColor = renderer.material.GetColor(emissionID);
    }

    private void Update()
    {
        

        if (lever.linearMapping.value > 0.01f)
            time += (Time.deltaTime + (rotatorMain.speed / 1000f)) / 1.5f;
        
        for (int index = 0; index < particles.Length; index++)
        {
            particles[index].localScale = Vector3.one * lever.linearMapping.value;
        }
        
        rotatorMain.speed = lever.linearMapping.value * maxRotateSpeed;
        rotatorReverse.speed = -rotatorMain.speed - (lever.linearMapping.value * maxReverseSpeed);

        if (lever.linearMapping.value > 0.01f)
            emissionColor = initialEmissionColor * (1.25f + (Mathf.Sin(time) / 2f));
        else
            emissionColor = Color.black;

        renderer.material.SetColor(emissionID, emissionColor);
    }
}
