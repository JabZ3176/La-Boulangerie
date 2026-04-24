using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject container;
    public GameObject container1;

    private bool isPaused = false;

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

    private void Pause()
    {
        container.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Resume()
    {
        container.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


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

    public void Tutorial()
    {
        container1.SetActive(true);
        container.SetActive(false);
        Time.timeScale = 1;
    }
}
