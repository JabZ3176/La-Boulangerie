using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelScene : MonoBehaviour
{
    #region BUTTONS

    [Header("Level Buttons")]
    public Button level1Button;
    public Button level2Button;
    public Button level3Button;

    #endregion

    #region START

    void Start()
    {
        int levelReached = PlayerPrefs.GetInt("LevelReached", 1);

        SetupButton(level1Button, levelReached >= 1);
        SetupButton(level2Button, levelReached >= 2);
        SetupButton(level3Button, levelReached >= 3);
    }

    #endregion

    #region BUTTON SETUP

    private void SetupButton(Button button, bool unlocked)
    {
        if (button == null) return;

        button.interactable = unlocked;

        Graphic[] graphics = button.GetComponentsInChildren<Graphic>(true);

        foreach (Graphic graphic in graphics)
        {
            Color color = graphic.color;
            color.a = unlocked ? 1f : 0.3f;
            graphic.color = color;
        }
    }

    #endregion

    #region LEVELS

    public void Level1()
    {
        if (PlayerPrefs.GetInt("HasPlayedBefore", 0) == 0)
        {
            PlayerPrefs.SetInt("HasPlayedBefore", 1);
            PlayerPrefs.Save();

            SceneManager.LoadScene("Tutorial");
            return;
        }

        SceneManager.LoadScene("Level1");
    }

    public void Level2()
    {
        if (PlayerPrefs.GetInt("LevelReached", 1) >= 2)
        {
            SceneManager.LoadScene("Level2");
        }
    }

    public void Level3()
    {
        if (PlayerPrefs.GetInt("LevelReached", 1) >= 3)
        {
            SceneManager.LoadScene("Level3");
        }
    }

    #endregion

    #region BACK

    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }

    #endregion
}