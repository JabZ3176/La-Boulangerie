using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public GameObject container; // drag your tutorial UI panel here

    private void Start()
    {
        // TEMPORARY — forces the tutorial to show regardless of PlayerPrefs
        container.SetActive(true);
        Time.timeScale = 0;
    }

    void Update()
    {
        // pressing Enter closes the tutorial and resumes the game
        if (Input.GetKeyDown(KeyCode.Return))
        {
            CloseTutorial();
        }
    }

    public void ResumeButton()
    {
        CloseTutorial();
    }

    private void CloseTutorial()
    {
        container.SetActive(false);
        Time.timeScale = 1;
    }
}