using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetColoring : MonoBehaviour
{
    private List<Rigidbody> colored = new List<Rigidbody>();

    private new Collider collider;

    private new Renderer renderer;

    private void Start()
    {
        collider = this.GetComponent<Collider>();
        renderer = this.GetComponent<Renderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            if (colored.Contains(collision.rigidbody) == false)
            {
                colored.Add(collision.rigidbody);

                ContactPoint contact = collision.contacts[0];
                RaycastHit hit;

                float rayLength = 0.1f;
                Ray ray = new Ray(contact.point - contact.normal * rayLength * 0.5f, contact.normal);
                Color color = Color.white; // default color when the raycast fails for some reason ;)
                if (collider.Raycast(ray, out hit, rayLength))
                {
                    Texture2D tex = (Texture2D)renderer.material.mainTexture;
                    color = tex.GetPixelBilinear(hit.textureCoord.x, hit.textureCoord.y);
                    Vector3 colorVec = new Vector3(color.r, color.g, color.b);
                    colorVec.Normalize();
                    color.r = colorVec.x;
                    color.g = colorVec.y;
                    color.b = colorVec.z;
                    color.a = 1;
                    //Vectrosity.VectorLine.SetRay3D(color, 3, contact.point, transform.forward * 3f);

                    /*
                    Renderer[] renderers = collision.rigidbody.GetComponentsInChildren<Renderer>();
                    foreach (Renderer otherRenderer in renderers)
                    {
                        if (otherRenderer is TrailRenderer)
                            continue;

                        color.a = 0.5f;

                        otherRenderer.material.color = color;
                        otherRenderer.material.SetColor("_EmissionColor", color);

                    }
                    */
                }
            }
        }
    }
}
