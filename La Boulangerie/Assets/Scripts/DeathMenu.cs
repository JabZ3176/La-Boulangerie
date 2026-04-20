using UnityEngine;
using UnityEngine.SceneManagement; 

public class DeathMenu : MonoBehaviour
{
    public GameObject deathScreenUI;

    public void ToggleDeathScreen()
    {
        deathScreenUI.SetActive(true);
        Time.timeScale = 0f; 

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Respawn()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}