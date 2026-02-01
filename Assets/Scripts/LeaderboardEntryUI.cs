using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private Image backgroundImage;

    public void SetData(int rank, string playerName, float time, string date)
    {
        if (rankText != null)
            rankText.text = $"#{rank}";

        if (nameText != null)
            nameText.text = playerName;

        if (timeText != null)
            timeText.text = SpeedrunTimer.FormatTime(time);

        if (dateText != null)
            dateText.text = date;
    }

    public void SetBackgroundColor(Color color)
    {
        if (backgroundImage != null)
            backgroundImage.color = color;
    }

    public void SetHighlight(Color color)
    {
        if (rankText != null)
            rankText.color = color;
    }
}
