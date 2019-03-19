using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingTargetRing : MonoBehaviour
{
    public ThrowingTarget target;
    public int ID;

    private Renderer ringRenderer;
    private Material ringMaterial;


    private void Start()
    {
        ringRenderer = GetComponent<Renderer>();
        ringMaterial = ringRenderer.material;
    }

    public void Flash()
    {
        StartCoroutine(DoFlashAnim());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            if (collision.rigidbody.GetComponent<TargetAbsorbable>() != null)
            {
                target.Hit(ID, collision.contacts[0].point);
                Destroy(collision.gameObject);
            }
        }
    }

    private IEnumerator DoFlashAnim()
    {
        float currentBrightness = 1;
        float brightnessMax = 2.5f;

        while (currentBrightness < brightnessMax)
        {
            currentBrightness += Time.deltaTime / 0.1f * (brightnessMax - 1);
            ringMaterial.color = Color.white * currentBrightness;
            yield return null;
        }

        while (currentBrightness > 1)
        {
            currentBrightness -= Time.deltaTime / 1 * (brightnessMax - 1);
            ringMaterial.color = Color.white * currentBrightness;
            yield return null;
        }

        ringMaterial.color = Color.white;
    }
}