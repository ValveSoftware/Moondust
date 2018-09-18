using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assembler
{
    public class AssemblerPoint : MonoBehaviour
    {
        public float radius;

        public AssemblerPoint connected;

        public bool isConnected;

        public AssemblerComponent comp;

        public int ID;

        public bool connectable;


        private void Start()
        {
            SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.radius = radius;
        }
        
        private void Update()
        {
            if (isConnected)
            {
                if (connected.connected != this)
                {
                    DetachPoint();
                }
                if(connected== null)
                {
                    DetachPoint();
                }
            }
        }

        public void DetachPoint()
        {
            connected = null;
            isConnected = false;
        }

        public bool CheckForPoints(out PointHitCheck hit)
        {
            hit = null;

            Collider[] colliders = Physics.OverlapSphere(transform.position, radius * 2);
            foreach (Collider collider in colliders)
            {
                AssemblerPoint point = collider.GetComponent<AssemblerPoint>();
                if (point != null && point.isConnected == false) 
                {
                    if (point.ID == ID && point.comp != comp && (point.comp.connected || point.comp.core)) //attach conditions
                    {
                        hit = new PointHitCheck(Vector3.Distance(transform.position, point.transform.position), point);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}