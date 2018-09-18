using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;

public class ThrowingDistanceMarker : MonoBehaviour
{
    public GameObject measurementPrefab;
    public float minMagnitude = 1.5f;
    private float minDistance = 1.5f;

    private Interactable item;

    private bool hasShownMeasurement;

    private void Awake()
    {
        item = this.GetComponent<Interactable>();
        item.onDetachedFromHand += (delegate { thrown(); });
    }

    bool interacted;

    public void thrown()
    {
        interacted = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasShownMeasurement == false)
        {
            if (interacted && !item.attachedToHand)
            {
                if (collision.impulse.magnitude > minMagnitude)
                {
                    float distance = Vector3.Distance(Camera.main.transform.position, this.transform.position);
                    if (distance > minDistance)
                    {
                        hasShownMeasurement = true;

                        float distanceFt = distance * 3.28084f;

                        GameObject measurement = GameObject.Instantiate(measurementPrefab);
                        Text text = measurement.GetComponentInChildren<Text>();
                        text.text = string.Format("{0:0.0}m\n{1:0.0}ft", distance, distanceFt);

                        Destroy(measurement, destroyAfterTime);

                        text.StartCoroutine(DoRaise(measurement, this.transform.position));
                    }
                }
            }
        }
    }

    private float moveUpAmount = 1f;
    private float moveUpOverTime = 6.24f;
    private float destroyAfterTime = 7f;

    private IEnumerator DoRaise(GameObject measurement, Vector3 startPosition)
    {
        Text text = measurement.GetComponentInChildren<Text>();
        Outline outline = measurement.GetComponentInChildren<Outline>();

        float startTime = Time.time;
        float overTime = moveUpOverTime;
        float endTime = startTime + overTime;
        
        Vector3 endPosition = startPosition + (Vector3.up * moveUpAmount);

        Vector3 startScale = Vector3.one * 0.001f;

        Vector3 endScale = Vector3.one;

        float distance = Vector3.Distance(Camera.main.transform.position, this.transform.position);
        if (distance > 4)
            endScale *= (distance / 4);
        

        while (Time.time < endTime)
        {
            float lerp = (Time.time - startTime) / overTime;
            float halflerp = Mathf.Clamp01((Time.time - startTime) / (overTime / 4));

            measurement.transform.position = Vector3.Lerp(startPosition, endPosition, lerp);
            measurement.transform.localScale = Vector3.Lerp(startScale, endScale, halflerp);

            measurement.transform.LookAt(Camera.main.transform, Vector3.up);

            float colorTime = (Time.time - startTime);
            text.color = Color.Lerp(Color.clear, Color.black, Mathf.Clamp01(Mathf.Sin(colorTime / 2)));
            outline.effectColor = Color.Lerp(Color.clear, Color.green, Mathf.Clamp01(Mathf.Sin(colorTime / 2)));

            yield return null;
        }

        text.color = Color.clear;
        outline.effectColor = Color.clear;
    }
}
