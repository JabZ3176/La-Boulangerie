using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackToMenuButton : MonoBehaviour
{
public void BackToMenu()
    {

        SceneManager.LoadScene("MainMenu");
    }
}
