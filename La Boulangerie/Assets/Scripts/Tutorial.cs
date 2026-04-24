using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public GameObject container;

    private void Start()
    {
        if (PlayerPrefs.GetInt("ShowTutorial", 0) == 1)
        {
            container.SetActive(true);
            Time.timeScale = 0;
            PlayerPrefs.SetInt("ShowTutorial", 0); 
            PlayerPrefs.Save();
        }
        else
        {
            container.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            container.SetActive(false);
            Time.timeScale = 1;
        }
    }
    public void ResumeButton()
    {
        container.SetActive(false);
        Time.timeScale = 1;
    }

}   

