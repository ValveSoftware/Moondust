using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assembler;

public class ThrusterRandom : MonoBehaviour
{
    public Transform thrust;
    public AudioSource au;

    private bool animating;
    private AssemblerComponent comp;
    private bool thrusting;
    private float thrustLvl;

    
    private void Start()
    {
        animating = false;
        comp = GetComponent<AssemblerComponent>();
    }
    
    private void Update()
    {
        if (!animating && comp.connected)
        {
            StartCoroutine(DoAnimate());
            animating = true;
        }

        thrustLvl = Mathf.Lerp(thrustLvl, thrusting ? 1 : 0, Time.deltaTime * (thrusting ? 5 : 2));
        thrust.localScale = new Vector3(1, 1, Random.Range(1.9f, 1.2f) * thrustLvl);
        au.volume = thrustLvl;
        au.enabled = thrustLvl > 0.05f;
    }

    private IEnumerator DoAnimate()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            thrusting = true;
            yield return new WaitForSeconds(Random.Range(0.4f, 2.0f));
            thrusting = false;

            yield return new WaitForSeconds(Random.Range(5, 15));
        }
    }
}