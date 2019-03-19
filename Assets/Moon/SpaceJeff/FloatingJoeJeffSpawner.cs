using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingJoeJeffSpawner : MonoBehaviour
{
    public GameObject JoeJeffPrefab;
    public Transform[] spawnPoints;
    [Range(0,1)]
    public float spawnProbability;

    private void Start()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if(Random.value < spawnProbability)
            {
                GameObject instance = Instantiate(JoeJeffPrefab, transform);
                instance.transform.position = spawnPoints[i].position;
            }
        }
    }
}
