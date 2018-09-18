using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    public float moveSpeed = 0.1f;

    public float grabSpeed = 0.1f;

    public float dropSpeed = 0.1f;

    public float maxDistanceToGrab = 5f;

    public float maxY = 15.5f;

    public Transform dropPoint;

    private List<Thrown> grabbingList = new List<Thrown>();

    private Vector3 grabbingForce;

    private void Awake()
    {
    }

    private void Update()
    {
        float closestDistance = float.MaxValue;
        Transform closestObject = null;

        for (int index = 0; index < Thrower.instance.thrownList.Count; index++)
        {
            Thrown thrown = Thrower.instance.thrownList[index];

            if (thrown != null && thrown.processed == false)
            {
                Transform thrownTransform = Thrower.instance.thrownList[index].transform;

                Vector3 localPosition = this.transform.InverseTransformPoint(thrownTransform.transform.position);
                
                //if it's in front of me
                if (localPosition.z > 0)
                {
                    float distance = Vector3.Distance(this.transform.position, thrownTransform.position);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestObject = thrownTransform;
                    }
                }

                if (localPosition.y < 0.75 && localPosition.y > -0.75)
                {
                    if (localPosition.z < maxDistanceToGrab)
                    {
                        if (grabbingList.Contains(thrown) == false)
                        {
                            Grab(thrown);
                        }
                    }
                }
            }
        }

        MoveGrabber(closestObject);

        grabbingList.RemoveAll(grabbing => grabbing == null);

        SuckTowards();

        Drop();
    }

    private void MoveGrabber(Transform closestObject)
    {
        if (closestObject != null)
        {
            Vector3 targetPosition = this.transform.position;
            targetPosition.y = closestObject.position.y;

            if (targetPosition.y > maxY)
                targetPosition.y = maxY;

            this.transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    private void SuckTowards()
    {
        for (int grabbingIndex = 0; grabbingIndex < grabbingList.Count; grabbingIndex++)
        {
            Thrown grabbing = grabbingList[grabbingIndex];

            Vector3 localPosition = dropPoint.InverseTransformPoint(grabbing.transform.position);

            if (localPosition.z >= 0.1f)
            {
                Vector3 targetPosition = dropPoint.transform.position;
                targetPosition.y = this.transform.position.y;

                grabbing.transform.position = Vector3.MoveTowards(grabbing.transform.position, targetPosition, dropSpeed * Time.deltaTime);
            }
        }
    }

    private void Drop()
    {
        for (int grabbingIndex = 0; grabbingIndex < grabbingList.Count; grabbingIndex++)
        {
            Thrown grabbing = grabbingList[grabbingIndex];

            Vector3 localPosition = dropPoint.InverseTransformPoint(grabbing.transform.position);

            if (localPosition.z <= 0.1f)
            {
                if (localPosition.y < 0.25f && localPosition.y > -0.25f)
                {
                    grabbingList.Remove(grabbing);
                    grabbing.rigidbody.isKinematic = false;
                }
                else
                {
                    grabbing.transform.position = Vector3.MoveTowards(grabbing.transform.position, dropPoint.position, dropSpeed * Time.deltaTime);
                }
            }
        }
    }

    private void Grab(Thrown thrown)
    {
        grabbingList.Add(thrown);

        thrown.rigidbody.velocity = Vector3.zero;
        thrown.rigidbody.angularVelocity = Vector3.zero;
        thrown.rigidbody.isKinematic = true;
    }
}
