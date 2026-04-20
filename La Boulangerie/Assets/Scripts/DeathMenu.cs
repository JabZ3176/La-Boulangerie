using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes

public class DeathMenu : MonoBehaviour
{
    public GameObject deathScreenUI;

    // Call this function when the player dies
    public void ToggleDeathScreen()
    {
        deathScreenUI.SetActive(true);
        Time.timeScale = 0f; // Pauses the game

        // Unlock the cursor if your game locks it (FPS games)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Respawn()
    {
        Time.timeScale = 1f; // Unpause the game
        // Reloads the currently active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        // Replace "MainMenu" with the exact name of your menu scene
        SceneManager.LoadScene("MainMenu");
    }
}