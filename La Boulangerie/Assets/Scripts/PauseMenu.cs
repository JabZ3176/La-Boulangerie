using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    #region REFERENCES
    public GameObject pauseMenuContainer;
    public GameObject settingsContainer;
    #endregion

    #region PRIVATE VARIABLES
    private bool isPaused = false;
    #endregion

    #region UPDATE
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    #endregion

    #region PAUSE CONTROL
    private void Pause()
    {
        pauseMenuContainer.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Resume()
    {
        pauseMenuContainer.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    #endregion

    #region BUTTONS
    public void ResumeButton()
    {
        Resume();
    }

    public void MainMenuButton()
    {
        Time.timeScale = 0;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void Levels()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelScene");
        Time.timeScale = 0;
    }

    public void OpenSettings()
    {
        pauseMenuContainer.SetActive(false);
        settingsContainer.SetActive(true);
    }

    public void BackToPauseMenu()
    {
        settingsContainer.SetActive(false);
        pauseMenuContainer.SetActive(true);
    }
    #endregion
}
