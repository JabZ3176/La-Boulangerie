using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    #region SETTINGS
    public string nextSceneName;
    public int nextLevelValue;
    #endregion

    #region REFERENCES
    [Header("References")]
    public DoorChoicePanel choicePanel;
    #endregion

    #region PRIVATE VARIABLES
    private bool isUnlocked = false;
    #endregion

    #region UNLOCK
    public void Unlock()
    {
        isUnlocked = true;

        int currentBest = PlayerPrefs.GetInt("LevelReached", 1);
        if (nextLevelValue > currentBest)
            PlayerPrefs.SetInt("LevelReached", nextLevelValue);

        string currentScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetInt("ShopUnlocked", 1);
        PlayerPrefs.SetString("LastCompletedLevel", currentScene);
        PlayerPrefs.SetString("CurrentShopReturnLevel", currentScene);
        PlayerPrefs.SetString("NextLevelAfterShop", nextSceneName);
        PlayerPrefs.SetString("CurrentLevel", currentScene);
        PlayerPrefs.Save();

        if (choicePanel != null)
            choicePanel.Unlock(nextSceneName);
    }
    #endregion

    #region TRIGGER
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !isUnlocked) return;

        if (choicePanel != null)
            choicePanel.ShowChoicePanel();
        else
            SceneManager.LoadScene(nextSceneName);
    }
    #endregion
}
