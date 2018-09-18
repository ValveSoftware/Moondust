using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VaccuumSuck : MonoBehaviour
{
    //note: all of this happens with local scaling

    public float strength;

    public float halfwidth;

    public float halflength;

    public float distancer;

    public AudioSource au_suck;
    public AudioSource au_insuck;

    [HideInInspector]
    public float DustCollected;

    public float collectSpeed = 1;

    public SkinnedMeshRenderer tubesDistort;

    public DustMinerGame gameManager;
    

    private float v_suck;
    private float v_insuck;


    public float GetStrength(Vector3 point)
    {
        point = transform.InverseTransformPoint(point);
        float side = new Vector2(point.x, point.y).magnitude;
        float s = 1 / Mathf.Pow(2, side / halfwidth);
        s += 1 / Mathf.Pow(2, halflength / halfwidth);

        return s * strength;
    }

    public float distance(Vector3 point)
    {
        return transform.InverseTransformPoint(point).magnitude / distancer;
    }

    public Vector3 GetAxisTarget(Vector3 point)
    {
        return Math3d.ProjectPointOnLineSegment(transform.position, transform.position + transform.forward * 30, point);
    }

    public void Suck()
    {
        v_suck = Mathf.Lerp(v_suck, 1, Time.deltaTime * 10);
        //v_suck = 1;
    }

    public void SuckIn()
    {
        if (gameManager)
        {
            gameManager.SuckDust();
        }

        v_insuck += 1;
        DustCollected += collectSpeed;
    }
    
    private void Update()
    {
        v_insuck -= Time.deltaTime * 2;
        v_insuck = Mathf.Clamp01(v_insuck);
        au_insuck.volume = v_insuck;

        v_suck = Mathf.Lerp(v_suck, 0, Time.deltaTime);
        v_suck = Mathf.Clamp01(v_suck * 0.95f + v_insuck * 0.1f);
        au_suck.volume = v_suck;
        au_suck.pitch = Mathf.Lerp(0.7f, 1.2f, v_suck);

        tubesDistort.SetBlendShapeWeight(0, 100 * Random.Range(0.90f, 1.00f) * (v_insuck + v_suck) / 2);
    }

    [HideInInspector]
    public bool rockIn;

    private void OnTriggerStay(Collider collider)
    {
        if (collider.GetComponentInParent<MoonRock>())
        {
            rockIn = true;
        }
    }

    private void LateUpdate()
    {
        rockIn = false;
    }
}