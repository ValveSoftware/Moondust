using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JoeJeffAgent : MonoBehaviour
{
    public RandomSkinColor skinColor;

    public JoeJeffRagdoll ragdollPrefab;


    [SerializeField]
    private float m_MovingTurnSpeed = 360;
    [SerializeField]
    private float m_StationaryTurnSpeed = 180;

    public Valve.VR.InteractionSystem.FireSource fire;


    private NavMeshAgent nav;

    private new Rigidbody rigidbody;

    private CrowdEntity crowdEntity;

    public float fleeSpeed = 1;

    private Animator anim;

    public JoeJeffCarrier carrier;

    [HideInInspector]
    public bool holding;
    [HideInInspector]
    public bool inAir;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        crowdEntity = GetComponent<CrowdEntity>();
        skinColor = GetComponent<RandomSkinColor>();
    }

    private void Update()
    {
        UpdateAnimator(nav.desiredVelocity);

        // wander when not carrying something
        crowdEntity.wandering = carrier.carrying == false;
        if(carrier.carrying && !fetching)
        {
            StartCoroutine(FetchToPlayer());
        }
    }

    private bool fetching = false;
    private IEnumerator FetchToPlayer()
    {
        fetching = true;
        yield return new WaitForSeconds(1);
        int oldPriority = nav.avoidancePriority;
        nav.avoidancePriority = 0; // be able to push through other JoeJeffs
        Vector3 target = FetchTarget.Find().position;
        // walk to fetch target
        nav.SetDestination(target);
        yield return new WaitForSeconds(1);
        // yield until reaches target
        while((target - nav.transform.position).sqrMagnitude > 0.03f)
        {
            yield return null;
        }
        // look at player
        useCustomLookDir = true;
        customLookDir = Camera.main.transform.position - transform.position;
        customLookDir.y = 0;
        yield return new WaitForSeconds(1);

        //set up  throw force to reach a certain height in the current gravity
        float throwHeight = 1f;
        float throwForce = Mathf.Sqrt(2 * -Physics.gravity.y * throwHeight);
        Vector3 throwVel = new Vector3(0, throwForce, 0.5f);
        throwVel = transform.rotation * throwVel;
        if (carrier.carrying.GetComponent<CrowdTarget>() != null)
        {
            //disable crowd target so I don't immediately chase this thing again
            carrier.carrying.GetComponent<CrowdTarget>().active = false;
        }
        carrier.Throw(throwVel);
        yield return new WaitForSeconds(0.5f);
        useCustomLookDir = false;
        nav.avoidancePriority = oldPriority;
        fetching = false;
    }


    private bool useCustomLookDir = false;
    private Vector3 customLookDir;


    //NOTE : Could be optimized by caching strings to hashes.
    private void UpdateAnimator(Vector3 move)
    {
        if (move.magnitude > 1f)
            move.Normalize();

        move = transform.InverseTransformDirection(move);

        float turnAmount = Mathf.Atan2(move.x, move.z);
        float forwardAmount = (move.z + move.magnitude) / 2;

        if (useCustomLookDir)
        {
            //override movement-based turning 
            Vector3 look = transform.InverseTransformDirection(customLookDir);
            turnAmount = Mathf.Atan2(look.x, look.z);
        }

        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, forwardAmount);
        transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);

        //anim.speed = fire.isBurning ? animationSpeed * 2 : animationSpeed;
        // update the animator parameters
        anim.SetFloat("Forward", fire.isBurning ? 2 : forwardAmount, 0.1f, Time.deltaTime);
        anim.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
        anim.SetBool("OnGround", !inAir);
        //anim.SetBool("Holding", held);

        float speedMultiplier = 1;
        if (crowdEntity.Fleeing) speedMultiplier = fleeSpeed;
        anim.SetFloat("SpeedMultiplier", speedMultiplier);

        if (inAir)
        {
            anim.SetFloat("Jump", rigidbody.velocity.y);
            anim.SetFloat("FallSpeed", Mathf.Abs(rigidbody.velocity.y));
        }
    }


    private void OnAnimatorMove()
    {
        if (nav != null && anim != null)
            nav.velocity = anim.velocity;
    }

    public void Explosion(ExplosionForcer ex)
    {
        float power = 5;
        Vector3 forceVector = (transform.position - ex.transform.position).normalized + Vector3.up;
        float forcePower = Mathf.Lerp(power*1.5f, power * 0.25f, Mathf.InverseLerp(ex.radius / 5, ex.radius, (transform.position - ex.transform.position).magnitude));
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
        if (nav == null) nav = GetComponent<NavMeshAgent>();
        rigidbody.isKinematic = false;
        nav.enabled = false;
        rigidbody.freezeRotation = !freeRotation;
        inAir = true;
    }

    public void Throw()
    {
        anim.SetBool("Holding", false);
        holding = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(inAir && !holding)
        {
            if(collision.rigidbody == null)
            {
                //is probably a solid object then
                NavMeshHit hit;
                if(NavMesh.SamplePosition(transform.position, out hit, 3.0f, NavMesh.AllAreas))
                {
                    transform.position = hit.position;
                    Land();
                }
            }
        }
    }

    public void Land()
    {
        Vector3 rot = transform.eulerAngles;
        rot.x = 0;
        rot.z = 0;
        transform.eulerAngles = rot;
        rigidbody.isKinematic = true;
        nav.enabled = true;
        rigidbody.freezeRotation = true;
        inAir = false;
    }



    public Transform skeletonRoot;
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
        JoeJeffCrowdSim.RegisterJoeJeffDeath(this);
        JoeJeffRagdoll rag = Instantiate(ragdollPrefab.gameObject, transform.position, transform.rotation).GetComponent<JoeJeffRagdoll>();
        rag.Ragdoll(transform, skeletonRoot, skinColor.skinColor, rigidbody.velocity, forceVector, forcePoint);
        Destroy(gameObject);
    }
}
