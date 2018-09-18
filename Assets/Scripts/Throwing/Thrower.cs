using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thrower : MonoBehaviour
{
    public static Thrower instance;

    public GameObject throwPrefab;

    public Transform throwPoint;


    public float rate;

    public Vector3 throwForce;

    public Vector3 thisThrowForce;

    public List<Thrown> thrownList = new List<Thrown>();

    private void Awake()
    {
        instance = this;

        StartCoroutine(DoThrowing());
    }

    private IEnumerator DoThrowing()
    {
        while (true)
        {
            yield return new WaitForSeconds(rate);

            GameObject thrownGameObject = GameObject.Instantiate(throwPrefab);
            GameObject.Destroy(thrownGameObject, 15f);

            Thrown thrown = thrownGameObject.GetComponent<Thrown>();

            thrown.rigidbody.position = throwPoint.position;
            thrown.rigidbody.rotation = throwPoint.rotation;

            yield return null;

            thrown.rigidbody.AddRelativeForce(throwForce);

            thrownList.Add(thrown);
        }
    }
}
