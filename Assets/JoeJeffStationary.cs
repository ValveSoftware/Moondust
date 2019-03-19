using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoeJeffStationary : MonoBehaviour
{
    Animator anim;
    Rigidbody rigidbody;

    public JoeJeffRagdoll ragdollPrefab;
    public RandomSkinColor skinColor;

    public Transform skeletonRoot;

    public bool dieOnDrop = true;


    private bool holding;
    private bool inAir;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        rigidbody.isKinematic = true;
        rigidbody.freezeRotation = true;

        //set animator time to random to avoid in-sync animations
        int stateName = anim.GetCurrentAnimatorStateInfo(0).shortNameHash;
        anim.Play(stateName, 0, Random.value);
        anim.speed = Random.Range(0.9f, 1.1f);
        anim.applyRootMotion = false;

    }

    const float killMass = 100;

    public void OnCollisionEnter(Collision collision)
    {
        // cannot die while being held
        if (!holding)
        {
            if (inAir)
            {
                //is falling
                if (dieOnDrop)
                {
                    Die();
                }
            }
            else
            {
                //has been hit with something
                if (collision.rigidbody != null)
                {
                    if (collision.rigidbody.mass > killMass)
                    {
                        Die(collision.relativeVelocity, collision.contacts[0].point);
                    }
                }
            }
        }
    }

    private void Update()
    {
        UpdateAnimator();
    }

    void UpdateAnimator()
    {
        anim.SetBool("OnGround", !inAir);
        if (inAir)
        {
            anim.SetFloat("Jump", rigidbody.velocity.y);
        }
    }


    public void Explosion(ExplosionForcer ex)
    {
        float power = 5;
        Vector3 forceVector = (transform.position - ex.transform.position).normalized + Vector3.up;
        float forcePower = Mathf.Lerp(power * 1.5f, power * 0.25f, Mathf.InverseLerp(ex.radius / 5, ex.radius, (transform.position - ex.transform.position).magnitude));
        Die(forceVector * forcePower, ex.transform.position);
    }

    public void Pickup()
    {
        Ascend();
        holding = true;
        anim.SetBool("Holding", true);
    }

    public void Ascend(bool freeRotation = true)
    {
        //find references in case this gets called right as I'm spawned
        if (rigidbody == null) rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.freezeRotation = !freeRotation;
        inAir = true;
    }

    public void Throw()
    {
        anim.SetBool("Holding", false);
        holding = false;
    }


    public void Die()
    {
        Die(Vector3.zero, Vector3.zero);
    }

    private bool hasDied;

    /// <summary>
    /// Die and spawn a ragdoll
    /// </summary>
    public void Die(Vector3 forceVector, Vector3 forcePoint)
    {
        if (hasDied) return; // ensure only one ragdoll is spawned.
        hasDied = true;
        JoeJeffRagdoll rag = Instantiate(ragdollPrefab.gameObject, transform.position, transform.rotation).GetComponent<JoeJeffRagdoll>();
        rag.Ragdoll(transform, skeletonRoot, skinColor.skinColor, rigidbody.velocity, forceVector, forcePoint);
        Destroy(gameObject);
    }
}
