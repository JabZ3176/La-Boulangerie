using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    #region BUTTONS
    [Header("Buttons")]
    public Button continueButton;
    #endregion

    #region START
    void Start()
    {
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
    #endregion

    #region GAME FLOW
    public void NewGame()
    {
        PlayerPrefs.DeleteKey("CurrentLevel");
        PlayerPrefs.DeleteKey("LevelReached");
        PlayerPrefs.Save();

        Time.timeScale = 1f;

        bool firstTime = PlayerPrefs.GetInt("HasPlayedBefore", 0) == 0;

        if (firstTime)
        {
            PlayerPrefs.SetInt("HasPlayedBefore", 1);
            PlayerPrefs.Save();

            SceneManager.LoadScene("Tutorial");
        }
        else
        {
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
        Time.timeScale = 1f;
        SceneManager.LoadScene("Tutorial");
    }
    #endregion

    #region NAVIGATION
    public void QuitGame()
    {
        Application.Quit();
    }

    public void Levels()
    {
        SceneManager.LoadScene("LevelScene");
    }
    #endregion
}
