using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    public Button continueButton; // drag your Continue button here in the Inspector

    void Start()
    {
        // if there is no saved level, grey out the continue button
        // the default is empty string "" if nothing has been saved yet
        if (PlayerPrefs.GetString("CurrentLevel", "") == "")
        {
            continueButton.interactable = false;

            // optionally fade the text so it looks locked
            TMPro.TextMeshProUGUI label = continueButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (label != null)
            {
                label.color = new Color(1f, 1f, 1f, 0.3f);
            }
        }
        else
        {
            continueButton.interactable = true;
        }
    }

    public void NewGame()
    {
        // wipe all saved progress so everything starts fresh
        PlayerPrefs.DeleteKey("CurrentLevel");
        PlayerPrefs.DeleteKey("LevelReached");
        PlayerPrefs.SetInt("ShowTutorial", 1);
        PlayerPrefs.Save();

        Time.timeScale = 1;
        SceneManager.LoadScene("Level1");
    }

    public void ContinueGame()
    {
        // load whatever level the player was last on
        string savedLevel = PlayerPrefs.GetString("CurrentLevel", "Level1");
        Time.timeScale = 1;
        SceneManager.LoadScene(savedLevel);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Levels()
    {
        SceneManager.LoadScene("LevelScene");
    }
}