using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region SINGLETON

    public static GameManager Instance;

    #endregion

    #region INGREDIENT TRACKING

    [Header("Ingredient Totals")]
    public int totalFlour;
    public int totalMilk;
    public int totalButter;

    [Header("Minimum Required")]
    public int minFlour;
    public int minMilk;
    public int minButter;

    private int collectedFlour = 0;
    private int collectedMilk = 0;
    private int collectedButter = 0;

    #endregion

    #region UI REFERENCES

    [Header("UI Panel")]
    public GameObject ingredientPanel;
    public TextMeshProUGUI flourCountText;
    public TextMeshProUGUI milkCountText;
    public TextMeshProUGUI butterCountText;
    public Image flourIcon;
    public Image milkIcon;
    public Image butterIcon;

    [Header("Unlock Screen")]
    public GameObject unlockPanel;

    [Header("Door")]
    public Door door;

    #endregion

    #region PRIVATE VARIABLES

    private Coroutine unlockPanelCoroutine;
    private bool doorUnlocked = false;

    #endregion

    #region AWAKE

    void Awake()
    {
        Instance = this;
    }

    #endregion

    #region START

    void Start()
    {
        if (unlockPanel != null)
            unlockPanel.SetActive(false);

        SetLevelRequirements();
        UpdateIngredientUI();
    }

    #endregion

    #region LEVEL REQUIREMENTS

    private void SetLevelRequirements()
    {
        string scene = SceneManager.GetActiveScene().name;

        if (scene == "Level1")
        {
            totalFlour = 2;
            minFlour = 1;
            totalMilk = 2;
            minMilk = 1;
            totalButter = 2;
            minButter = 1;
        }
        else if (scene == "Level2")
        {
            totalFlour = 3;
            minFlour = 1;
            totalMilk = 3;
            minMilk = 1;
            totalButter = 3;
            minButter = 2;
        }
        else if (scene == "Level3")
        {
            totalFlour = 5;
            minFlour = 2;
            totalMilk = 5;
            minMilk = 2;
            totalButter = 5;
            minButter = 1;
        }
    }

    #endregion

    #region ITEM COLLECTION

    public void CollectItem(string itemType)
    {
        if (itemType == "Flour")
            collectedFlour++;
        else if (itemType == "Milk")
            collectedMilk++;
        else if (itemType == "Butter")
            collectedButter++;

        SaveIngredientTotals();
        UpdateIngredientUI();
        CheckDoorUnlock();
    }

    #endregion

    #region UI UPDATES

    private void UpdateIngredientUI()
    {
        if (flourCountText != null)
        {
            flourCountText.text = collectedFlour + "/" + totalFlour;
            flourCountText.color = collectedFlour >= minFlour ? Color.green : Color.white;
        }

        if (milkCountText != null)
        {
            milkCountText.text = collectedMilk + "/" + totalMilk;
            milkCountText.color = collectedMilk >= minMilk ? Color.green : Color.white;
        }

        if (butterCountText != null)
        {
            butterCountText.text = collectedButter + "/" + totalButter;
            butterCountText.color = collectedButter >= minButter ? Color.green : Color.white;
        }
    }

    #endregion

    #region DOOR UNLOCK

    private void CheckDoorUnlock()
    {
        if (doorUnlocked) return;

        bool flourMet = collectedFlour >= minFlour;
        bool milkMet = collectedMilk >= minMilk;
        bool butterMet = collectedButter >= minButter;

        if (flourMet && milkMet && butterMet)
        {
            doorUnlocked = true;
            door.Unlock();
            ShowUnlockScreen();
        }
    }

    private void ShowUnlockScreen()
    {
        if (unlockPanelCoroutine != null)
            StopCoroutine(unlockPanelCoroutine);

        unlockPanelCoroutine = StartCoroutine(UnlockPanelTimer());
    }

    private IEnumerator UnlockPanelTimer()
    {
        if (unlockPanel != null)
            unlockPanel.SetActive(true);

        yield return new WaitForSeconds(5f);

        if (unlockPanel != null)
            unlockPanel.SetActive(false);
    }

    #endregion

    #region SAVE DATA

    private void SaveIngredientTotals()
    {
        PlayerPrefs.SetInt("ShopFlour",
            PlayerPrefs.GetInt("ShopFlour", 0) + 1);
        PlayerPrefs.SetInt("ShopMilk",
            PlayerPrefs.GetInt("ShopMilk", 0) + 1);
        PlayerPrefs.SetInt("ShopButter",
            PlayerPrefs.GetInt("ShopButter", 0) + 1);
        PlayerPrefs.Save();
    }

    #endregion
}
