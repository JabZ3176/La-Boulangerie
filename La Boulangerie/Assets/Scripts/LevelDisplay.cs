using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelDisplay : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // REFERENCES
    // ─────────────────────────────────────────────
    [Header("References")]
    public TextMeshProUGUI levelText; // drag LevelText here in the Inspector

    // ─────────────────────────────────────────────
    // START — runs once when the scene loads
    // ─────────────────────────────────────────────
    void Start()
    {
        // get the current scene name and display it
        string currentScene = SceneManager.GetActiveScene().name;

        // convert scene name to a friendly display name
        // add more scenes here as you create them
        if (currentScene == "Level1")
            levelText.text = "Level 1";
        else if (currentScene == "Level2")
            levelText.text = "Level 2";
        else if (currentScene == "Level3")
            levelText.text = "Level 3";
        else
            levelText.text = currentScene; // fallback — just shows the scene name
    }
}