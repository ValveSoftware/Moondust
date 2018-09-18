using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevatorRandom : MonoBehaviour
{
    public Vector2 positions;

    public float speed;

    public float waitAverage;

    private float position;
    private float distance;

    private void Start()
    {
        speed *= Random.Range(0.60f, 1.50f);
        distance = Mathf.Abs(positions.y - positions.x);
        position = Mathf.InverseLerp(positions.x, positions.y, transform.localPosition.y);
        StartCoroutine(DoMove());
    }
    
    private void Update()
    {
        Vector3 p = transform.localPosition;
        p.y = Mathf.Lerp(positions.x, positions.y, position);
        transform.localPosition = p;
    }

    private IEnumerator DoMove()
    {
        yield return new WaitForSeconds(waitAverage * Random.Range(0.20f, 5f));
        while (position < 1)
        {
            position += Time.deltaTime * speed / distance;
            yield return null;
        }
        position = 1;
        yield return new WaitForSeconds(waitAverage * Random.Range(0.20f, 5f));
        while (position > 0)
        {
            position -= Time.deltaTime * speed / distance;
            yield return null;
        }
        position = 0;
    }
}