using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class passTrigger : MonoBehaviour
{
    public GameObject target;

    void OnTriggerEnter(Collider collider)
    {
        target.SendMessage("OnTriggerEnter", collider);
    }

    void OnTriggerExit(Collider collider)
    {
        target.SendMessage("OnTriggerExit", collider);
    }

    void OnTriggerStay(Collider collider)
    {
        target.SendMessage("OnTriggerStay", collider);
    }
}