using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingTarget : MonoBehaviour
{
    public ThrowingTargetRing[] rings;

    public GameObject hitPrefab;

    public Color[] hitColors;


    private float hitTimer = 10;
    private float hitTime = 0.1f;


    public void Hit(int ringIndex, Vector3 position)
    {
        if (hitTimer > hitTime)
        {
            rings[ringIndex].Flash();

            GameObject hitInstance = (GameObject)Instantiate(hitPrefab, position, transform.rotation, transform);
            position = hitInstance.transform.localPosition;
            position.z = 0;
            hitInstance.transform.localPosition = position;
            hitInstance.transform.localScale = Vector3.one;

            float pitchShift = Mathf.Lerp(0.8f, 1.2f, 1 - ((float)ringIndex / (float)rings.Length));
            hitInstance.GetComponent<AudioSource>().pitch = pitchShift;
            hitInstance.GetComponentInChildren<Renderer>().material.SetColor("_TintColor", hitColors[ringIndex]);
            hitTimer = 0;
        }
    }
    
    private void Update()
    {
        hitTimer += Time.deltaTime;
    }
}