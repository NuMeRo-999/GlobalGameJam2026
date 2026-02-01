using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LeaderboardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform contentParent; // ScrollView Content
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private Button backButton;
    [SerializeField] private Button clearButton;

    [Header("Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private Color oddRowColor = new Color(0.75f, 0.75f, 0.75f, 1f);
    [SerializeField] private Color evenRowColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    [SerializeField] private Color topThreeColor = new Color(1f, 0.84f, 0f, 1f); // Gold

    void Start()
    {
        if (backButton != null)
            backButton.onClick.AddListener(GoBack);

        if (clearButton != null)
            clearButton.onClick.AddListener(ClearLeaderboard);

        PopulateLeaderboard();
    }

    public void PopulateLeaderboard()
    {
        // Clear existing entries
        if (contentParent != null)
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }
        }

        if (LeaderboardManager.Instance == null || entryPrefab == null || contentParent == null)
        {
            Debug.LogWarning("LeaderboardUI: Missing references!");
            return;
        }

        var entries = LeaderboardManager.Instance.Entries;

        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            GameObject entryObj = Instantiate(entryPrefab, contentParent);

            // Set up the entry display
            LeaderboardEntryUI entryUI = entryObj.GetComponent<LeaderboardEntryUI>();
            if (entryUI != null)
            {
                entryUI.SetData(i + 1, entry.playerName, entry.time, entry.date);
                
                // Color coding
                if (i < 3)
                    entryUI.SetHighlight(topThreeColor);
                else
                    entryUI.SetBackgroundColor(i % 2 == 0 ? evenRowColor : oddRowColor);
            }
            else
            {
                // Fallback: Try to find TextMeshPro components directly
                SetupEntryFallback(entryObj, i, entry);
            }
        }

        if (entries.Count == 0)
        {
            // Show "No entries" message
            GameObject emptyObj = Instantiate(entryPrefab, contentParent);
            TextMeshProUGUI[] texts = emptyObj.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length > 0)
            {
                texts[0].text = "No entries yet. Be the first!";
                for (int i = 1; i < texts.Length; i++)
                    texts[i].text = "";
            }
        }
    }

    private void SetupEntryFallback(GameObject entryObj, int index, LeaderboardEntry entry)
    {
        TextMeshProUGUI[] texts = entryObj.GetComponentsInChildren<TextMeshProUGUI>();
        
        // Assumes order: Rank, Name, Time, Date
        if (texts.Length >= 1) texts[0].text = $"#{index + 1}";
        if (texts.Length >= 2) texts[1].text = entry.playerName;
        if (texts.Length >= 3) texts[2].text = SpeedrunTimer.FormatTime(entry.time);
        if (texts.Length >= 4) texts[3].text = entry.date;
    }

    private void GoBack()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void ClearLeaderboard()
    {
        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.ClearLeaderboard();
            PopulateLeaderboard();
        }
    }
}
