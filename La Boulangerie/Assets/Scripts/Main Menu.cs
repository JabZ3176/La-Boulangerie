using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame ()
    {
        PlayerPrefs.SetInt("ShowTutorial", 1); 
        PlayerPrefs.Save();
        SceneManager.LoadScene("Level1");
    }

    public void QuitGame ()
        {
            Application.Quit();
    }
    public void Levels()
    {
        SceneManager.LoadScene("LevelScene");
    }
}
