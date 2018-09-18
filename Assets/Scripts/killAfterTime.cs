using UnityEngine;
using System.Collections;

public class killAfterTime : MonoBehaviour
{
    public float timeToDie;

    private float timer;

    
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeToDie)
        {
            Kill();
        }
    }
    
    private void Kill()
    {
        Destroy(gameObject);
    }

    public void ResetTimer()
    {
        timer = 0;
    }
}