using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUpgradeRowUI : MonoBehaviour
{
    #region SETTINGS
    [Header("Upgrade")]
    public ShopUpgradeType upgradeType;
    public string displayName = "Upgrade";
    public Sprite icon;
    #endregion

    #region REFERENCES
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI requirementText;
    public Button buyButton;
    public TextMeshProUGUI buyButtonText;
    #endregion

    #region BUTTON CONNECTION
    [Header("Button Connection")]
    [Tooltip("Leave this OFF if you want to wire the Buy button manually in Button > On Click().")]
    public bool connectBuyButtonAutomatically = false;
    #endregion

    #region COLORS
    [Header("Colors")]
    public Color affordableColor = Color.white;
    public Color unaffordableColor = Color.red;
    public Color maxedColor = Color.green;
    #endregion

    private ShopManager shopManager;

    private void Reset()
    {
        AutoAssignMissingReferences();
    }

    private void OnValidate()
    {
        AutoAssignMissingReferences();
    }

    public void Setup(ShopManager manager)
    {
        shopManager = manager;
        AutoAssignMissingReferences();

        if (connectBuyButtonAutomatically && buyButton != null)
        {
            buyButton.onClick.RemoveListener(BuyThisUpgrade);
            buyButton.onClick.AddListener(BuyThisUpgrade);
        }

        if (iconImage != null && icon != null)
            iconImage.sprite = icon;

        if (nameText != null)
            nameText.text = displayName;
    }

    public void Refresh(int currentLevel, int maxLevel, UpgradeCost cost, bool canAfford)
    {
        AutoAssignMissingReferences();

        bool maxed = currentLevel >= maxLevel;

        if (iconImage != null && icon != null)
            iconImage.sprite = icon;

        if (nameText != null)
            nameText.text = displayName;

        if (levelText != null)
            levelText.text = "Upgrade " + currentLevel + "/" + maxLevel;

        if (requirementText != null)
        {
            if (maxed)
            {
                requirementText.text = "Fully upgraded";
                requirementText.color = maxedColor;
            }
            else
            {
                requirementText.text = cost.ToRequirementText();
                requirementText.color = canAfford ? affordableColor : unaffordableColor;
            }
        }

        if (buyButton != null)
            buyButton.interactable = !maxed && canAfford;

        if (buyButtonText != null)
            buyButtonText.text = maxed ? "Owned" : "Buy";
    }

    // Manual OnClick option:
    // Drag this row GameObject into Button > On Click(), then choose ShopUpgradeRowUI > BuyThisUpgrade().
    public void BuyThisUpgrade()
    {
        if (shopManager == null)
            shopManager = Object.FindFirstObjectByType<ShopManager>();

        if (shopManager == null)
        {
            Debug.LogError("No ShopManager found. Add ShopManager.cs to an object in the Shop scene.", this);
            return;
        }

        shopManager.BuyUpgrade(upgradeType);
    }

    private void AutoAssignMissingReferences()
    {
        if (buyButton == null)
            buyButton = GetComponentInChildren<Button>(true);

        if (buyButtonText == null && buyButton != null)
            buyButtonText = buyButton.GetComponentInChildren<TextMeshProUGUI>(true);

        if (iconImage == null)
        {
            Image[] images = GetComponentsInChildren<Image>(true);
            for (int i = 0; i < images.Length; i++)
            {
                if (buyButton != null && images[i].transform.IsChildOf(buyButton.transform)) continue;
                if (images[i].gameObject == gameObject) continue;

                iconImage = images[i];
                break;
            }
        }
    }
}
