using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DoorChoicePanel : MonoBehaviour
{
    #region REFERENCES
    [Header("Panel")]
    public GameObject choicePanel;

    [Header("Buttons")]
    public Button nextLevelButton;
    public Button shopButton;
    public Button stayButton;

    [Header("Settings")]
    public string nextSceneName;
    public string shopSceneName = "Shop";
    #endregion

    #region PRIVATE VARIABLES
    private bool isUnlocked = false;
    #endregion

    #region START
    private void Start()
    {
        if (choicePanel != null)
            choicePanel.SetActive(false);
    }
    #endregion

    #region UNLOCK
    public void Unlock(string nextScene)
    {
        isUnlocked = true;
        nextSceneName = nextScene;
        SaveShopRoute();
    }
    #endregion

    #region TRIGGER
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isUnlocked)
            ShowChoicePanel();
    }
    #endregion

    #region CHOICE PANEL
    public void ShowChoicePanel()
    {
        if (choicePanel == null) return;

        choicePanel.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnNextLevel()
    {
        Time.timeScale = 1f;
        SaveShopRoute();

        PlayerPrefs.SetString("CurrentLevel", nextSceneName);
        PlayerPrefs.Save();

        SceneManager.LoadScene(nextSceneName);
    }

    public void OnGoToShop()
    {
        Time.timeScale = 1f;
        SaveShopRoute();
        SceneManager.LoadScene(shopSceneName);
    }

    public void OnStay()
    {
        if (choicePanel != null)
            choicePanel.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    #endregion

    #region SAVE ROUTE
    private void SaveShopRoute()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        PlayerPrefs.SetInt("ShopUnlocked", 1);
        PlayerPrefs.SetString("LastCompletedLevel", currentScene);
        PlayerPrefs.SetString("CurrentShopReturnLevel", currentScene);
        PlayerPrefs.SetString("NextLevelAfterShop", nextSceneName);
        PlayerPrefs.Save();
    }
    #endregion
}
