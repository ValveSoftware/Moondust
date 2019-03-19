using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JoeJeffCrowdSim : MonoBehaviour
{

    public static JoeJeffCrowdSim instance;

    public GameObject crowdEntityPrefab;

    public int crowdSize = 50;

    public float respawnVelocity = 5f;

    public List<CrowdEntity> crowd = new List<CrowdEntity>();

    public List<CrowdTarget> targets = new List<CrowdTarget>();

    public Bounds spawnVolume;
    public Bounds wanderVolume;

    public static UnityEngine.Events.UnityEvent OnJoeJeffDeath = new UnityEngine.Events.UnityEvent();

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnVolume.center, spawnVolume.size);
        Gizmos.color = Color.grey;
        Gizmos.DrawWireCube(wanderVolume.center, wanderVolume.size);
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for (int spawnIndex = 0; spawnIndex < crowdSize; spawnIndex++)
        {
            SpawnJoeJeff();
        }

        StartCoroutine(Simulate());
    }

    public static void RegisterCrowdTarget(CrowdTarget t)
    {
        if (instance == null)
        {
            Debug.LogWarning("No Crowd Sim in scene, cannot register crowd target");
            return;
        }

        instance.targets.Add(t);
    }
    public static void DeRegisterCrowdTarget(CrowdTarget t)
    {
        if (instance == null)
        {
            Debug.LogWarning("No Crowd Sim in scene, cannot deregister crowd target");
            return;
        }

        instance.targets.Remove(t);
    }

    public static void RegisterJoeJeffDeath(JoeJeffAgent jj)
    {
        OnJoeJeffDeath.Invoke();
    }

    

    public void SpawnFromPortal(Vector3 startPosition, Vector3 up)
    {
        GameObject newJoeJeff = Instantiate<GameObject>(crowdEntityPrefab.gameObject, startPosition, Quaternion.Euler(0, Random.value * 360, 0));

        Rigidbody newJeffRigidbody = newJoeJeff.GetComponent<Rigidbody>();
        newJeffRigidbody.detectCollisions = false;

        JoeJeffAgent newJoeJeffAgent = newJoeJeff.GetComponent<JoeJeffAgent>();
        newJoeJeffAgent.Ascend(false); // tell the joejeff he's getting thrown into the air

        Vector3 portalVector = up;
        portalVector.x += Random.value * 0.2f;
        portalVector.z += Random.value * 0.2f;
        newJeffRigidbody.velocity = portalVector * respawnVelocity;

        CrowdEntity crowdEntity = newJoeJeff.GetComponent<CrowdEntity>();
        crowdEntity.crowd = this;
        crowd.Add(crowdEntity);

        StartCoroutine(DoEnableJeffCollider(newJeffRigidbody, 0.5f));
    }

    private IEnumerator DoEnableJeffCollider(Rigidbody newJeffRigidbody, float afterTime)
    {
        yield return new WaitForSeconds(afterTime);
        newJeffRigidbody.detectCollisions = true;
    }

    void SpawnJoeJeff()
    {
        // spawn new JJ at random spot
        Vector3 position = SampleNavMesh(RandomPointInBounds(spawnVolume));
        CrowdEntity crowdEntity = Instantiate(crowdEntityPrefab.gameObject, position, Quaternion.Euler(0, Random.value * 360, 0)).GetComponent<CrowdEntity>();
        crowdEntity.crowd = this;
        crowd.Add(crowdEntity);
    }

    const float wanderDistance = 4;
    const float wanderTime = 5f;
    const int ticksPerFrame = 20;
    private IEnumerator Simulate()
    {
        int ticker = 0;
        while (true)
        {
            for(int i = crowd.Count - 1; i >= 0; i--)
            {
                if (crowd[i] == null)
                {
                    // this one has died or otherwise been deleted, remove it from list
                    crowd.RemoveAt(i);
                }
                else
                {
                    if (crowd[i].wandering && (crowd[i].currentTarget == null))
                    {
                        if (Time.time - crowd[i].prevWanderTime > crowd[i].nextWanderTime)
                        {
                            // wander to new nearby spot and setup next wander event
                            crowd[i].WanderEvent(GetRandomWanderTarget(crowd[i].nav.transform.position, wanderDistance * Random.Range(0.2f, 1.5f)), Random.value * wanderTime);
                        }
                        CheckEntityTargets(crowd[i]);
                    }
                    else
                    {
                        //not wandering
                    }
                }
                ticker++;
                if (ticker >= ticksPerFrame) { ticker = 0; yield return null; }
            }
            yield return null;
        }
    }

    void CheckEntityTargets(CrowdEntity ent)
    {
        float currentTargetDist = 1000;
        if (ent.currentTarget != null)
        {
            currentTargetDist = SqrDist(ent.transform.position, ent.currentTarget.transform.position);
        }

        for(int i = 0; i < targets.Count; i++)
        {
            if (targets[i].active == false) continue; // ignore this one, it's inactive
            //if target in range
            if(CheckRange(ent.transform.position, targets[i].transform.position, targets[i].radius))
            {
                //if closer than current target
                if(SqrDist(ent.transform.position, targets[i].transform.position) < currentTargetDist)
                {
                    //make this the current target
                    ent.currentTarget = targets[i];
                }
            }
        }
    }

    // random spot on nav mesh
    public Vector3 GetRandomWanderTarget()
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(RandomPointInBounds(wanderVolume), out hit, 100, NavMesh.AllAreas);
        return hit.position;
    }
    //random spot within a certain distance of current pos
    public Vector3 GetRandomWanderTarget(Vector3 currentPos, float maxDist)
    {
        Vector3 offset = RandomPointInBounds(wanderVolume) - currentPos;
        offset = Vector3.ClampMagnitude(offset, maxDist);
        return SampleNavMesh(currentPos + offset);
    }

    public static bool CheckRange(Vector3 a, Vector3 b, float range)
    {
        return (a - b).sqrMagnitude < range * range;
    }

    public static float SqrDist(Vector3 a, Vector3 b)
    {
        return (a - b).sqrMagnitude;
    }

    public Vector3 SampleNavMesh(Vector3 pos, float maxDist = 100)
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(pos, out hit, maxDist, NavMesh.AllAreas);
        return hit.position;
    }

    /// <summary>
    /// Random point in a bounding box
    /// </summary>
    Vector3 RandomPointInBounds(Bounds b)
    {
        return new Vector3(Random.Range(b.min.x, b.max.x), Random.Range(b.min.y, b.max.y), Random.Range(b.min.z, b.max.z));
    }
}
