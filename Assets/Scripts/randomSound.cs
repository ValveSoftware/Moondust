using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class randomSound : MonoBehaviour
{
    public Vector2 pitchRange = Vector2.one;
    public Vector2 volRange = Vector2.one;

    public AudioClip[] clips;

    public bool playOnAwake;


    private AudioSource au;

    private void Awake()
    {
        au = GetComponent<AudioSource>();

        if (playOnAwake)
            Play();
    }

    public void Play()
    {
        au.pitch = Random.Range(pitchRange.x, pitchRange.y);
        au.PlayOneShot(clips[Random.Range(0, clips.Length)], Random.Range(volRange.x, volRange.y));
    }
}