using UnityEngine;
using UnityEngine.SceneManagement;

public class SpeedrunTimer : MonoBehaviour
{
    public static SpeedrunTimer Instance { get; private set; }

    private float currentTime;
    private bool isRunning;
    private string startSceneName = "Demo1"; // Scene where timer starts
    private string endSceneName = "GameOver"; // Scene where timer stops

    public float CurrentTime => currentTime;
    public bool IsRunning => isRunning;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void Update()
    {
        if (isRunning)
        {
            currentTime += Time.deltaTime;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        print($"Scene Loaded: {scene.name}");
        // Auto-start timer when entering the first level
        if (scene.name == startSceneName && !isRunning)
        {
            StartTimer();
        }
        // Auto-stop timer when reaching end scene
        else if (scene.name == endSceneName && isRunning)
        {
            StopTimer();
        }
    }

    public void StartTimer()
    {
        currentTime = 0f;
        isRunning = true;
        Debug.Log("Speedrun Timer Started!");
    }

    public void StopTimer()
    {
        isRunning = false;
        Debug.Log($"Speedrun Timer Stopped! Final Time: {GetFormattedTime()}");
    }

    public void ResetTimer()
    {
        currentTime = 0f;
        isRunning = false;
    }

    public string GetFormattedTime()
    {
        return FormatTime(currentTime);
    }

    public static string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);
        return $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }

    public void SetStartScene(string sceneName)
    {
        startSceneName = sceneName;
    }

    public void SetEndScene(string sceneName)
    {
        endSceneName = sceneName;
    }
}
