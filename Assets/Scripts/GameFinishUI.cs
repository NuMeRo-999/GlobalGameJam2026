using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameFinishUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject finishPanel;
    [SerializeField] private TextMeshProUGUI finalTimeText;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button leaderboardButton;

    [Header("Settings")]
    [SerializeField] private string leaderboardSceneName = "Leaderboard";
    [SerializeField] private int maxNameLength = 15;

    private float finalTime;
    private bool hasSubmitted = false;

    void Start()
    {
        if (finishPanel != null)
            finishPanel.SetActive(false);

        if (submitButton != null)
            submitButton.onClick.AddListener(OnSubmitScore);

        if (leaderboardButton != null)
            leaderboardButton.onClick.AddListener(GoToLeaderboard);

        if (nameInputField != null)
            nameInputField.characterLimit = maxNameLength;

        // Auto-show panel if timer exists and was just stopped
        if (SpeedrunTimer.Instance != null && !SpeedrunTimer.Instance.IsRunning)
        {
            ShowFinishPanel();
        }
    }

    public void ShowFinishPanel()
    {
        if (SpeedrunTimer.Instance == null) return;

        finalTime = SpeedrunTimer.Instance.CurrentTime;

        if (finishPanel != null)
            finishPanel.SetActive(true);

        if (finalTimeText != null)
            finalTimeText.text = $"{SpeedrunTimer.Instance.GetFormattedTime()}";

        if (rankText != null && LeaderboardManager.Instance != null)
        {
            int rank = LeaderboardManager.Instance.GetRank(finalTime);
            rankText.text = $"Rango: #{rank}";
        }

        hasSubmitted = false;
        UpdateSubmitButton();
    }

    private void OnSubmitScore()
    {
        if (hasSubmitted) return;

        string playerName = nameInputField != null ? nameInputField.text.Trim() : "Anonymous";

        if (string.IsNullOrEmpty(playerName))
            playerName = "Anonymous";

        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.AddEntry(playerName, finalTime);
        }

        hasSubmitted = true;
        UpdateSubmitButton();

        Debug.Log($"Score submitted: {playerName} - {SpeedrunTimer.FormatTime(finalTime)}");
    }

    private void UpdateSubmitButton()
    {
        if (submitButton != null)
            submitButton.interactable = !hasSubmitted;
    }

    private void GoToLeaderboard()
    {
        // Submit if not already submitted
        if (!hasSubmitted && nameInputField != null && !string.IsNullOrEmpty(nameInputField.text.Trim()))
        {
            OnSubmitScore();
        }

        SceneManager.LoadScene(leaderboardSceneName);
    }
}
