using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Valve.VR.InteractionSystem.Sample;

public class MurderRace : Race
{
   

    public enum RaceStates
    {
        Inactive,
        WaitingForRace,
        Racing,
        Finished
    }

    public bool debug;

   // [HideInInspector]
    public RaceStates state;

    [Space()]
    [Header("Info")]

    public float raceTime = 60;

    public LeaderboardManager leaderboardManager;

    [Header("Main References")]

    [SerializeField]
    private GameObject inactiveTargetGO;

    [SerializeField]
    private GameObject raceInitiatorGO;

    [Space()]
   

    [SerializeField]
    private GameObject raceTitleGraphic;


    [Space()]

    [SerializeField]
    private GameObject raceConfirmerUIGO;
    [SerializeField]
    private GameObject raceCountdownGO;
    [SerializeField]
    private UnityEngine.UI.Text raceCountdownText;

    [Space()]

    [SerializeField]
    private GameObject raceTimerCanvas;
    [SerializeField]
    private UnityEngine.UI.Text raceScoreText;

    [SerializeField]
    private UnityEngine.UI.Text raceTimerText;
    [SerializeField]
    private UnityEngine.UI.Image raceTimerFill;

    [SerializeField]
    private GameObject raceCancelCanvas;

    [Space()]

    [SerializeField]
    private GameObject raceFinishedCanvas;
    [SerializeField]
    private UnityEngine.UI.Text raceFinishedScore;
    [SerializeField]
    private LeaderboardSingleEntryText raceFinishedPBText;
    [SerializeField]
    private LeaderboardSingleEntryText raceFinishedWRText;

    [Space()]

    [SerializeField]
    private GameObject statsGO;


    [SerializeField]
    private TeleportPoint teleportPoint;



    [Space()]
    

    [Header("Audio")]

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip au_countdown;
    [SerializeField]
    private AudioClip au_raceBegin;
    [SerializeField]
    private AudioClip au_checkpoint;
    [SerializeField]
    private AudioClip au_raceFinish;


    float timer;
    int score;

    private bool waitingForExit;


    private void Start()
    {
        ResetAll();
        JoeJeffCrowdSim.OnJoeJeffDeath.AddListener(JoeJeffKilled);
    }

    

    void ResetAll()
    {
        inactiveTargetGO.SetActive(true);
        raceInitiatorGO.SetActive(false);
        statsGO.SetActive(false);
        raceTimerCanvas.SetActive(false);
        raceFinishedCanvas.SetActive(false);
        raceTitleGraphic.SetActive(false);
        raceCancelCanvas.SetActive(false);
    }


    public void JoeJeffKilled()
    {
        if(state == RaceStates.Racing)
        {
            score++;
        }
    }


    float cancelTimer;

    const float standingRadius = 100;

    private void Update()
    {
        Player player = Player.instance;
        if(state == RaceStates.Inactive)
        {
            inactiveTargetGO.SetActive(activeRace == null);
            if((teleportPoint.transform.position - player.feetPositionGuess).sqrMagnitude < standingRadius)
            {
                if (!waitingForExit) // only start race if we're not waiting for the player to leave from a previous race
                {
                    // player has teleported to race, begin.
                    SwitchState(RaceStates.WaitingForRace);
                }
            }
            else
            {
                if (waitingForExit)
                {
                    waitingForExit = false;
                }
            }
        }
        else
        {
            if ((teleportPoint.transform.position - player.feetPositionGuess).sqrMagnitude > standingRadius)
                cancelTimer += Time.deltaTime;
            else
                cancelTimer = 0;

            if (cancelTimer > 0.5f)
            {
                // player has teleported away from race, cancel.
                StopAllCoroutines();
                SwitchState(RaceStates.Inactive);
                cancelTimer = 0;
            }
        }

        if(state == RaceStates.WaitingForRace)
        {

        }

        if (state == RaceStates.Racing)
        {
            timer -= Time.deltaTime;
            raceScoreText.text = "SCORE: " + score;
            raceTimerText.text = Mathf.Ceil(timer).ToString();
            raceTimerFill.fillAmount = timer / raceTime;
            if (timer <= 0)
            {
                PlaySound(au_raceFinish);
                FinishRace();
            }
        }

        if (state == RaceStates.Finished)
        {
            // highlight personal best if we just achieved it!
            raceFinishedPBText.highlighted = raceFinishedPBText.score == score;
            // highlight world record if we just achieved it!
            raceFinishedWRText.highlighted = raceFinishedWRText.score == score;
        }
        
    }

    public void RaceButtonPressed()
    {
        if(state == RaceStates.WaitingForRace)
        {
            raceConfirmerUIGO.SetActive(false);
            StartCoroutine(CountdownCoroutine());
        }
    }

    public void RaceFinishButtonRetry()
    {
        if (state == RaceStates.Finished)
        {
            SwitchState(RaceStates.WaitingForRace);

            RaceButtonPressed();
        }
    }
    

    public void RaceFinishButtonExit()
    {
        if (state == RaceStates.Finished)
        {
            SwitchState(RaceStates.Inactive);
        }
    }

    public void RaceFinishButtonCancelRace()
    {
        if (state == RaceStates.Racing)
        {
            // restart race
            SwitchState(RaceStates.WaitingForRace);
        }
    }

    IEnumerator CountdownCoroutine()
    {
        float countTime = 1f;
        raceCountdownGO.SetActive(true);

        raceCountdownText.text = "3";
        PlaySound(au_countdown);
        yield return new WaitForSeconds(countTime);


        raceCountdownText.text = "2";
        PlaySound(au_countdown);
        yield return new WaitForSeconds(countTime);

        raceCountdownText.text = "1";
        PlaySound(au_countdown);
        yield return new WaitForSeconds(countTime);

        PlaySound(au_raceBegin);
        BeginRace();
    }

    void BeginRace()
    {
        SwitchState(RaceStates.Racing);
        timer = raceTime;
        score = 0;
    }

    void FinishRace()
    {
        leaderboardManager.UploadScore(score);
        raceFinishedScore.text = score.ToString();
        SwitchState(RaceStates.Finished);
        Debug.Log("race complete!");
    }

    void CancelRace()
    {

    }


    // shitty pseudo state machine
    void SwitchState(RaceStates targetState)
    {
        DebugLog("switching from state " + state.ToString() + " to state " + targetState.ToString());

        if (state == targetState) return; // do nothing

        RaceStates sourceState = state;

        if (sourceState == RaceStates.Inactive)
        {
            inactiveTargetGO.SetActive(false);
            activeRace = this;
        }
        
        if (sourceState == RaceStates.WaitingForRace)
        {
            raceInitiatorGO.SetActive(false);
            raceCountdownGO.SetActive(false);
            raceConfirmerUIGO.SetActive(false);
        }

        if (sourceState == RaceStates.Racing)
        {
            raceTimerCanvas.SetActive(false);
            raceCancelCanvas.SetActive(false);
        }

        if (sourceState == RaceStates.Finished)
        {
            raceFinishedCanvas.SetActive(false);
        }

        // ==================================

        if (targetState == RaceStates.Inactive)
        {
            waitingForExit = true; // do not activate again until player has left and come back.
            inactiveTargetGO.SetActive(true);
            statsGO.SetActive(false);
            ResetAll();
            if (activeRace == this) activeRace = null;
        }

        

        if (targetState == RaceStates.WaitingForRace)
        {
            statsGO.SetActive(true);
            raceTitleGraphic.SetActive(true);
            raceInitiatorGO.SetActive(true);
            raceCountdownGO.SetActive(false);
            raceConfirmerUIGO.SetActive(true);
        }

        if (targetState == RaceStates.Racing)
        {
            raceTitleGraphic.SetActive(false);
            statsGO.SetActive(false);
            raceCancelCanvas.SetActive(true);
            raceTimerCanvas.SetActive(true);
        }

        if (targetState == RaceStates.Finished)
        {
            statsGO.SetActive(true);
            raceFinishedCanvas.SetActive(true);
        }

        state = targetState;
    }

    private void OnDestroy()
    {
        if (activeRace == this) activeRace = null;
        JoeJeffCrowdSim.OnJoeJeffDeath.RemoveListener(JoeJeffKilled);
    }

    //default to this position
    void PlaySound(AudioClip clip, float volume = 1)
    {
        PlaySound(clip, transform.position, volume);
    }

    void PlaySound(AudioClip clip, Vector3 position, float volume = 1)
    {
        audioSource.transform.position = position;
        audioSource.PlayOneShot(clip, volume);
    }




    void DebugLog(string log)
    {
        if (debug)
            Debug.Log(log);
    }
}
