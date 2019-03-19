using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Valve.VR;

public class MainMenu : MonoBehaviour
{
    Animator anim;
    [System.Serializable]
    public class Level
    {
        public string name;
        public GameObject portalGO;

        public Level(string name, GameObject portalGO)
        {
            this.name = name;
            this.portalGO = portalGO;
        }
    }
    public Level[] levels;

    public ParticleSystem portalParticles;

    public AudioSource audio;

    // shorter variation
    public bool abridged;

    // let other scripts set whether menu can be interacted with
    [HideInInspector]
    public bool interactable = true;

    private void Start()
    {
        anim = GetComponent<Animator>();
        //hide all portals
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].portalGO.SetActive(false);
        }
    }

    Level GetLevelByName(string name)
    {
        return Array.Find(levels, l => l.name == name);
    }

    [HideInInspector]
    public bool loading;

    public void LoadLevel(string name)
    {
        if (loading) Debug.LogError("already loading");
        if (!interactable) Debug.LogError("not interactable");
        if (!loading && interactable)
        {
            loading = true;
            Level l = GetLevelByName(name);
            if (abridged)
            {
                StartCoroutine(DoLoadLevelAbridged(l));
            }
            else
            {
                StartCoroutine(DoLoadLevel(l));
            }
            StartCoroutine(FadeMusic());
        }
    }

    public IEnumerator DoLoadLevel(Level l)
    {
        audio.Play();
        anim.SetTrigger("LoadLevel");
        //show portal for level
        l.portalGO.SetActive(true);
        yield return new WaitForSeconds(2);
        portalParticles.Play();
        yield return new WaitForSeconds(2.4f);
        SteamVR_Fade.Start(Color.black, 0.4f,true);
        yield return new WaitForSeconds(0.5f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(l.name);
    }

    //shorter variation
    public IEnumerator DoLoadLevelAbridged(Level l)
    {
        audio.Play();
        anim.SetTrigger("LoadLevel");
        //show portal for level
        l.portalGO.SetActive(true);
        yield return new WaitForSeconds(1);
        portalParticles.Play();
        yield return new WaitForSeconds(1.6f);
        SteamVR_Fade.Start(Color.black, 0.4f, true);
        yield return new WaitForSeconds(0.5f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(l.name);
    }

    public IEnumerator FadeMusic()
    {
        AudioSource music;

        try
        {
            music = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();
        }
        catch
        {
            //music null, nothing to fade.
            yield break;
        }

        float svol = music.volume;

        for(float vol = 1; vol >= 0; vol -= Time.deltaTime)
        {
            music.volume = svol * vol;
            yield return null;
        }

        music.volume = 0;
    }
}
