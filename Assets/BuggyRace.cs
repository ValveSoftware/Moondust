using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Valve.VR.InteractionSystem.Sample;
using System;

public class BuggyRace : Race
{

    public enum RaceStates
    {
        Inactive,
        WaitingForBuggy,
        WaitingForRace,
        Racing,
        Finished,
        Cancelled
    }

    public bool debug;

   // [HideInInspector]
    public RaceStates state;

    [Space()]
    [Header("Info")]
    public string trackName = "new track";

    public LeaderboardManager leaderboardManager;

    [Header("Main References")]

    [SerializeField]
    private GameObject inactiveTargetGO;

    [SerializeField]
    private GameObject buggyRequestGO;

    [SerializeField]
    private GameObject raceInitiatorGO;

    [Space()]

    [SerializeField]
    private UnityEngine.UI.Text raceTitleInactive;

    [SerializeField]
    private UnityEngine.UI.Text raceTitleLeaderboard;

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
    private UnityEngine.UI.Text raceTimerText;

    [SerializeField]
    private GameObject raceCancelCanvas;

    [Space()]

    [SerializeField]
    private GameObject raceFinishedCanvas;
    [SerializeField]
    private UnityEngine.UI.Text raceFinishedTimeText;
    [SerializeField]
    private LeaderboardSingleEntryText raceFinishedPBText;
    [SerializeField]
    private LeaderboardSingleEntryText raceFinishedWRText;

    [SerializeField]
    private GameObject raceCancelledCanvas;

    [Space()]

    [SerializeField]
    private GameObject statsGO;

    [SerializeField]
    private GameObject raceTrackGO;

    [SerializeField]
    private TeleportPoint teleportPoint;



    [Space()]
    [Header("Racetrack")]

    [Tooltip("Checkpoints are children of this")]
    [SerializeField]
    public GameObject checkpointContainer;

    [HideInInspector]
    public RaceCheckpoint[] checkpoints;

    public RaceCheckpoint finishLine;



    [HideInInspector]
    public int activeCheckpoint = 0;


    [HideInInspector]
    public BuggyBuddy buggy;

    [SerializeField]
    private Transform buggyStartPosition;

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
    int timescore;

    private bool waitingForExit;


    private class BuggySystem
    {
        public BuggyControllerMoon controller;
        public BuggyBuddy buggy;

        public BuggySystem(BuggyControllerMoon controller, BuggyBuddy buggy)
        {
            this.controller = controller;
            this.buggy = buggy;
        }
    }

    BuggySystem[] buggies;
    BuggyBuddy GetBuggyByController(BuggyControllerMoon c)
    {
        return Array.Find(buggies, b => b.controller == c).buggy;
    }
    BuggyControllerMoon GetControllerByBuggy(BuggyBuddy b)
    {
        return Array.Find(buggies, c => c.buggy == b).controller;
    }
    int CurrentBuggyIndex()
    {
        return Array.FindIndex(buggies, b => b.buggy == buggy);
    }


    private void Start()
    {
        ResetAll();

        checkpoints = checkpointContainer.GetComponentsInChildren<RaceCheckpoint>();

        raceTitleInactive.text = trackName;
        raceTitleLeaderboard.text = trackName;

        // make a list of controllers and associated buggies
        BuggyControllerMoon[] controllers = FindObjectsOfType<BuggyControllerMoon>();
        buggies = new BuggySystem[controllers.Length];
        for (int i = 0; i < controllers.Length; i++)
        {
            buggies[i] = new BuggySystem(controllers[i], controllers[i].buggy);
        }
    }

    void ResetAll()
    {
        inactiveTargetGO.SetActive(true);
        buggyRequestGO.SetActive(false);
        raceInitiatorGO.SetActive(false);
        statsGO.SetActive(false);
        raceTrackGO.SetActive(false);
        raceTimerCanvas.SetActive(false);
        raceCancelCanvas.SetActive(false);
        raceFinishedCanvas.SetActive(false);
        raceCancelledCanvas.SetActive(false);
        raceTitleGraphic.SetActive(false);
        activeCheckpoint = 0;
    }



    public void ActivateForBuggy(BuggyBuddy selectedBuggy)
    {
        buggy = selectedBuggy;

        ClearStartArea();

        GetControllerByBuggy(buggy).resetter.CancelReset(); // cancel reset process in case buggy is in the middle of dying

        buggy.transform.position = buggyStartPosition.position;
        buggy.transform.rotation = buggyStartPosition.rotation;
        buggy.body.velocity = Vector3.zero;
        buggy.body.angularVelocity = Vector3.zero;

        SwitchState(RaceStates.WaitingForRace);
        //StartCoroutine(TeleportPlayerToTransform(playerStandingPosition));
    }

    void ClearStartArea()
    {
        // find any other buggies in start area and reset them out of the way.

        Collider[] cols = Physics.OverlapSphere(buggyStartPosition.position, 5);
        List<BuggyBuddy> buggiesCleared = new List<BuggyBuddy>();
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].attachedRigidbody != null)
            {
                BuggyBuddy b = cols[i].attachedRigidbody.GetComponent<BuggyBuddy>();
                if (b != null)
                {
                    if (!buggiesCleared.Contains(b) && b != buggy)
                    {
                        buggiesCleared.Add(b);
                        GetControllerByBuggy(b).resetter.ResetBuggy();
                    }
                }
            }
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
                    SwitchState(RaceStates.WaitingForBuggy);
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

        if(state == RaceStates.WaitingForBuggy)
        {
            BuggySystem holdingController = CheckHoldingControllers();
            if (holdingController != null)
            {
                ActivateForBuggy(holdingController.buggy);
            }
        }

        if(state == RaceStates.WaitingForRace)
        {
            FreezeBuggy();
           // activeCheckpoint = 0;
            //UpdateCheckpoints();
        }

        if (state == RaceStates.Racing)
        {
            UpdateCheckpoints();
            timer += Time.deltaTime;
            raceTimerText.text = LeaderboardManager.FormatMilliseconds(Mathf.RoundToInt(timer * 1000));
        }

        if (state == RaceStates.Finished)
        {
            // highlight personal best if we just achieved it!
            raceFinishedPBText.highlighted = raceFinishedPBText.score == timescore;
            // highlight world record if we just achieved it!
            raceFinishedWRText.highlighted = raceFinishedWRText.score == timescore;
        }
        
    }

    BuggySystem CheckHoldingControllers()
    {
        BuggySystem rhc = null;
        BuggySystem lhc = null;
        for(int i = 0; i < buggies.Length; i++)
        {
            if(Player.instance.rightHand.currentAttachedObject == buggies[i].controller.gameObject)
            {
                rhc = buggies[i];
            }
            if (Player.instance.leftHand.currentAttachedObject == buggies[i].controller.gameObject)
            {
                lhc = buggies[i];
            }
        }

        if(rhc!=null && lhc != null)
        {
            return rhc; // return right hand controller if holding with both hands. Safe guess.
        }
        else if(rhc != null)
        {
            return rhc;
        }
        else if (lhc != null)
        {
            return lhc;
        }
        else
        {
            return null;
        }
    }

    public void RaceButtonPressed()
    {
        if(state == RaceStates.WaitingForRace)
        {
            raceConfirmerUIGO.SetActive(false);
            StartCoroutine(CountdownCoroutine());
            activeCheckpoint = 0; // show checkpoints
            UpdateCheckpoints();
        }
    }

    public void RaceFinishButtonRetry()
    {
        if (state == RaceStates.Finished || state == RaceStates.Cancelled)
        {
            if(buggy != null)
            {
                ActivateForBuggy(buggy); // replay with same buggy
            }
        }
    }

    public void RaceFinishButtonNewCar()
    {
        if (state == RaceStates.Finished || state == RaceStates.Cancelled)
        {
            int currentBuggy = CurrentBuggyIndex();
            int newBuggy = currentBuggy == 0 ? 1 : 0;

            Hand holding = FindHandHoldingController(buggies[currentBuggy].controller);
            // switch controller in hand if holding it
            if (holding != null)
            {
                holding.DetachObject(buggies[currentBuggy].controller.gameObject);
                holding.AttachObject(buggies[newBuggy].controller.gameObject, GrabTypes.Grip); 
            }

            ActivateForBuggy(buggies[newBuggy].buggy);
        }
    }

    Hand FindHandHoldingController(BuggyControllerMoon controller)
    {
        if (Player.instance.rightHand.currentAttachedObject == controller.gameObject) return Player.instance.rightHand;
        if (Player.instance.leftHand.currentAttachedObject == controller.gameObject) return Player.instance.leftHand;

        return null;
    }

    public void RaceFinishButtonExit()
    {
        if (state == RaceStates.Finished || state == RaceStates.Cancelled)
        {
            SwitchState(RaceStates.Inactive);
        }
    }


    public void ButtonCancelRace()
    {
        if(state == RaceStates.Racing)
        {
            SwitchState(RaceStates.Cancelled);
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
        UnFreezeBuggy();
        StartCoroutine(RaceCoroutine());
        timer = 0;
    }

    void FinishRace()
    {
        // upload score converted to milliseconds
        timescore = (Mathf.RoundToInt(timer * 1000));
        leaderboardManager.UploadScore(timescore);
        raceFinishedTimeText.text = LeaderboardManager.FormatMilliseconds(timescore);
        SwitchState(RaceStates.Finished);
        Debug.Log("race complete!");
    }
    

    IEnumerator RaceCoroutine()
    {
        Vector3 oldBuggyPosition = buggy.transform.position;

        // wait for car to pass through each checkpoint
        for(activeCheckpoint = 0; activeCheckpoint < checkpoints.Length; activeCheckpoint++)
        {
            while ( !checkpoints[activeCheckpoint].HasPassedCheckpoint(oldBuggyPosition, buggy.transform.position) ){
                oldBuggyPosition = buggy.transform.position;
                yield return null;
            }

            PlaySound(au_checkpoint, checkpoints[activeCheckpoint].transform.position);
        }

        // wait for car to pass through finish line
        while (!finishLine.HasPassedCheckpoint(oldBuggyPosition, buggy.transform.position))
        {
            oldBuggyPosition = buggy.transform.position;
            yield return null;
        }

        PlaySound(au_raceFinish, finishLine.transform.position);

        FinishRace();
    }

    void UpdateCheckpoints()
    {
        for (int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i].state = GetStateForCheckpoint(i);
        }
        finishLine.state = GetStateForCheckpoint(checkpoints.Length);
    }

    RaceCheckpoint.CheckpointStates GetStateForCheckpoint(int i)
    {
        if (i == activeCheckpoint)
        {
            return RaceCheckpoint.CheckpointStates.Active;
        }
        else if (i == activeCheckpoint + 1)
        {
            return RaceCheckpoint.CheckpointStates.Next;
        }
        else if (i > activeCheckpoint)
        {
            return RaceCheckpoint.CheckpointStates.Inactive;
        }
        else
        {
            return RaceCheckpoint.CheckpointStates.Disabled;
        }
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

        if (sourceState == RaceStates.WaitingForBuggy)
        {
            buggyRequestGO.SetActive(false);
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
            raceTrackGO.SetActive(false);
        }

        if (sourceState == RaceStates.Finished)
        {
            raceFinishedCanvas.SetActive(false);
        }

        if (sourceState == RaceStates.Cancelled)
        {
            raceCancelledCanvas.SetActive(false);
        }

        // ==================================

        if (targetState == RaceStates.Inactive)
        {
            waitingForExit = true; // do not activate again until player has left and come back.
            inactiveTargetGO.SetActive(true);
            statsGO.SetActive(false);
            ResetAll();
            if (activeRace == this) activeRace = null;
            UnFreezeBuggy();
        }

        if (targetState == RaceStates.WaitingForBuggy)
        {
            buggyRequestGO.SetActive(true);
            statsGO.SetActive(true);
            raceTrackGO.SetActive(true);
            raceTitleGraphic.SetActive(true);
            activeCheckpoint = 1000; // hide all checkpoints
            UpdateCheckpoints();

        }

        if (targetState == RaceStates.WaitingForRace)
        {
            raceTitleGraphic.SetActive(false);
            raceInitiatorGO.SetActive(true);
            raceTrackGO.SetActive(true);
            raceCountdownGO.SetActive(false);
            raceConfirmerUIGO.SetActive(true);
            activeCheckpoint = 1000; // hide all checkpoints
            UpdateCheckpoints();
        }

        if (targetState == RaceStates.Racing)
        {
            statsGO.SetActive(false);

            raceTimerCanvas.SetActive(true);
            raceCancelCanvas.SetActive(true);
            raceTrackGO.SetActive(true);
        }

        if (targetState == RaceStates.Finished)
        {
            statsGO.SetActive(true);
            raceTrackGO.SetActive(false);
            raceFinishedCanvas.SetActive(true);
        }

        if (targetState == RaceStates.Cancelled)
        {
            statsGO.SetActive(true);
            raceTrackGO.SetActive(false);
            raceCancelledCanvas.SetActive(true);
        }

        state = targetState;
    }

    private void OnDestroy()
    {
        if (activeRace == this) activeRace = null;
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

    /*
    IEnumerator TeleportPlayerToTransform(Transform t)
    {
        SteamVR_Fade.Start(Color.black, 0.5f);
        yield return new WaitForSeconds(0.5f);

        MovePlayerToTransform(t);

        SteamVR_Fade.Start(Color.clear, 0.5f);
        yield return new WaitForSeconds(0.5f);
    }

    void MovePlayerToTransform(Transform t)
    {
        Player player = Player.instance;

        //rotate the player to face the right direction;
        float targetAngle = t.eulerAngles.y;
        float playerAngle = player.hmdTransform.eulerAngles.y;
        float angleOffset = Mathf.DeltaAngle(targetAngle, playerAngle);
        player.trackingOriginTransform.Rotate(Vector3.up, -angleOffset, Space.World);

        Vector3 playerFeetOffset = player.trackingOriginTransform.position - player.feetPositionGuess;
        player.trackingOriginTransform.position = t.position + playerFeetOffset;
    }
    */
    void FreezeBuggy()
    {
        if (buggy != null)
        {
            for (int i = 0; i < buggy.wheels.Length; i++)
            {
                buggy.wheels[i].brakeTorque = 100000;
            }
        }
    }

    void UnFreezeBuggy()
    {
        if (buggy != null)
        {
            for (int i = 0; i < buggy.wheels.Length; i++)
            {
                buggy.wheels[i].brakeTorque = 0;
            }
        }
    }


    void DebugLog(string log)
    {
        if (debug)
            Debug.Log(log);
    }
}
