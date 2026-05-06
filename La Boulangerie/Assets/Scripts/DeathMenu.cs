using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    #region REFERENCES
    public GameObject deathScreenUI;
    #endregion

    #region DEATH SCREEN
    public void ToggleDeathScreen()
    {
        deathScreenUI.SetActive(true);
        Time.timeScale = 0;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    #endregion

    #region BUTTONS
    public void Respawn()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
    #endregion
}
