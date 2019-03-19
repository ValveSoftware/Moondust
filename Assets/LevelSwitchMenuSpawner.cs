using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class LevelSwitchMenuSpawner : MonoBehaviour
{
    public MainMenu menu;
    Animator anim;
    public Transform menuCenter;

    bool menuOpen;
    bool animating;

    public Player player;

    public SteamVR_Action_Boolean action_menu;

    public float spawnDistance = 0.7f;

    public AudioClip au_menuOpen;
    public AudioClip au_menuClose;
    AudioSource audioSource;

    private void Start()
    {
        anim = menu.GetComponent<Animator>();
        menu.gameObject.SetActive(false);
        menu.interactable = false;

        audioSource = new GameObject("menuAudio").AddComponent<AudioSource>();
        audioSource.transform.parent = transform;
        audioSource.spatialBlend = 1;
        audioSource.spatialize = true;
        audioSource.minDistance = 100;
        audioSource.dopplerLevel = 0;
    }


    private void Update()
    {
        if (!animating & !menu.loading)
        {
            if (!menuOpen)
            {
                if (player.leftHand.currentAttachedObject == null && action_menu.GetStateDown(SteamVR_Input_Sources.LeftHand))
                {
                    OpenMenu();
                }
                if (player.rightHand.currentAttachedObject == null && action_menu.GetStateDown(SteamVR_Input_Sources.RightHand))
                {
                    OpenMenu();
                }
            }
            else
            {
                // if menu button is pressed or player looks away from menu
                if (action_menu.GetStateDown(SteamVR_Input_Sources.Any) || Vector3.Dot(player.hmdTransform.forward, (menuCenter.position - player.hmdTransform.position).normalized) < 0.2)
                {
                    CloseMenu();
                }
            }
        }
    }

    public void OpenMenu()
    {
        Vector3 lookDir = player.hmdTransform.forward;
        lookDir.y = 0;
        lookDir = lookDir.normalized;

        // spawn in front of player
        Vector3 spawnPos = player.feetPositionGuess + lookDir * player.scale * spawnDistance;

        menu.transform.position = spawnPos;
        menu.gameObject.SetActive(true);
        menuOpen = true;
        menu.interactable = true;

        audioSource.transform.position = menuCenter.position;
        audioSource.PlayOneShot(au_menuOpen);
        StartCoroutine(AnimMenuOpen());
    }

    IEnumerator AnimMenuOpen()
    {
        animating = true;
        yield return new WaitForSeconds(0.5f);
        animating = false;
    }

    public void CloseMenu()
    {
        menu.gameObject.SetActive(false);
        menu.interactable = false;

        menuOpen = false;
        audioSource.PlayOneShot(au_menuClose);
        StartCoroutine(AnimMenuClose());
    }

    IEnumerator AnimMenuClose()
    {
        animating = true;
        yield return new WaitForSeconds(0.5f);
        animating = false;
    }
}
