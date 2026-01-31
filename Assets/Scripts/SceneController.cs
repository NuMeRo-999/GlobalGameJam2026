using UnityEngine;

public class SceneController : MonoBehaviour
{

private void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
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

}
