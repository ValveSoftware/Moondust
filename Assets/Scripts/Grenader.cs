using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Grenader : MonoBehaviour
{
    public HoverButton vrbutton;

    public GameObject grenadePrefab;
    public GameObject TurretPrefab;
    public ResetPortal port;

    public float portForce;

    bool spawning;

    float numGrenades;

    public int minGrenades;
    public int maxGrenades;

    public Transform triggerzone;
    public float triggerRadius;
    
    private void Start()
    {
        StartCoroutine(DoCheckGrenadesPresence());
    }

    private void Update()
    {
        if ((vrbutton.engaged || Input.GetKeyDown(KeyCode.Space) || numGrenades < minGrenades) & !spawning && numGrenades <= maxGrenades)
        {
            spawning = true;
            StartCoroutine(DoSpawnNades(1, 0.6f));
        }
    }

    private IEnumerator DoSpawnNades(int numberToSpawn, float timeBetweenSpawns)
    {
        port.shootForce = portForce * (-Physics.gravity.y+3);
        for (int spawnIndex = 0; spawnIndex < numberToSpawn; spawnIndex++)
        {
            GameObject spawnPrefab;

            if (Random.value < 0.75f)
                spawnPrefab = grenadePrefab;
            else
                spawnPrefab = TurretPrefab;

            GameObject spawnedObject = Instantiate(spawnPrefab, Vector3.down * 1000, Quaternion.identity);

            port.resetter = spawnedObject.GetComponent<Rigidbody>();
            port.ResetNow();

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        spawning = false;
    }

    private IEnumerator DoCheckGrenadesPresence()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            Collider[] overlappingColliders = Physics.OverlapSphere(triggerzone.position, triggerRadius);
            List<Rigidbody> grenadeList = new List<Rigidbody>();
            foreach (Collider collider in overlappingColliders)
            {
                Rigidbody attachedRigidbody = collider.attachedRigidbody;

                if (attachedRigidbody != null)
                {
                    if (!grenadeList.Contains(attachedRigidbody))
                    {
                        if (attachedRigidbody.GetComponent<grenade>() != null)
                        {
                            grenadeList.Add(attachedRigidbody);
                        }
                    }
                }
            }

            numGrenades = grenadeList.Count;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(triggerzone.position, triggerRadius);
    }
}