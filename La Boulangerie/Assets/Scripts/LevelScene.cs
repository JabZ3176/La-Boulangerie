using TMPro;
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

    #region VISUAL SETTINGS
    [Header("Locked Button Visuals")]
    [Range(0f, 1f)] public float lockedAlpha = 0.3f;
    [Range(0f, 1f)] public float unlockedAlpha = 1f;
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

        // Only fade the actual button visuals/text. Do not rely on disabled arrows
        // staying a certain color, because MenuButtonVisuals controls arrow color.
        Graphic[] graphics = button.GetComponentsInChildren<Graphic>(true);
        foreach (Graphic graphic in graphics)
        {
            if (graphic == null) continue;
            if (IsInsideArrowObject(button, graphic.transform)) continue;

            Color color = graphic.color;
            color.a = unlocked ? unlockedAlpha : lockedAlpha;
            graphic.color = color;
        }

        MenuButtonVisuals visuals = button.GetComponent<MenuButtonVisuals>();
        if (visuals != null)
            visuals.Refresh();
    }

    private bool IsInsideArrowObject(Button button, Transform graphicTransform)
    {
        MenuButtonVisuals visuals = button.GetComponent<MenuButtonVisuals>();
        if (visuals == null) return false;

        // Arrows are hidden/colored by MenuButtonVisuals, so this method is kept
        // intentionally conservative. If you need custom arrow child filtering,
        // use MenuButtonVisuals to control the arrow objects directly.
        return graphicTransform.name.ToLower().Contains("arrow");
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
            SceneManager.LoadScene("Level2");
    }

    public void Level3()
    {
        if (PlayerPrefs.GetInt("LevelReached", 1) >= 3)
            SceneManager.LoadScene("Level3");
    }
    #endregion

    #region BACK
    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
    #endregion
}
