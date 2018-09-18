using UnityEngine;
using System.Collections;
using Valve.VR;

public class AmbientSound : MonoBehaviour
{
    public float fadeintime;
    public bool fadeblack = false;

    private float volume;
    private AudioSource audiosource;
    private float fadeTime;

    private void Start()
    {
        AudioListener.volume = 1;
        audiosource = GetComponent<AudioSource>();
        audiosource.time = Random.Range(0, audiosource.clip.length);
        if (fadeintime > 0)
            fadeTime = 0;

        volume = audiosource.volume;

        SteamVR_Fade.Start(Color.black, 0);
        SteamVR_Fade.Start(Color.clear, 7);
    }

    private void Update()
    {
        if (fadeintime > 0 && fadeTime < 1)
        {
            fadeTime += Time.deltaTime / fadeintime;
            audiosource.volume = fadeTime * volume;
        }
    }
}