using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenuButton : MonoBehaviour
{
    #region MENU NAVIGATION
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    #endregion
}
