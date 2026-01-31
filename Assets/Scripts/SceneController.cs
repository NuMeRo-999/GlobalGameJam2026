using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private GameObject transitionUI;
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private float scaleMultiplier = 1.5f;

    private bool isTransitioning = false;

    void Start()
    {
        if (transitionUI != null)
        {
            // Start covering the screen, then scale down to reveal scene
            transitionUI.transform.localScale = Vector3.one * scaleMultiplier;
            StartCoroutine(ScaleDown());
        }
    }

    private IEnumerator ScaleDown()
    {
        Vector3 startScale = Vector3.one * scaleMultiplier;
        Vector3 targetScale = Vector3.zero;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;
            // Smooth easing
            t = t * t * (3f - 2f * t);
            transitionUI.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transitionUI.transform.localScale = targetScale;
        transitionUI.SetActive(false);
    }

    private IEnumerator ScaleUpAndLoadScene(string sceneName)
    {
        isTransitioning = true;
        transitionUI.SetActive(true);
        
        Vector3 startScale = Vector3.zero;
        Vector3 targetScale = Vector3.one * scaleMultiplier;
        float elapsedTime = 0f;

        transitionUI.transform.localScale = startScale;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;
            // Smooth easing
            t = t * t * (3f - 2f * t);
            transitionUI.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transitionUI.transform.localScale = targetScale;
        
        // Load the scene after transition completes
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator ScaleUpAndLoadSceneByIndex(int sceneIndex)
    {
        isTransitioning = true;
        transitionUI.SetActive(true);
        
        Vector3 startScale = Vector3.zero;
        Vector3 targetScale = Vector3.one * scaleMultiplier;
        float elapsedTime = 0f;

        transitionUI.transform.localScale = startScale;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;
            // Smooth easing
            t = t * t * (3f - 2f * t);
            transitionUI.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transitionUI.transform.localScale = targetScale;
        
        // Load the scene after transition completes
        SceneManager.LoadScene(sceneIndex);
    }

    private void LoadScene(string sceneName)
    {
        if (isTransitioning) return;
        
        if (transitionUI != null)
        {
            StartCoroutine(ScaleUpAndLoadScene(sceneName));
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    public void LoadMainMenu()
    {
        LoadScene("MainMenu");
    }
    public void LoadLevel1()
    {
        LoadScene("Level1");
    }
    public void LoadLevel2()
    {
        LoadScene("Level2");
    }
    public void LoadLevel3()
    {
        LoadScene("Level3");
    }
    public void LoadLevel4()
    {
        LoadScene("Level4");
    }

    public void LoadGameOver()
    {
        LoadScene("GameOver");
    }

    public void LoadNextScene()
    {
        if (isTransitioning) return;
        
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Check if next scene exists
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            if (transitionUI != null)
            {
                StartCoroutine(ScaleUpAndLoadSceneByIndex(nextSceneIndex));
            }
            else
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
        }
    }
}
