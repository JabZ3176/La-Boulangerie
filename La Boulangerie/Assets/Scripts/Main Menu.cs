using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    #region BUTTONS
    [Header("Buttons")]
    public Button continueButton;
    #endregion

    #region START
    void Start()
    {
        if (PlayerPrefs.GetString("CurrentLevel", "") == "")
        {
            continueButton.interactable = false;
            Graphic[] graphics = continueButton.GetComponentsInChildren<Graphic>();

            foreach (Graphic graphic in graphics)
            {
                Color color = graphic.color;
                color.a = 0.3f;
                graphic.color = color;
            }
        }
        else
        {
            continueButton.interactable = true;
        }
    }
    #endregion

    #region GAME FLOW
    public void NewGame()
    {
        PlayerPrefs.DeleteKey("CurrentLevel");
        PlayerPrefs.DeleteKey("LevelReached");
        PlayerPrefs.DeleteKey("TotalFlour");
        PlayerPrefs.DeleteKey("TotalMilk");
        PlayerPrefs.DeleteKey("TotalButter");
        PlayerPrefs.DeleteKey("ShopUnlocked");
        PlayerPrefs.DeleteKey("NextLevelAfterShop");
        PlayerPrefs.DeleteKey("CurrentShopReturnLevel");

        PlayerPrefs.DeleteKey("Upgrade_Health");
        PlayerPrefs.DeleteKey("Upgrade_Baguette");
        PlayerPrefs.DeleteKey("Upgrade_Stamina");
        PlayerPrefs.DeleteKey("Upgrade_Movement");
        PlayerPrefs.DeleteKey("Upgrade_Jump");
        PlayerPrefs.DeleteKey("Upgrade_BaguetteDamage");

        // clear all collected item flags
        // add every itemID here that exists in your game
        string[] itemIDs = new string[]
        {
        "Level1_Flour_1", "Level1_Flour_2",
        "Level1_Milk_1",  "Level1_Milk_2",
        "Level1_Butter_1","Level1_Butter_2",
        "Level2_Flour_1", "Level2_Flour_2", "Level2_Flour_3",
        "Level2_Milk_1",  "Level2_Milk_2",  "Level2_Milk_3",
        "Level2_Butter_1","Level2_Butter_2","Level2_Butter_3",
        "Level3_Flour_1", "Level3_Flour_2", "Level3_Flour_3",
        "Level3_Flour_4", "Level3_Flour_5",
        "Level3_Milk_1",  "Level3_Milk_2",  "Level3_Milk_3",
        "Level3_Milk_4",  "Level3_Milk_5",
        "Level3_Butter_1","Level3_Butter_2","Level3_Butter_3",
        "Level3_Butter_4","Level3_Butter_5",
        };

        foreach (string id in itemIDs)
            PlayerPrefs.DeleteKey("Collected_" + id);

        if (PlayerUpgrades.Instance != null)
            PlayerUpgrades.Instance.ResetUpgrades();

        PlayerPrefs.SetInt("HasPlayedBefore", 1);
        PlayerPrefs.Save();

        Time.timeScale = 1f;

        bool firstTime = PlayerPrefs.GetInt("HasPlayedBefore", 0) == 0;

        if (firstTime)
        {
            PlayerPrefs.SetInt("HasPlayedBefore", 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Tutorial");
        }
        else
        {
            SceneManager.LoadScene("Level1");
        }
    }

    public void ContinueGame()
    {
        PlayerPrefs.SetInt("ShowTutorial", 0);
        PlayerPrefs.Save();

        string savedLevel = PlayerPrefs.GetString("CurrentLevel", "Level1");
        Time.timeScale = 1f;
        SceneManager.LoadScene(savedLevel);
    }

    public void OpenTutorial()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Tutorial");
    }

    public void OpenIngredientTracker()
    {
        SceneManager.LoadScene("IngredientTracker");
    }
    #endregion

    #region NAVIGATION
    public void QuitGame()
    {
        Application.Quit();
    }

    public void Levels()
    {
        SceneManager.LoadScene("LevelScene");
    }
    #endregion
}
