using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public string nextSceneName; // type the name of your next scene here
    private bool isUnlocked = false;

    public void Unlock()
    {
        isUnlocked = true;
        Debug.Log("Door unlocked! Walk through to proceed.");
        // Optional: play an animation or change the door sprite here
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isUnlocked)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}