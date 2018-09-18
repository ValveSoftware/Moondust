using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;


namespace Assembler
{
    public class AssemblerComponent : MonoBehaviour
    {
        [System.Serializable]
        public class snapPoint
        {
            public Vector3 point;
            public Vector3 normal;

            public int ID;

            public snapPoint(Vector3 p, Vector3 n, int i)
            {
                point = p;
                normal = n.normalized;
                ID = i;
            }
        }

        public static bool dismantle;

        public static AssemblerComponent coreComp;

        public bool faker;

        [Tooltip("Is this part the center that cannot be destroyed?")]
        public bool core;

        public snapPoint[] points;

        [HideInInspector]
        public AssemblerPoint[] pointColliders;

        [HideInInspector]
        public bool connected;

        public int treeDist; // distance from core on connection tree

        public GameObject previewHolo;

        public Material fakerSky;


        private Interactable interactable;
        private Rigidbody rb;
        private float rad = 0.3f;

        private bool wasgrab;

        private AssemblerPoint attachPoint;
        private AssemblerPoint m_attachPt;

        private void Start()
        {
            if (faker)
                return;

            if (previewHolo)
            {
                previewHolo.SetActive(true);
            }

            rb = GetComponent<Rigidbody>();
            pointColliders = new AssemblerPoint[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                GameObject g = new GameObject("pt_" + i);
                pointColliders[i] = (AssemblerPoint)g.AddComponent<AssemblerPoint>();
                pointColliders[i].radius = rad;
                pointColliders[i].transform.parent = transform;
                pointColliders[i].transform.position = transform.TransformPoint(points[i].point);
                pointColliders[i].transform.rotation = Quaternion.LookRotation(transform.TransformDirection(points[i].normal), transform.up);
                pointColliders[i].comp = this;
                pointColliders[i].ID = points[i].ID;
                pointColliders[i].gameObject.layer = 1; // set it to transparentFX layer, so they don't collide with eachother
            }


            if (core)
            {
                coreComp = this;
            }
            else
            {
                interactable = GetComponent<Interactable>();
            }
        }

        private void Update()
        {
            if (faker)
                return;

            bool detached = true;
            for (int pointIndex = 0; pointIndex < points.Length; pointIndex++)
            {
                if (core == false)
                {
                    pointColliders[pointIndex].connectable = (rb == null);

                    if (pointColliders[pointIndex].connected)
                        detached = false;
                }
                else
                {
                    detached = false;
                    pointColliders[pointIndex].connectable = true;
                }

            }

            if (core == false)
            {
                if (detached)
                {
                    if (connected)
                        Detach();
                }

                if (interactable.attachedToHand) // If i'm being held
                {
                    if (connected)
                    {
                        if (dismantle)
                        {
                            Detach();
                        }
                        else
                        {
                            //KnucklesPhysicalInteraction kpi = ki.heldBy;
                            //kpi.ForceGrab(coreComp.ki);
                        }
                    }

                    rb.isKinematic = false;
                    PointHitCheck[] hits = new PointHitCheck[points.Length];
                    for (int i = 0; i < points.Length; i++)
                    {


                        PointHitCheck hit;
                        if (pointColliders[i].CheckForPoints(out hit)) // check if each point is touching another object's
                        {
                            hits[i] = (hit);
                        }
                    }
                    attachPoint = FindNearest(hits);
                    if (attachPoint != null)
                    {
                        RenderSnap();
                    }
                }
                else
                {
                    if (wasgrab)
                    {
                        if (attachPoint != null)
                        {
                            Attach();
                            attachPoint = null;
                        }
                    }
                }

                wasgrab = interactable.attachedToHand;
            }
        }

        private void Attach()
        {
            if (previewHolo)
            {
                previewHolo.SetActive(false);
            }
            if (GetComponent<Animator>())
            {
                GetComponent<Animator>().SetTrigger("Open");
            }

            connected = true;

            Transform[] allTransforms = interactable.GetComponentsInChildren<Transform>(true);
            GameObject[] allGameObjects = new GameObject[allTransforms.Length];
            for (int transformIndex = 0; transformIndex < allTransforms.Length; transformIndex++)
                allGameObjects[transformIndex] = allTransforms[transformIndex].gameObject;

            interactable.hideHighlight = allGameObjects;

            transform.parent = coreComp.transform;
            treeDist = attachPoint.comp.treeDist + 1;

            SnapIn(transform);

            attachPoint.isConnected = true;
            attachPoint.connected = m_attachPt;

            m_attachPt.isConnected = true;
            m_attachPt.connected = attachPoint;
        }

        public void Detach()
        {
            if (previewHolo)
            {
                previewHolo.SetActive(true);
            }

            if (core == false)
            {
                rb.isKinematic = false;
                connected = false;
                for (int pointIndex = 0; pointIndex < points.Length; pointIndex++)
                {
                    if (pointColliders[pointIndex].isConnected)
                    {
                        if (pointColliders[pointIndex].connected.comp.treeDist > treeDist)// check if another component will be detached from the core. If so, start a chain reaction
                        {
                            pointColliders[pointIndex].connected.comp.Detach();
                        }
                    }

                    //detach my own points
                    pointColliders[pointIndex].DetachPoint();
                }

                float rvel = 0.5f;
                rb.velocity = new Vector3(Random.Range(-rvel, rvel), Random.Range(-rvel, rvel), Random.Range(-rvel, rvel));

                float ra = 20;
                rb.angularVelocity = new Vector3(Random.Range(-ra, ra), Random.Range(-ra, ra), Random.Range(-ra, ra));
            }
        }

        public void Delete()
        {
            if (faker == false)
            {
                for (int pointIndex = 0; pointIndex < points.Length; pointIndex++)
                {
                    if (pointColliders[pointIndex].isConnected)
                    {
                        if (pointColliders[pointIndex].connected.comp.treeDist > treeDist)// check if another component will be detached from the core. If so, start a chain reaction
                        {
                            pointColliders[pointIndex].connected.comp.Delete();
                        }
                    }
                }
                if (core == false)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                if (fakerSky != null)
                {
                    RenderSettings.skybox = fakerSky;
                }

                Destroy(gameObject);
            }
        }

        private static Material previewDelete;
        public void PreviewDelete()
        {
            if (previewDelete == null)
                previewDelete = (Material)Resources.Load("PreviewDelete", typeof(Material));

            foreach (MeshFilter mesh in GetMeshesUp())
            {
                Matrix4x4 matrix = Matrix4x4.TRS(mesh.transform.position, mesh.transform.rotation, mesh.transform.lossyScale);
                Graphics.DrawMesh(mesh.mesh, matrix, previewDelete, 0);
            }
        }

        private static Material previewDeleteSmall;
        public void PreviewOneDelete()
        {
            if (previewDeleteSmall == null)
                previewDeleteSmall = (Material)Resources.Load("PreviewDeleteSmall", typeof(Material));

            foreach (MeshFilter mesh in GetComponentsInChildren<MeshFilter>())
            {
                Matrix4x4 matrix = Matrix4x4.TRS(mesh.transform.position, mesh.transform.rotation, mesh.transform.lossyScale);
                Graphics.DrawMesh(mesh.mesh, matrix, previewDeleteSmall, 0);
            }
        }

        public List<MeshFilter> GetMeshesUp()
        {
            List<MeshFilter> meshes = new List<MeshFilter>();

            if (core == false)
            {
                foreach (MeshFilter meshFilter in GetComponentsInChildren<MeshFilter>())
                    meshes.Add(meshFilter);
            }

            for (int i = 0; i < points.Length; i++)
            {
                if (pointColliders[i].isConnected)
                {
                    if (pointColliders[i].connected.comp.treeDist > treeDist)// check if another component will be detached from the core. If so, start a chain reaction
                    {
                        meshes.AddRange(pointColliders[i].connected.comp.GetMeshesUp());

                    }
                }
            }

            return meshes;
        }

        public List<GameObject> GetObjectsUp()
        {
            List<GameObject> pointList = new List<GameObject>();

            for (int pointIndex = 0; pointIndex < points.Length; pointIndex++)
            {
                if (pointColliders[pointIndex].isConnected)
                {
                    if (pointColliders[pointIndex].connected.comp.treeDist > treeDist) // check if another component will be detached from the core. If so, start a chain reaction
                    {
                        pointList.Add(pointColliders[pointIndex].connected.comp.gameObject);
                        pointList.AddRange(pointColliders[pointIndex].connected.comp.GetObjectsUp());
                    }
                }
            }

            return pointList;
        }

        private AssemblerPoint FindNearest(PointHitCheck[] hits)
        {
            AssemblerPoint nearestPoint = null;
            float closestDistance = Mathf.Infinity;
            for (int pointIndex = 0; pointIndex < points.Length; pointIndex++)
            {
                if (hits[pointIndex] != null)
                {
                    if (hits[pointIndex].dist < closestDistance)
                    {
                        closestDistance = hits[pointIndex].dist;
                        nearestPoint = hits[pointIndex].point;
                        m_attachPt = pointColliders[pointIndex];
                    }
                }
            }

            return nearestPoint;
        }

        private static Material previewMaterial;
        private void RenderSnap()
        {
            Vector3 oldpos = transform.position;
            Quaternion oldrot = transform.rotation;
            SnapIn(transform, false);

            if (previewMaterial == null)
                previewMaterial = (Material)Resources.Load("PreviewHologram", typeof(Material));

            Matrix4x4 matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Graphics.DrawMesh(GetComponent<MeshFilter>().mesh, matrix, previewMaterial, 0);
            transform.position = oldpos;
            transform.rotation = oldrot;
        }

        private void SnapIn(Transform snapTransform, bool doSnap = true)
        {
            Vector3 initialPosition = snapTransform.position;
            Quaternion initialRotation = snapTransform.rotation;

            Transform tempObject = new GameObject().transform;
            Transform snapParent = snapTransform.parent;


            tempObject.rotation = m_attachPt.transform.rotation;
            Quaternion tempR = tempObject.rotation;
            snapTransform.parent = tempObject;
            tempObject.rotation = Quaternion.LookRotation(-attachPoint.transform.forward);

            Quaternion[] angs = new Quaternion[4]; // rotate by 90 degree increments to see which is closest to correct angle
            for (int i = 0; i < 4; i++)
            {
                angs[i] = tempObject.rotation;
                tempObject.Rotate(attachPoint.transform.forward * 90, Space.World);
            }

            float angClose = Mathf.Infinity;
            int closest = 0;

            for (int i = 0; i < 4; i++)
            {
                float a = Vector3.Angle(angs[i] * Vector3.right, tempR * Vector3.right);
                if (a < angClose)
                {
                    closest = i;
                    angClose = a;
                }
            }

            tempObject.rotation = angs[closest];

            snapTransform.parent = snapParent;
            Destroy(tempObject.gameObject);

            snapTransform.position = initialPosition;
            Vector3 pDiff = attachPoint.transform.position - m_attachPt.transform.position;
            snapTransform.position += pDiff;

            if (doSnap)
            {
                DestroyImmediate(interactable.GetComponent<Throwable>());
                DestroyImmediate(interactable.GetComponent<VelocityEstimator>());
                DestroyImmediate(interactable.GetComponent<Rigidbody>());

                Rigidbody parentRigidbody = snapParent.GetComponentInParent<Rigidbody>();

                var joints = snapTransform.GetComponentsInChildren<Joint>();
                foreach (var joint in joints)
                {
                    joint.GetComponent<Rigidbody>().rotation = snapTransform.rotation;
                    joint.connectedBody = parentRigidbody;
                    joint.autoConfigureConnectedAnchor = false;
                    joint.anchor = Vector3.zero;
                    joint.connectedAnchor = parentRigidbody.transform.InverseTransformPoint(joint.transform.position);
                    StartCoroutine(DoSetBreakForce(joint));
                }

                Destroy(interactable);

                //Interactable item = t.GetComponentInParent<Interactable>();
                //item.UpdateColliders(); //give the parent this item's colliders
            }
        }


        private IEnumerator DoSetBreakForce(Joint joint)
        {
            yield return new WaitForSeconds(0.1f);

            if (joint != null)
            {
                joint.breakForce = 2000000f;
                //joint.breakTorque = 300000f;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            for (int pointIndex = 0; pointIndex < points.Length; pointIndex++)
            {
                Gizmos.DrawWireSphere(transform.TransformPoint(points[pointIndex].point), rad);
            }

            for (int pointIndex = 0; pointIndex < points.Length; pointIndex++)
            {
                Gizmos.DrawLine(transform.TransformPoint(points[pointIndex].point) + transform.TransformDirection(points[pointIndex].normal) * rad, transform.TransformPoint(points[pointIndex].point) + transform.TransformDirection(points[pointIndex].normal) * rad * 1.8f);
            }
        }
    }

    public class PointHitCheck
    {
        public float dist;
        public AssemblerPoint point;
        public PointHitCheck(float hitDistance, AssemblerPoint hitPoint)
        {
            dist = hitDistance;
            point = hitPoint;
        }
    }
}