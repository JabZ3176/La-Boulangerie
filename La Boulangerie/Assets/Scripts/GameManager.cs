using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // ─────────────────────────────────────────────
    // INGREDIENT TRACKING
    // ─────────────────────────────────────────────
    [Header("Ingredient Totals — set per level in Inspector")]
    public int totalFlour;      // total flour in this level
    public int totalMilk;       // total milk in this level
    public int totalButter;     // total butter in this level

    [Header("Minimum Required — set per level in Inspector")]
    public int minFlour;        // minimum flour needed to unlock door
    public int minMilk;         // minimum milk needed to unlock door
    public int minButter;       // minimum butter needed to unlock door

    // tracks how many of each have been collected this level
    private int collectedFlour = 0;
    private int collectedMilk = 0;
    private int collectedButter = 0;

    // ─────────────────────────────────────────────
    // UI REFERENCES
    // ─────────────────────────────────────────────
    [Header("UI Panel")]
    public GameObject ingredientPanel;          // the whole panel background
    public TextMeshProUGUI flourCountText;       // shows "0/3" for flour
    public TextMeshProUGUI milkCountText;        // shows "0/3" for milk
    public TextMeshProUGUI butterCountText;      // shows "0/3" for butter
    public Image flourIcon;                     // flour sprite icon
    public Image milkIcon;                      // milk sprite icon
    public Image butterIcon;                    // butter sprite icon

    [Header("Unlock Screen")]
    public GameObject unlockPanel;

    [Header("Door")]
    public Door door;

    // ─────────────────────────────────────────────
    // AWAKE — runs before start
    // ─────────────────────────────────────────────
    void Awake()
    {
        Instance = this;
    }

    // ─────────────────────────────────────────────
    // START — runs once when scene loads
    // ─────────────────────────────────────────────
    void Start()
    {
        // hide unlock panel at start
        if (unlockPanel != null)
            unlockPanel.SetActive(false);

        // set the level requirements based on current scene
        SetLevelRequirements();

        // update the UI to show 0/total for each ingredient
        UpdateIngredientUI();
    }

    // ─────────────────────────────────────────────
    // SET LEVEL REQUIREMENTS
    // ─────────────────────────────────────────────
    private void SetLevelRequirements()
    {
        string scene = SceneManager.GetActiveScene().name;

        if (scene == "Level1")
        {
            totalFlour = 3; minFlour = 1;
            totalMilk = 3; minMilk = 1;
            totalButter = 3; minButter = 1;
        }
        else if (scene == "Level2")
        {
            totalFlour = 5; minFlour = 3;
            totalMilk = 5; minMilk = 3;
            totalButter = 5; minButter = 2;
        }
        else if (scene == "Level3")
        {
            totalFlour = 7; minFlour = 4;
            totalMilk = 7; minMilk = 4;
            totalButter = 7; minButter = 3;
        }
    }

    // ─────────────────────────────────────────────
    // COLLECT ITEM — called from Item.cs
    // ─────────────────────────────────────────────
    public void CollectItem(string itemType)
    {
        // add to the correct ingredient counter
        if (itemType == "Flour") collectedFlour++;
        else if (itemType == "Milk") collectedMilk++;
        else if (itemType == "Butter") collectedButter++;

        // save running totals to PlayerPrefs for the shop
        SaveIngredientTotals();

        // update the UI counters
        UpdateIngredientUI();

        // check if the door should unlock
        CheckDoorUnlock();
    }

    // ─────────────────────────────────────────────
    // UPDATE UI
    // ─────────────────────────────────────────────
    private void UpdateIngredientUI()
    {
        // update each counter text — green if minimum met, white if not
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

    // ─────────────────────────────────────────────
    // CHECK DOOR UNLOCK
    // ─────────────────────────────────────────────
    private void CheckDoorUnlock()
    {
        // door unlocks when all three minimums are met
        bool flourMet = collectedFlour >= minFlour;
        bool milkMet = collectedMilk >= minMilk;
        bool butterMet = collectedButter >= minButter;

        if (flourMet && milkMet && butterMet)
        {
            door.Unlock();
            ShowUnlockScreen();
        }
    }

    // ─────────────────────────────────────────────
    // SAVE INGREDIENT TOTALS FOR SHOP
    // ─────────────────────────────────────────────
    private void SaveIngredientTotals()
    {
        // add this level's collected ingredients to the running shop total
        PlayerPrefs.SetInt("ShopFlour",
            PlayerPrefs.GetInt("ShopFlour", 0) + 1);
        PlayerPrefs.SetInt("ShopMilk",
            PlayerPrefs.GetInt("ShopMilk", 0) + 1);
        PlayerPrefs.SetInt("ShopButter",
            PlayerPrefs.GetInt("ShopButter", 0) + 1);
        PlayerPrefs.Save();
    }

    // ─────────────────────────────────────────────
    // SHOW UNLOCK SCREEN
    // ─────────────────────────────────────────────
    void ShowUnlockScreen()
    {
        if (unlockPanel != null)
            unlockPanel.SetActive(true);
    }
}