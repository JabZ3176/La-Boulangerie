using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    #region SCENES
    [Header("Scene Names")]
    public string mainMenuSceneName = "MainMenu";
    public string fallbackPreviousLevel = "Level1";
    public string fallbackNextLevel = "Level1";

    [Header("Access")]
    [Tooltip("When true, the shop redirects to the main menu if the player has never completed/unlocked a level door.")]
    public bool requireCompletedLevelToUseShop = true;
    #endregion

    #region CURRENCY UI
    [Header("Currency Display")]
    public TextMeshProUGUI flourText;
    public TextMeshProUGUI milkText;
    public TextMeshProUGUI butterText;
    public TextMeshProUGUI totalText;
    #endregion

    #region SHOP ROWS
    [Header("Upgrade Rows")]
    [Tooltip("You can assign the rows here, or leave this empty and the manager will find every ShopUpgradeRowUI in the scene.")]
    public ShopUpgradeRowUI[] upgradeRows;
    #endregion

    #region PRIVATE VARIABLES
    private int flour;
    private int milk;
    private int butter;
    #endregion

    #region PLAYER PREF KEYS
    private const string FlourKey = "TotalFlour";
    private const string MilkKey = "TotalMilk";
    private const string ButterKey = "TotalButter";
    private const string ShopUnlockedKey = "ShopUnlocked";
    private const string NextLevelKey = "NextLevelAfterShop";
    private const string PreviousLevelKey = "CurrentShopReturnLevel";
    #endregion

    #region START
    private void Start()
    {
        Time.timeScale = 1f;

        if (requireCompletedLevelToUseShop && PlayerPrefs.GetInt(ShopUnlockedKey, 0) == 0)
        {
            SceneManager.LoadScene(mainMenuSceneName);
            return;
        }

        FindRowsIfNeeded();
        SetupRows();
        RefreshShop();
    }
    #endregion

    #region SETUP
    private void FindRowsIfNeeded()
    {
        if (upgradeRows != null && upgradeRows.Length > 0) return;

        List<ShopUpgradeRowUI> foundRows = new List<ShopUpgradeRowUI>();
        foundRows.AddRange(GetComponentsInChildren<ShopUpgradeRowUI>(true));

#if UNITY_2023_1_OR_NEWER
        ShopUpgradeRowUI[] allRows = Object.FindObjectsByType<ShopUpgradeRowUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
        ShopUpgradeRowUI[] allRows = Object.FindObjectsOfType<ShopUpgradeRowUI>(true);
#endif

        for (int i = 0; i < allRows.Length; i++)
        {
            if (allRows[i] != null && !foundRows.Contains(allRows[i]))
                foundRows.Add(allRows[i]);
        }

        upgradeRows = foundRows.ToArray();
    }

    private void SetupRows()
    {
        if (upgradeRows == null || upgradeRows.Length == 0)
        {
            Debug.LogError("ShopManager could not find any ShopUpgradeRowUI rows. Add ShopUpgradeRowUI to each upgrade row panel, or drag the rows into Upgrade Rows.", this);
            return;
        }

        for (int i = 0; i < upgradeRows.Length; i++)
        {
            if (upgradeRows[i] != null)
                upgradeRows[i].Setup(this);
        }
    }
    #endregion

    #region MANUAL BUY METHODS FOR BUTTON ON CLICK
    // Drag your ShopManager object into the Button On Click() slot,
    // then choose one of these methods for the matching Buy button.

    public void BuyHealthUpgrade()
    {
        BuyUpgrade(ShopUpgradeType.Health);
    }

    public void BuyBaguetteSlotsUpgrade()
    {
        BuyUpgrade(ShopUpgradeType.BaguetteSlots);
    }

    public void BuyStaminaUpgrade()
    {
        BuyUpgrade(ShopUpgradeType.Stamina);
    }

    public void BuyMovementUpgrade()
    {
        BuyUpgrade(ShopUpgradeType.Movement);
    }

    public void BuyJumpUpgrade()
    {
        BuyUpgrade(ShopUpgradeType.Jump);
    }

    public void BuyBaguetteDamageUpgrade()
    {
        BuyUpgrade(ShopUpgradeType.BaguetteDamage);
    }
    #endregion

    #region CURRENCY
    private void LoadCurrency()
    {
        flour = PlayerPrefs.GetInt(FlourKey, 0);
        milk = PlayerPrefs.GetInt(MilkKey, 0);
        butter = PlayerPrefs.GetInt(ButterKey, 0);
    }

    private void SaveCurrency()
    {
        PlayerPrefs.SetInt(FlourKey, flour);
        PlayerPrefs.SetInt(MilkKey, milk);
        PlayerPrefs.SetInt(ButterKey, butter);
        PlayerPrefs.Save();
    }

    private void UpdateCurrencyDisplay()
    {
        if (flourText != null) flourText.text = "Flour: " + flour;
        if (milkText != null) milkText.text = "Milk: " + milk;
        if (butterText != null) butterText.text = "Butter: " + butter;
        if (totalText != null) totalText.text = "Total Ingredients: " + (flour + milk + butter);
    }

    private bool CanAfford(UpgradeCost cost)
    {
        if (cost.useTotalIngredients)
            return flour + milk + butter >= cost.totalIngredients;

        return flour >= cost.flour && milk >= cost.milk && butter >= cost.butter;
    }

    private void Spend(UpgradeCost cost)
    {
        if (cost.useTotalIngredients)
        {
            SpendTotalIngredients(cost.totalIngredients);
            return;
        }

        flour -= cost.flour;
        milk -= cost.milk;
        butter -= cost.butter;
    }

    private void SpendTotalIngredients(int amount)
    {
        int remaining = amount;

        int spendFlour = Mathf.Min(flour, remaining);
        flour -= spendFlour;
        remaining -= spendFlour;

        int spendMilk = Mathf.Min(milk, remaining);
        milk -= spendMilk;
        remaining -= spendMilk;

        int spendButter = Mathf.Min(butter, remaining);
        butter -= spendButter;
    }
    #endregion

    #region SHOP REFRESH
    public void RefreshShop()
    {
        LoadCurrency();
        UpdateCurrencyDisplay();
        FindRowsIfNeeded();

        if (upgradeRows == null) return;

        for (int i = 0; i < upgradeRows.Length; i++)
        {
            ShopUpgradeRowUI row = upgradeRows[i];
            if (row == null) continue;

            int currentLevel = GetUpgradeLevel(row.upgradeType);
            int maxLevel = GetMaxLevel(row.upgradeType);
            UpgradeCost cost = GetUpgradeCost(row.upgradeType, currentLevel);
            bool canAfford = currentLevel < maxLevel && CanAfford(cost);

            row.Refresh(currentLevel, maxLevel, cost, canAfford);
        }
    }
    #endregion

    #region BUYING
    public void BuyUpgrade(ShopUpgradeType upgradeType)
    {
        LoadCurrency();

        int currentLevel = GetUpgradeLevel(upgradeType);
        int maxLevel = GetMaxLevel(upgradeType);
        if (currentLevel >= maxLevel)
        {
            RefreshShop();
            return;
        }

        UpgradeCost cost = GetUpgradeCost(upgradeType, currentLevel);
        if (!CanAfford(cost))
        {
            Debug.Log("Cannot afford " + upgradeType + ". Need: " + cost.ToRequirementText());
            RefreshShop();
            return;
        }

        Spend(cost);
        SaveCurrency();

        SetUpgradeLevel(upgradeType, currentLevel + 1);
        RefreshShop();
    }
    #endregion

    #region UPGRADE LEVELS
    private int GetUpgradeLevel(ShopUpgradeType upgradeType)
    {
        if (PlayerUpgrades.Instance != null)
        {
            switch (upgradeType)
            {
                case ShopUpgradeType.Health: return PlayerUpgrades.Instance.healthLevel;
                case ShopUpgradeType.BaguetteSlots: return PlayerUpgrades.Instance.baguetteLevel;
                case ShopUpgradeType.Stamina: return PlayerUpgrades.Instance.staminaLevel;
                case ShopUpgradeType.Movement: return PlayerUpgrades.Instance.movementLevel;
                case ShopUpgradeType.Jump: return PlayerUpgrades.Instance.jumpLevel;
                case ShopUpgradeType.BaguetteDamage: return PlayerUpgrades.Instance.baguetteDamageLevel;
            }
        }

        return PlayerPrefs.GetInt(GetUpgradePrefsKey(upgradeType), 0);
    }

    private void SetUpgradeLevel(ShopUpgradeType upgradeType, int level)
    {
        level = Mathf.Clamp(level, 0, GetMaxLevel(upgradeType));

        if (PlayerUpgrades.Instance != null)
        {
            switch (upgradeType)
            {
                case ShopUpgradeType.Health:
                    PlayerUpgrades.Instance.healthLevel = level;
                    break;
                case ShopUpgradeType.BaguetteSlots:
                    PlayerUpgrades.Instance.baguetteLevel = level;
                    break;
                case ShopUpgradeType.Stamina:
                    PlayerUpgrades.Instance.staminaLevel = level;
                    break;
                case ShopUpgradeType.Movement:
                    PlayerUpgrades.Instance.movementLevel = level;
                    break;
                case ShopUpgradeType.Jump:
                    PlayerUpgrades.Instance.jumpLevel = level;
                    break;
                case ShopUpgradeType.BaguetteDamage:
                    PlayerUpgrades.Instance.baguetteDamageLevel = level;
                    break;
            }

            PlayerUpgrades.Instance.SaveUpgrades();
            return;
        }

        PlayerPrefs.SetInt(GetUpgradePrefsKey(upgradeType), level);
        PlayerPrefs.Save();
    }

    private string GetUpgradePrefsKey(ShopUpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case ShopUpgradeType.Health: return "Upgrade_Health";
            case ShopUpgradeType.BaguetteSlots: return "Upgrade_Baguette";
            case ShopUpgradeType.Stamina: return "Upgrade_Stamina";
            case ShopUpgradeType.Movement: return "Upgrade_Movement";
            case ShopUpgradeType.Jump: return "Upgrade_Jump";
            case ShopUpgradeType.BaguetteDamage: return "Upgrade_BaguetteDamage";
            default: return "Upgrade_Unknown";
        }
    }

    private int GetMaxLevel(ShopUpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case ShopUpgradeType.Health: return 3;
            case ShopUpgradeType.BaguetteSlots: return 3;
            case ShopUpgradeType.Stamina: return 2;
            case ShopUpgradeType.Movement: return 2;
            case ShopUpgradeType.Jump: return 2;
            case ShopUpgradeType.BaguetteDamage: return 2;
            default: return 0;
        }
    }
    #endregion

    #region UPGRADE COSTS
    private UpgradeCost GetUpgradeCost(ShopUpgradeType upgradeType, int currentLevel)
    {
        switch (upgradeType)
        {
            case ShopUpgradeType.Health:
                if (currentLevel == 0) return new UpgradeCost(1, 2, 1);
                if (currentLevel == 1) return new UpgradeCost(3, 3, 2);
                return new UpgradeCost(10);

            case ShopUpgradeType.BaguetteSlots:
                if (currentLevel == 0) return new UpgradeCost(2, 1, 1);
                if (currentLevel == 1) return new UpgradeCost(3, 2, 2);
                return new UpgradeCost(4, 3, 3);

            case ShopUpgradeType.Stamina:
                if (currentLevel == 0) return new UpgradeCost(3, 2, 4);
                return new UpgradeCost(4, 4, 5);

            case ShopUpgradeType.Movement:
                if (currentLevel == 0) return new UpgradeCost(2, 2, 1);
                return new UpgradeCost(4, 3, 3);

            case ShopUpgradeType.Jump:
                if (currentLevel == 0) return new UpgradeCost(1, 2, 2);
                return new UpgradeCost(3, 4, 4);

            case ShopUpgradeType.BaguetteDamage:
                if (currentLevel == 0) return new UpgradeCost(3, 2, 3);
                return new UpgradeCost(5, 4, 5);

            default:
                return new UpgradeCost(0, 0, 0);
        }
    }
    #endregion

    #region NAVIGATION BUTTONS
    public void ContinueToNextLevel()
    {
        string nextLevel = PlayerPrefs.GetString(NextLevelKey, fallbackNextLevel);
        if (string.IsNullOrEmpty(nextLevel)) nextLevel = fallbackNextLevel;

        PlayerPrefs.SetString("CurrentLevel", nextLevel);
        PlayerPrefs.Save();
        SceneManager.LoadScene(nextLevel);
    }

    public void ReturnToPreviousLevel()
    {
        string previousLevel = PlayerPrefs.GetString(PreviousLevelKey, fallbackPreviousLevel);
        if (string.IsNullOrEmpty(previousLevel)) previousLevel = fallbackPreviousLevel;

        PlayerPrefs.SetString("CurrentLevel", previousLevel);
        PlayerPrefs.Save();
        SceneManager.LoadScene(previousLevel);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
    #endregion
}
