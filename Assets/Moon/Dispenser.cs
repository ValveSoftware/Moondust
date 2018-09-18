using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assembler;
using Valve.VR.InteractionSystem;

public class Dispenser : MonoBehaviour
{
    public GameObject item;
    public bool assembler;

    public float distanceToSpawn = 3f;

    private Interactable interactable;
    private Interactable lastInteractable;
    private AssemblerComponent assemblerComponent;


    private void Update()
    {
        if (interactable == null)
        {
            if (lastInteractable == null || Vector3.Distance(this.transform.position, lastInteractable.transform.position) > distanceToSpawn)
            {
                GameObject instantiated = (GameObject)Instantiate(item, transform.position, transform.rotation);
                interactable = instantiated.GetComponent<Interactable>();
                instantiated.SetActive(true);
                interactable.GetComponent<Rigidbody>().position = transform.position;
                interactable.GetComponent<Rigidbody>().rotation = transform.rotation;
                interactable.GetComponent<Rigidbody>().isKinematic = true;

                instantiated.SetActive(false);
                StartCoroutine(DoGrow(instantiated));

                if (assembler)
                {
                    assemblerComponent = instantiated.GetComponent<AssemblerComponent>();
                    assemblerComponent.enabled = false;
                    foreach (Collider collider in instantiated.GetComponentsInChildren<Collider>())
                    {
                        collider.gameObject.layer = 1;
                    }
                }
            }
        }
        else
        {
            if (interactable.attachedToHand)
            {
                interactable.GetComponent<Rigidbody>().isKinematic = false;

                if (assembler)
                {
                    foreach (Collider collider in interactable.GetComponentsInChildren<Collider>())
                    {
                        collider.gameObject.layer = 0;
                    }
                    assemblerComponent.enabled = true;
                }
                assemblerComponent = null;

                lastInteractable = interactable;
                interactable = null;
            }
        }
    }

    private IEnumerator DoGrow(GameObject toGrowGameObject)
    {
        Transform toGrow = toGrowGameObject.transform;

        float startTime = Time.time;
        float overTime = 0.5f;
        float endTime = startTime + overTime;

        Vector3 initialScale = Vector3.one * 0.01f;
        Vector3 endScale = toGrow.transform.localScale;

        toGrowGameObject.SetActive(true);

        while (Time.time < endTime)
        {
            toGrow.localScale = Vector3.Lerp(initialScale, endScale, (Time.time - startTime) / overTime);
            yield return null;
        }

        toGrow.localScale = endScale;
    }
}