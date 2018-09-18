using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class flarescale : MonoBehaviour
{
    public float brightness = 1;

    public float factor = 1;
    public float origval;

    public bool scl = false;

    private float scaledScale;
    private LensFlare lensFlare;

    private void Awake()
    {
        lensFlare = this.GetComponent<LensFlare>();
    }

    private void Start()
    {
        scaledScale = transform.localScale.x + 0.0001f;
    }

    
    private void OnWillRenderObject()
    {
        float scaler = 1;
        if (scl)
        {
            scaler = transform.localScale.z / scaledScale;
        }

        float distance = Camera.current.transform.InverseTransformPoint(transform.position).z;

        lensFlare.brightness = Mathf.Lerp(origval, brightness / distance, factor) * scaler;
    }
}