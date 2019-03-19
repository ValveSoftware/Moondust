using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using UnityEngine.UI;

public class LeaderboardDisplay : MonoBehaviour
{
    public LeaderboardManager leaderboard;

    public GameObject entryPrefab;

    public RectTransform board_topTen;
    public RectTransform board_user;

    private EntryCard[] cards_topTen;
    private EntryCard[] cards_user;

    class EntryCard
    {
        Text ui_place;
        Text ui_name;
        Text ui_score;
        Image ui_background;

        public int place;
        public CSteamID ID;
        public string name;
        public int score;
        public bool isUser;

        public GameObject gameObject;

        public ScoreFormats format;

        public EntryCard(GameObject gameObject, int place, CSteamID steamID, string name, int score, ScoreFormats format)
        {
            this.gameObject = gameObject;
            this.place = place;
            this.ID = steamID;
            this.name = name;
            this.score = score;
            this.format = format;
            this.isUser = this.ID == SteamUser.GetSteamID();

            // get UI components
            ui_place = gameObject.transform.Find("NumberBG/Number").GetComponent<Text>();
            ui_name = gameObject.transform.Find("Name").GetComponent<Text>();
            ui_score = gameObject.transform.Find("TimeBG/Time").GetComponent<Text>();
            ui_background = gameObject.GetComponent<Image>();

            UpdateGraphics();
        }

        public void UpdateGraphics()
        {
            ui_place.text = place + ".";
            ui_name.text = name;
            ui_score.text = format == ScoreFormats.Milliseconds ? LeaderboardManager.FormatMilliseconds(score) : score.ToString();
            isUser = ID == SteamUser.GetSteamID();
            ui_background.color = isUser ? color_highlight : color_normal;
        }
    }
    public static readonly Color color_highlight = new Color(0.909f, 0.647f, 0.349f, 0.2f);
    public static readonly Color color_normal = new Color(0.571f, 0.693f, 0.801f, 0.2f);

    public enum ScoreFormats
    {
        Milliseconds,
        Number
    }
    public ScoreFormats scoreFormat;

    private void Start()
    {
        leaderboard.OnLeaderboardUpdate.AddListener(UpdateLeaderboards);
    }

    private void OnEnable()
    {
        UpdateLeaderboards(!leaderboard.s_initialized);
    }

    public void UpdateLeaderboards(bool failed)
    {
        if (failed) return;
        UpdateLeaderboard(ref cards_topTen, leaderboard.entriesTopTen, board_topTen);
        UpdateLeaderboard(ref cards_user, leaderboard.entriesAroundUser, board_user);
    }

    void UpdateLeaderboard(ref EntryCard[] board, LeaderboardEntry_t[] entries, RectTransform boardUI)
    {
        if (entries == null) return; // entries not recieved yet

        ClearBoard(ref board);
        board = new EntryCard[entries.Length];
        for (int i = 0; i < entries.Length; i++)
        {
            GameObject newEntry = Instantiate(entryPrefab, boardUI);
            newEntry.SetActive(true);
            newEntry.name = i.ToString();
            board[i] = new EntryCard(newEntry, entries[i].m_nGlobalRank, entries[i].m_steamIDUser, SteamFriends.GetFriendPersonaName(entries[i].m_steamIDUser), entries[i].m_nScore, scoreFormat);
        }
    }

    void ClearBoard(ref EntryCard[] board)
    {
        if (board == null) return; // already null

        // Destroy gameObjects of all board entries
        for (int i = 0; i < board.Length; i++)
        {
            Destroy(board[i].gameObject);
            Debug.Log(board[i].gameObject);
        }
    }
}
