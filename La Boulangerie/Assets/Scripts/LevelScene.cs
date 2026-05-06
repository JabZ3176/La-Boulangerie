using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelScene : MonoBehaviour
{
    #region LEVEL BUTTONS
    public void Level1()
    {
        SceneManager.LoadScene("Level1");
    }

    public void Level2()
    {
        if (PlayerPrefs.GetInt("LevelReached", 1) >= 2)
        {
            SceneManager.LoadScene("Level2");
        }
    }

    public void Level3()
    {
        if (PlayerPrefs.GetInt("LevelReached", 1) >= 3)
        {
            SceneManager.LoadScene("Level3");
        }
    }
    #endregion

    #region NAVIGATION
    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
    #endregion
}
