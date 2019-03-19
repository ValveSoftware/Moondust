using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System.Threading;
using UnityEngine.Events;

public class LeaderboardManager : MonoBehaviour
{
    public enum LeaderboardTypes
    {
        Timed,
        Score
    }
    public LeaderboardTypes type;

    public class UpdateEvent : UnityEvent<bool> { }

    public string s_leaderboardName = "newBoard";
    private const ELeaderboardUploadScoreMethod s_leaderboardMethod = ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate;

    private SteamLeaderboard_t s_currentLeaderboard;

    private SteamLeaderboardEntries_t s_entriesTopTen;
    public LeaderboardEntry_t[] entriesTopTen;
    private SteamLeaderboardEntries_t s_entriesAroundUser;
    public LeaderboardEntry_t[] entriesAroundUser;
    private SteamLeaderboardEntries_t s_userEntry;
    public LeaderboardEntry_t entryUser;

    LeaderboardEntry_t[] GetDownloadedEntries(SteamLeaderboardEntries_t entries, int count)
    {
        LeaderboardEntry_t[] e = new LeaderboardEntry_t[count];
        for (int i = 0; i < count; i++)
        {
            LeaderboardEntry_t entry;
            SteamUserStats.GetDownloadedLeaderboardEntry(entries, i, out entry, new int[] { }, 0);
            e[i] = entry;
        }
        return e;
    }

    [HideInInspector]
    public bool s_initialized = false;
    private CallResult<LeaderboardFindResult_t> m_findResult = new CallResult<LeaderboardFindResult_t>();
    private CallResult<LeaderboardScoreUploaded_t> m_uploadResult = new CallResult<LeaderboardScoreUploaded_t>();
    private CallResult<LeaderboardScoresDownloaded_t> m_downloadResultTopTen = new CallResult<LeaderboardScoresDownloaded_t>();
    private CallResult<LeaderboardScoresDownloaded_t> m_downloadResultAroundUser = new CallResult<LeaderboardScoresDownloaded_t>();
    private CallResult<LeaderboardScoresDownloaded_t> m_downloadResultUser = new CallResult<LeaderboardScoresDownloaded_t>();



    [HideInInspector]
    public UpdateEvent OnLeaderboardUpdate = new UpdateEvent();

    public void Start()
    {
        ELeaderboardSortMethod sortMethod = type == LeaderboardTypes.Score ? ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending : ELeaderboardSortMethod.k_ELeaderboardSortMethodAscending;
        ELeaderboardDisplayType displayType = type == LeaderboardTypes.Score ? ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric : ELeaderboardDisplayType.k_ELeaderboardDisplayTypeTimeMilliSeconds;

        SteamAPICall_t hSteamAPICall = SteamUserStats.FindOrCreateLeaderboard(s_leaderboardName, sortMethod, displayType);
        m_findResult.Set(hSteamAPICall, OnLeaderboardFindResult);

        InitTimer();
    }
    

    public void DownloadEntries()
    {
        //download entries from steam!

        if (s_initialized == false) return;

        SteamAPICall_t hSteamAPICall = SteamUserStats.DownloadLeaderboardEntries(s_currentLeaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 0, 9);
        m_downloadResultTopTen.Set(hSteamAPICall, OnLeaderboardFindTopTen);

        SteamAPICall_t hSteamAPICallB = SteamUserStats.DownloadLeaderboardEntries(s_currentLeaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobalAroundUser, -5, 4);
        m_downloadResultAroundUser.Set(hSteamAPICallB, OnLeaderboardFindAroundUser);

        SteamAPICall_t hSteamAPICallC = SteamUserStats.DownloadLeaderboardEntriesForUsers(s_currentLeaderboard, new CSteamID[]{SteamUser.GetSteamID()}, 1);
        m_downloadResultUser.Set(hSteamAPICallC, OnLeaderboardFindUser);
    }

    public void UploadScore(int score)
    {
        if (!s_initialized)
        {
            UnityEngine.Debug.Log("Can't upload to the leaderboard because isn't loaded yet");
        }
        else
        {
            bool shouldUpdate = true;
            if (entryUser.m_nScore != 0)
            {
                // do not overwrite if the user already has a better score
                if (type == LeaderboardTypes.Timed)
                {
                    if (entryUser.m_nScore < score) shouldUpdate = false;
                }
                if (type == LeaderboardTypes.Score)
                {
                    if (entryUser.m_nScore > score) shouldUpdate = false;
                }
            }
            if (shouldUpdate)
            {
                UnityEngine.Debug.Log("uploading score(" + score + ") to steam leaderboard(" + s_leaderboardName + ")");
                SteamAPICall_t hSteamAPICall = SteamUserStats.UploadLeaderboardScore(s_currentLeaderboard, s_leaderboardMethod, score, new int[] { }, 0);
                m_uploadResult.Set(hSteamAPICall, OnLeaderboardUploadResult);
            }
            else
            {
                Debug.Log("better score exists, not uploading new score");
            }
        }
    }


    private void OnLeaderboardFindResult(LeaderboardFindResult_t pCallback, bool failure)
    {
        UnityEngine.Debug.Log("STEAM LEADERBOARDS: Found - " + pCallback.m_bLeaderboardFound + " leaderboardID - " + pCallback.m_hSteamLeaderboard.m_SteamLeaderboard);
        s_currentLeaderboard = pCallback.m_hSteamLeaderboard;
        s_initialized = true;

        DownloadEntries();
    }

    private void OnLeaderboardUploadResult(LeaderboardScoreUploaded_t pCallback, bool failure)
    {
        UnityEngine.Debug.Log("STEAM LEADERBOARDS: failure - " + failure + " Completed - " + pCallback.m_bSuccess + " NewScore: " + pCallback.m_nGlobalRankNew + " Score " + pCallback.m_nScore + " HasChanged - " + pCallback.m_bScoreChanged);
        if (!failure) DownloadEntries();
    }

    private void OnLeaderboardFindTopTen(LeaderboardScoresDownloaded_t pCallback, bool failure)
    {
        Debug.Log("STEAM LEADERBOARDS: found top " + pCallback.m_cEntryCount + " entries with failure: " + failure);
        s_entriesTopTen = pCallback.m_hSteamLeaderboardEntries;
        entriesTopTen = GetDownloadedEntries(s_entriesTopTen, pCallback.m_cEntryCount);
        OnLeaderboardUpdate.Invoke(failure);
    }

    private void OnLeaderboardFindAroundUser(LeaderboardScoresDownloaded_t pCallback, bool failure)
    {
        Debug.Log("STEAM LEADERBOARDS: found " + pCallback.m_cEntryCount + " entries around user with failure: "+failure);
        s_entriesAroundUser = pCallback.m_hSteamLeaderboardEntries;
        entriesAroundUser = GetDownloadedEntries(s_entriesAroundUser, pCallback.m_cEntryCount);
        OnLeaderboardUpdate.Invoke(failure);
    }

    private void OnLeaderboardFindUser(LeaderboardScoresDownloaded_t pCallback, bool failure)
    {
        if (pCallback.m_cEntryCount == 0) failure = true;
        Debug.Log("STEAM LEADERBOARDS: found user entry with failure: "+failure);
        s_userEntry = pCallback.m_hSteamLeaderboardEntries;

        if (pCallback.m_cEntryCount == 1)
            entryUser = GetDownloadedEntries(s_userEntry, pCallback.m_cEntryCount)[0];
        else
            failure = true;

        OnLeaderboardUpdate.Invoke(failure);
    }

    public static string FormatMilliseconds(int value)
    {
        int milliseconds = value;
        int seconds = 0;
        int minutes = 0;
        if (milliseconds >= 1000)
        {
            seconds = Mathf.FloorToInt((float)milliseconds / 1000);
            milliseconds = (int)Mathf.Repeat(milliseconds, 1000);
        }
        if (seconds >= 60)
        {
            minutes = Mathf.FloorToInt((float)seconds / 60);
            seconds = (int)Mathf.Repeat(seconds, 60);
        }
        return minutes.ToString("D2") + ":" + seconds.ToString("D2") + "." + milliseconds.ToString("D3");
    }



    private Timer timer1;
    public void InitTimer()
    {
        timer1 = new Timer(timer1_Tick, null, 0, 1000);
    }

    private static void timer1_Tick(object state)
    {
        SteamAPI.RunCallbacks();
    }

}
