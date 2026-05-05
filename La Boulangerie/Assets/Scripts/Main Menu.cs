using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    public Button continueButton;   // drag Continue button here

    void Start()
    {
        // grey out continue if no save exists
        if (PlayerPrefs.GetString("CurrentLevel", "") == "")
        {
            continueButton.interactable = false;
            TMPro.TextMeshProUGUI label = continueButton
                .GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (label != null)
                label.color = new Color(1f, 1f, 1f, 0.3f);
        }
        else
        {
            continueButton.interactable = true;
        }
    }

    public void NewGame()
    {
        // wipe all saved progress
        PlayerPrefs.DeleteKey("CurrentLevel");
        PlayerPrefs.DeleteKey("LevelReached");
        PlayerPrefs.Save();

        Time.timeScale = 1f;

        // check if this is the first time playing
        bool firstTime = PlayerPrefs.GetInt("HasPlayedBefore", 0) == 0;

        if (firstTime)
        {
            // mark as played so next new game skips tutorial
            PlayerPrefs.SetInt("HasPlayedBefore", 1);
            PlayerPrefs.Save();

            // go to tutorial first
            SceneManager.LoadScene("Tutorial");
        }
        else
        {
            // skip tutorial and go straight to Level1
            SceneManager.LoadScene("Level1");
        }
    }

    public void ContinueGame()
    {
        PlayerPrefs.SetInt("ShowTutorial", 0);
        PlayerPrefs.Save();

        string savedLevel = PlayerPrefs.GetString("CurrentLevel", "Level1");
        Time.timeScale = 1f;
        SceneManager.LoadScene(savedLevel);
    }

    public void OpenTutorial()
    {
        // accessible from main menu for returning players
        Time.timeScale = 1f;
        SceneManager.LoadScene("Tutorial");
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