using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject container;
    public GameObject container1;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            container.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void ResumeButton()
    {
        container.SetActive(false);
        Time.timeScale = 1;

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            container.SetActive(true);
            Time.timeScale = 1;
        }
    }

    public void MainMenuButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void Levels()
    {
               UnityEngine.SceneManagement.SceneManager.LoadScene("LevelScene");
       Time.timeScale = 0;
    }

    public void Tutorial()
    {
        container1.SetActive(true);
        container.SetActive(false);
        Time.timeScale = 1;
    }
}
