using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelDisplay : MonoBehaviour
{
    #region REFERENCES
    [Header("References")]
    public TextMeshProUGUI levelText;
    #endregion

    #region START
    void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Level1")
            levelText.text = "Level 1";
        else if (currentScene == "Level2")
            levelText.text = "Level 2";
        else if (currentScene == "Level3")
            levelText.text = "Level 3";
        else
            levelText.text = currentScene;
    }
    #endregion
}
