using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Match the bounds of this particle emitter to the bounds of a renderer
/// </summary>
public class matchBounds : MonoBehaviour
{
    public Renderer target;

    public ParticleSystem me;

    public int maxps = 30;

    private float initialRateOverTime;

    private void Start()
    {
        ParticleSystem.EmissionModule em = me.emission;
        initialRateOverTime = em.rateOverTime.constant;
    }
    
    private void Update()
    {
        ParticleSystem.ShapeModule s = me.shape;
        s.scale = target.bounds.size;

        if (target.GetComponent<ParticleSystem>())
        {
            s.scale -= Vector3.one * target.GetComponent<ParticleSystem>().main.startSize.constantMax * 2;
            s.position = transform.InverseTransformPoint(target.bounds.center);

            ParticleSystem.EmissionModule em = me.emission;
            ParticleSystem.MinMaxCurve c = em.rateOverTime;
            c.constant = initialRateOverTime * (float)target.GetComponent<ParticleSystem>().particleCount / maxps;
            em.rateOverTime = c;
        }
    }
}