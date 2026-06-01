using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class IngredientTracker : MonoBehaviour
{
    #region REFERENCES
    [Header("Text References")]
    public TextMeshProUGUI flourText;
    public TextMeshProUGUI milkText;
    public TextMeshProUGUI butterText;
    public TextMeshProUGUI totalText;

    [Header("Icons")]
    public Image flourIcon;
    public Image milkIcon;
    public Image butterIcon;

    [Header("Navigation Panels")]
    [Tooltip("The ingredient tracker panel/container. If left empty, this GameObject will be used.")]
    public GameObject ingredientTrackerPanel;

    [Tooltip("The main pause menu panel/container that should reopen when pressing Back To Pause Menu.")]
    public GameObject pauseMenuPanel;

    [Header("Scene Navigation")]
    public string mainMenuSceneName = "MainMenu";
    #endregion

    #region ON ENABLE
    void OnEnable()
    {
        UpdateDisplay();
    }
    #endregion

    #region UPDATE DISPLAY
    public void UpdateDisplay()
    {
        int flour = PlayerPrefs.GetInt("TotalFlour", 0);
        int milk = PlayerPrefs.GetInt("TotalMilk", 0);
        int butter = PlayerPrefs.GetInt("TotalButter", 0);
        int total = flour + milk + butter;

        if (flourText != null)
            flourText.text = "Flour:     " + flour;

        if (milkText != null)
            milkText.text = "Milk:      " + milk;

        if (butterText != null)
            butterText.text = "Butter:    " + butter;

        if (totalText != null)
            totalText.text = "Total:     " + total;
    }
    #endregion

    #region BUTTONS
    public void BackToPauseMenu()
    {
        GameObject trackerPanel = ingredientTrackerPanel != null
            ? ingredientTrackerPanel
            : gameObject;

        trackerPanel.SetActive(false);

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(mainMenuSceneName);
    }
    #endregion
}
