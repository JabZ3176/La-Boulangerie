using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public string nextSceneName;
    public int nextLevelValue;
    private bool isUnlocked = false;

    public void Unlock()
    {
        PlayerPrefs.SetInt("LevelReached", nextLevelValue);
        isUnlocked = true;
        Debug.Log("Door unlocked! Walk through to proceed.");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isUnlocked)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}