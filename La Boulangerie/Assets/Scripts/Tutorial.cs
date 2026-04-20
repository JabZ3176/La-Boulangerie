using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public GameObject container;

    private void Start()
    {
        
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

