using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelDust : MonoBehaviour {
    WheelCollider col;

    public ParticleSystem p;

    public float EmissionMul;

    public float velocityMul = 2;

    public float maxEmission;

    public float minSlip;

    [HideInInspector]
    public float amt;

    [HideInInspector]
    public Vector3 slip;

    float emitTimer;

    // Use this for initialization
    void Start () {
        col = GetComponent<WheelCollider>();
        StartCoroutine(emitter());
	}
	
	// Update is called once per frame
	void Update () {
        slip = Vector3.zero;
        if (col.isGrounded) {
            WheelHit hit;
            col.GetGroundHit(out hit);

            slip += Vector3.right * hit.sidewaysSlip;
            slip += Vector3.forward * -hit.forwardSlip;
            //print(slip);
        }
        amt = slip.magnitude;
        //print(amt);
	}

    IEnumerator emitter()
    {
        while (true)
        {
            while (emitTimer < 1)
            {
                yield return null;
                if (amt > minSlip)
                {
                    emitTimer += Mathf.Clamp((EmissionMul * amt), 0.01f, maxEmission);
                }
            }
            emitTimer = 0;
            DoEmit();
        }
    }

    void DoEmit()
    {
        // Any parameters we assign in emitParams will override the current system's when we call Emit.
        // Here we will override the start color and size.
        p.transform.rotation = Quaternion.LookRotation(transform.TransformDirection(slip));
        var m = p.main;
        m.startSpeed = velocityMul * amt;
        p.Emit(1);
    }
}
