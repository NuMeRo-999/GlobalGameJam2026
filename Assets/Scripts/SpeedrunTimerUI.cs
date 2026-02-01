using UnityEngine;
using TMPro;

public class SpeedrunTimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private bool showOnlyWhenRunning = false;

    void Update()
    {
        if (SpeedrunTimer.Instance == null)
        {
            if (timerText != null)
                timerText.text = "00:00.000";
            return;
        }

        if (showOnlyWhenRunning && !SpeedrunTimer.Instance.IsRunning)
        {
            if (timerText != null)
                timerText.gameObject.SetActive(false);
            return;
        }

        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
            timerText.text = SpeedrunTimer.Instance.GetFormattedTime();
        }
    }
}
