using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using UnityEngine.UI;

public class LeaderboardSingleEntryText : MonoBehaviour
{
    public LeaderboardManager leaderboard;

    Text text;

    Color defaultColor;

    bool hasGottenScore = false;

    [HideInInspector]
    public int score;

    public enum ScoreTypes
    {
        PersonalBest,
        WorldRecord
    }
    public ScoreTypes scoreType;

    public enum ScoreFormats
    {
        Milliseconds,
        Number
    }
    public ScoreFormats scoreFormat;

    static Color highlightColor = new Color(0.702f, 1f, 0.5f, 1);
    public bool highlighted
    {
        set
        {
            text.color = value ? highlightColor : defaultColor;
            m_highlighted = value;
        }
        get
        {
            return m_highlighted;
        }
    }
    private bool m_highlighted = false;

    private void Awake()
    {
        text = GetComponent<Text>();
        defaultColor = text.color;
        text.text = "loading..."; // make sure this happens before OnEnable
    }

    private void Start()
    {
        leaderboard.OnLeaderboardUpdate.AddListener(UpdateScore);
    }

    private void Update()
    {
        if (highlighted)
        {
            Color c = highlightColor;
            c.a = Mathf.Sin(Time.time * 8f);
            text.color = c;
        }
    }

    private void OnEnable()
    {
        UpdateScore(!leaderboard.s_initialized);
    }

    public void UpdateScore(bool failed)
    {
        if(text == null) text = GetComponent<Text>();

        if (failed)
        {
            text.text = "";
            score = 0;
        }
        else
        {
            if (scoreType == ScoreTypes.PersonalBest) score = leaderboard.entryUser.m_nScore;
            if (scoreType == ScoreTypes.WorldRecord)
            {
                if(leaderboard.entriesTopTen.Length > 0)
                    score = leaderboard.entriesTopTen[0].m_nScore;
            }

            if (score == 0) {
                text.text = "";
            }
            else
            {
                text.text = scoreFormat == ScoreFormats.Milliseconds ? LeaderboardManager.FormatMilliseconds(score) : score.ToString();
            }
        }
    }
}
