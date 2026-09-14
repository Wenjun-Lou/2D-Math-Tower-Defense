using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    public void EnterEndlessMode()
    {
        LevelManager.Instance.LoadLevel(LevelManager.Instance.allLevels[0]);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
