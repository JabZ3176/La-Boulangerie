using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public string nextSceneName;    // name of the next scene to load
    public int nextLevelValue;      // the level number this door unlocks

    private bool isUnlocked = false;

    public void Unlock()
    {
        isUnlocked = true;

        // only update best progress if this is higher than what was saved before
        int currentBest = PlayerPrefs.GetInt("LevelReached", 1);
        if (nextLevelValue > currentBest)
        {
            PlayerPrefs.SetInt("LevelReached", nextLevelValue);
        }

        // save the next scene as the level to continue from
        PlayerPrefs.SetString("CurrentLevel", nextSceneName);
        PlayerPrefs.Save();

        Debug.Log("Progress saved. Continue will load: " + nextSceneName);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isUnlocked)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}