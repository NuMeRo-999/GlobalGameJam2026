using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public float time;
    public string date;

    public LeaderboardEntry(string name, float time)
    {
        this.playerName = name;
        this.time = time;
        this.date = DateTime.Now.ToString("yyyy-MM-dd");
    }
}

[Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
}

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    private const string LEADERBOARD_KEY = "SpeedrunLeaderboard";
    private const int MAX_ENTRIES = 100;

    private LeaderboardData leaderboardData;

    public List<LeaderboardEntry> Entries => leaderboardData.entries;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLeaderboard();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddEntry(string playerName, float time)
    {
        LeaderboardEntry newEntry = new LeaderboardEntry(playerName, time);
        leaderboardData.entries.Add(newEntry);

        // Sort by time (fastest first)
        leaderboardData.entries = leaderboardData.entries
            .OrderBy(e => e.time)
            .Take(MAX_ENTRIES)
            .ToList();

        SaveLeaderboard();
        Debug.Log($"Leaderboard entry added: {playerName} - {SpeedrunTimer.FormatTime(time)}");
    }

    public int GetRank(float time)
    {
        int rank = 1;
        foreach (var entry in leaderboardData.entries)
        {
            if (time > entry.time)
                rank++;
            else
                break;
        }
        return rank;
    }

    public LeaderboardEntry GetBestTime()
    {
        if (leaderboardData.entries.Count > 0)
            return leaderboardData.entries[0];
        return null;
    }

    private void SaveLeaderboard()
    {
        string json = JsonUtility.ToJson(leaderboardData);
        PlayerPrefs.SetString(LEADERBOARD_KEY, json);
        PlayerPrefs.Save();
    }

    private void LoadLeaderboard()
    {
        if (PlayerPrefs.HasKey(LEADERBOARD_KEY))
        {
            string json = PlayerPrefs.GetString(LEADERBOARD_KEY);
            leaderboardData = JsonUtility.FromJson<LeaderboardData>(json);
        }
        else
        {
            leaderboardData = new LeaderboardData();
        }
    }

    public void ClearLeaderboard()
    {
        leaderboardData = new LeaderboardData();
        PlayerPrefs.DeleteKey(LEADERBOARD_KEY);
        PlayerPrefs.Save();
        Debug.Log("Leaderboard cleared!");
    }
}
