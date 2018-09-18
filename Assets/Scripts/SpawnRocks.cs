using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRocks : MonoBehaviour
{
    public GameObject[] Rocks;

    public bool initial;

    public float time;

    [HideInInspector]
    public bool go = true;
    

    private void Start()
    {
        if (time != 0)
        {
            StartCoroutine(DoSpawn());
        }

        if (initial)
        {
            Vector3 rockPosition = transform.position + Vector3.up * 2;
            Instantiate(Rocks[Random.Range(0, Rocks.Length)], rockPosition, Random.rotation);
        }
    }

    private IEnumerator DoSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(time * Random.Range(0.7f, 1.4f));
            while (!go)
            {
                yield return null;
            }
            Instantiate(Rocks[Random.Range(0, Rocks.Length)], transform.position, Random.rotation);
            yield return null;
        }
    }
}