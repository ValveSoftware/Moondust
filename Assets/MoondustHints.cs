using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class MoondustHints : MonoBehaviour
{
    // how many scenes have we been in
    static int sceneNum;

    public Hint hint_menu;

    public Hint hint_teleport;

    [System.Serializable]
    public class Hint
    {
        public SteamVR_Action_Boolean action;
        public string hintText;
        public Text text;
        [HideInInspector]
        public float startAlpha;

        private bool fading;

        public IEnumerator Init()
        {
            startAlpha = text.color.a;
            Hide(); // hidden by default

            yield return null; // wait a frame for actions to be ready

            string buttonName = action.GetLocalizedOriginPart(SteamVR_Input_Sources.RightHand, EVRInputStringBits.VRInputString_InputSource);
            text.text = string.Format(hintText, buttonName);
        }
        public void Show()
        {
            text.gameObject.SetActive(true);
        }
        public void Hide()
        {
            text.gameObject.SetActive(false);
        }
        public IEnumerator FadeIn(float time = 1)
        {
            yield return new WaitWhile(() => fading);
            text.CrossFadeAlpha(0, 0, true);
            Show();
            fading = true;
            text.CrossFadeAlpha(startAlpha, time, false);
            yield return new WaitForSeconds(time);
            fading = false;
        }

        public IEnumerator FadeOut(float time = 1)
        {
            text.CrossFadeAlpha(startAlpha, 0, true);
            yield return new WaitWhile(() => fading);
            fading = true;
            text.CrossFadeAlpha(0, time, false);
            yield return new WaitForSeconds(time);
            fading = false;
            Hide();
        }
    }

    private void Start()
    {
        StartCoroutine(hint_menu.Init());
        StartCoroutine(hint_teleport.Init());

        if(sceneNum == 0)
        {
            StartCoroutine(ShowHint(hint_menu, 10, 40));
        }
        sceneNum++;

        if(Valve.VR.InteractionSystem.Teleport.instance != null)
        {
            StartCoroutine(ShowHint(hint_teleport, 5, 40));
        }
    }

    bool showingHint;

    IEnumerator ShowHint(Hint hint, float delay, float showTime)
    {
        showingHint = true;
        yield return new WaitForSeconds(delay);
        StartCoroutine(hint.FadeIn());
        float t = 0;
        bool hintDone = false;
        while(!hintDone)
        {
            t += Time.deltaTime;
            yield return null;
            if(t> showTime || hint.action.GetState(SteamVR_Input_Sources.Any))
            {
                hintDone = true;
            }
            showingHint = true;
        }
        StartCoroutine(hint.FadeOut());
        yield return new WaitForSeconds(1);
        showingHint = false;
    }

}
