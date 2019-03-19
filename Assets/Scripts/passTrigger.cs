using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class passTrigger : MonoBehaviour
{
    public GameObject target;

    void OnTriggerEnter(Collider collider)
    {
        target.SendMessage("OnTriggerEnter", collider, SendMessageOptions.DontRequireReceiver);
    }

    void OnTriggerExit(Collider collider)
    {
        target.SendMessage("OnTriggerExit", collider, SendMessageOptions.DontRequireReceiver);
    }

    void OnTriggerStay(Collider collider)
    {
        target.SendMessage("OnTriggerStay", collider, SendMessageOptions.DontRequireReceiver);
    }
}