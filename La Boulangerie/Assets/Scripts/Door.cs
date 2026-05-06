using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    #region SETTINGS
    public string nextSceneName;
    public int nextLevelValue;
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
        {
            PlayerPrefs.SetInt("LevelReached", nextLevelValue);
        }

        PlayerPrefs.SetString("CurrentLevel", nextSceneName);
        PlayerPrefs.Save();

        Debug.Log("Progress saved. Continue will load: " + nextSceneName);
    }
    #endregion

    #region TRIGGER
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isUnlocked)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
    #endregion
}
