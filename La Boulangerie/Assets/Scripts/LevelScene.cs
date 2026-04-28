using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelScene : MonoBehaviour
{
    // each of these is called by the corresponding button in the Inspector

    public void Level1()
    {
        // level 1 is always accessible
        SceneManager.LoadScene("Level1");
    }

    public void Level2()
    {
        // only load if player has reached level 2
        if (PlayerPrefs.GetInt("LevelReached", 1) >= 2)
        {
            SceneManager.LoadScene("Level2");
        }
    }

    public void Level3()
    {
        // only load if player has reached level 3
        if (PlayerPrefs.GetInt("LevelReached", 1) >= 3)
        {
            SceneManager.LoadScene("Level3");
        }
    }

    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}