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

        // IMPORTANT:
        // Ingredients themselves are permanently hidden using PlayerPrefs in Item.cs.
        // When the level reloads, the GameManager used to reset collectedFlour/Milk/Butter to 0,
        // which made the UI and door requirements reset even though the items were gone.
        // This rebuilds the current level's collected counts from the saved item IDs.
        LoadCollectedIngredientsForCurrentLevel();

        UpdateIngredientUI();

        // Restore the door if the player had already collected enough ingredients in this level.
        // false means do not show the unlock popup again when returning to a completed level.
        CheckDoorUnlock(false);
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

    #region LOAD LEVEL PROGRESS

    private void LoadCollectedIngredientsForCurrentLevel()
    {
        collectedFlour = 0;
        collectedMilk = 0;
        collectedButter = 0;

        Item[] items = Object.FindObjectsByType<Item>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (Item item in items)
        {
            if (item == null) continue;
            if (string.IsNullOrEmpty(item.itemID)) continue;

            bool alreadyCollected = PlayerPrefs.GetInt("Collected_" + item.itemID, 0) == 1;
            if (!alreadyCollected) continue;

            AddCollectedCountOnly(item.itemType);
        }
    }

    private void AddCollectedCountOnly(string itemType)
    {
        if (itemType == "Flour")
            collectedFlour++;
        else if (itemType == "Milk")
            collectedMilk++;
        else if (itemType == "Butter")
            collectedButter++;
    }

    #endregion

    #region ITEM COLLECTION

    public void CollectItem(string itemType)
    {
        AddCollectedCountOnly(itemType);

        SaveIngredientTotals(itemType);
        UpdateIngredientUI();
        CheckDoorUnlock(true);
    }

    #endregion

    #region SAVE DATA

    private void SaveIngredientTotals(string itemType)
    {
        // save running totals for shop
        if (itemType == "Flour")
            PlayerPrefs.SetInt("TotalFlour", PlayerPrefs.GetInt("TotalFlour", 0) + 1);
        else if (itemType == "Milk")
            PlayerPrefs.SetInt("TotalMilk", PlayerPrefs.GetInt("TotalMilk", 0) + 1);
        else if (itemType == "Butter")
            PlayerPrefs.SetInt("TotalButter", PlayerPrefs.GetInt("TotalButter", 0) + 1);

        PlayerPrefs.Save();
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

    private void CheckDoorUnlock(bool showUnlockScreen)
    {
        if (doorUnlocked) return;

        bool flourMet = collectedFlour >= minFlour;
        bool milkMet = collectedMilk >= minMilk;
        bool butterMet = collectedButter >= minButter;

        if (flourMet && milkMet && butterMet)
        {
            doorUnlocked = true;

            if (door != null)
                door.Unlock();

            if (showUnlockScreen)
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
}
