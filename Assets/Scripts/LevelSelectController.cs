using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectController : MonoBehaviour
{
    public void LevelSelect(int levelIndex)
    {
        LevelManager.Instance.LoadLevel(LevelManager.Instance.allLevels[levelIndex]);
    }

    public void BackToMainMenu()
    {
        GameManager.Instance.SetTimeScale(1f);
        SceneManager.LoadScene("MainMenu");
    }
}
