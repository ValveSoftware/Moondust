using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class JoeJeffRagdoll : MonoBehaviour
{
    public Transform skeletonRoot;
    public Renderer skinRenderer;

    public Transform joeJeffSpine;

    public GameObject joeJeffPortalPrefab;

    List<Transform> skeletonChildren = new List<Transform>();
    Rigidbody[] rigidbodies;

    public float liveTime = 3f;
    private float spawnTime;
    public bool shouldRespawn = true;

    void AddChildrenToSkeletonList(Transform t, List<Transform> list)
    {
        if (list.Contains(t) == false)
        {
            list.Add(t);
        }
        foreach(Transform child in t)
        {
            AddChildrenToSkeletonList(child, list);
        }
    }


    private void Awake()
    {
        spawnTime = Time.time;
        AddChildrenToSkeletonList(skeletonRoot, skeletonChildren);
        rigidbodies = this.GetComponentsInChildren<Rigidbody>(true);
    }

    private void Start()
    {
        lastPosition = joeJeffSpine.position;
        lastPositionTime = Time.time;

        checkTime += Random.value;
    }


    /// <summary>
    /// Initiate this Ragdoll
    /// </summary>
    /// <param name="referenceRoot">The Transform of the JoeJeff who just died</param>
    /// <param name="skeleton">The skeleton root of the JoeJeff who just died. Will be used to match pose.</param>
    /// <param name="skinColor">Skin color to match</param>
    /// <param name="velocity">The velocity of the JoeJeff who just died</param>
    /// <param name="forceVector">Force direction and magnitude</param>
    /// <param name="forcePosition">point of contact</param>
    public void Ragdoll(Transform referenceRoot, Transform skeleton, Color skinColor, Vector3 velocity, Vector3 forceVector, Vector3 forcePosition)
    {
        transform.parent = referenceRoot.parent;
        transform.position = referenceRoot.position;
        transform.rotation = referenceRoot.rotation;
        transform.localScale = referenceRoot.localScale;

        List<Transform> skeletonCopy = new List<Transform>();
        AddChildrenToSkeletonList(skeleton, skeletonCopy);

        //match skeleton of recently deceased JoeJeff
        for(int i = 0; i< skeletonChildren.Count; i++)
        {
            skeletonChildren[i].localPosition = skeletonChildren[i].localPosition;
            skeletonChildren[i].localRotation = skeletonCopy[i].localRotation;

            //match velocity of recently deceased JoeJeff and add some impact force
            Rigidbody rb = skeletonChildren[i].GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = velocity;
                rb.AddForceAtPosition(forceVector, forcePosition, ForceMode.VelocityChange);
            }
        }

        //match skin color of recently deceased JoeJeff
        MaterialPropertyBlock props = new MaterialPropertyBlock();
        props.SetColor("_Color", skinColor);
        skinRenderer.SetPropertyBlock(props);
    }

    private Vector3 lastPosition;
    private float lastPositionTime;
    private float lastDistance;

    private bool portalCreated = false;

    private float checkTime = 0f;

    private void Update()
    {
        if (portalCreated || joeJeffSpine == null)
            return;

        if(Time.time > spawnTime + liveTime) {
            if (shouldRespawn)
            {
                if ((Time.time - lastPositionTime) > checkTime)
                {
                    if ((joeJeffSpine.position - lastPosition).magnitude < 0.25f)
                        CreatePortal();

                    lastPosition = joeJeffSpine.position;
                    lastPositionTime = Time.time;
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void CreatePortal()
    {
        Vector3 origin = joeJeffSpine.position + (Vector3.up * 0.5f);

        RaycastHit[] hitinfos = Physics.RaycastAll(origin, Vector3.down, 2);

        for (int hitIndex = 0; hitIndex < hitinfos.Length; hitIndex++)
        {
            RaycastHit hit = hitinfos[hitIndex];
            if (hit.collider != null && hit.collider.attachedRigidbody == null)
            {
                Vector3 spawnPoint = hit.point;
                Vector3 spawnRotation = hit.normal;

                GameObject portalObject = GameObject.Instantiate<GameObject>(joeJeffPortalPrefab);
                portalObject.transform.position = spawnPoint;
                portalObject.transform.forward = spawnRotation;

                JoeJeffPortal portal = portalObject.GetComponent<JoeJeffPortal>();
                portal.EatJoeJeffRagdoll(this.gameObject, joeJeffSpine.gameObject);

                portalCreated = true;
            }
        }
    }
}
