using UnityEngine;
using UnityEngine.UI;

public class HeartHealthBar : MonoBehaviour
{
    #region REFERENCES
    [Header("Heart Images - Assign All 6")]
    public Image heart1;
    public Image heart2;
    public Image heart3;
    public Image heart4;
    public Image heart5;
    public Image heart6;

    [Header("Optional Background")]
    public RectTransform brownBackground;
    public bool resizeBackground = false;
    public float baseBackgroundWidth = 105f;
    public float widthPerExtraHeart = 18f;
    #endregion

    #region SPRITES
    [Header("Sprites")]
    public Sprite heartFull;
    public Sprite heartEmpty;
    #endregion

    private Image[] hearts;

    private void Awake()
    {
        CacheHearts();
    }

    private void OnValidate()
    {
        CacheHearts();
    }

    public void UpdateHearts(int currentHealth, int maxHealth = 3)
    {
        CacheHearts();
        if (hearts == null || hearts.Length == 0) return;

        if (maxHealth <= 3)
        {
            int savedHealthLevel = PlayerPrefs.GetInt("Upgrade_Health", 0);
            int singletonHealthLevel = PlayerUpgrades.Instance != null ? PlayerUpgrades.Instance.healthLevel : 0;
            maxHealth = 3 + Mathf.Clamp(Mathf.Max(savedHealthLevel, singletonHealthLevel), 0, 3);
        }

        maxHealth = Mathf.Clamp(maxHealth, 1, hearts.Length);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null) continue;

            bool shouldShow = i < maxHealth;
            hearts[i].gameObject.SetActive(shouldShow);

            if (!shouldShow) continue;

            hearts[i].sprite = currentHealth > i ? heartFull : heartEmpty;
            hearts[i].rectTransform.localScale = Vector3.one;
        }

        ResizeBackground(maxHealth);
    }

    private void CacheHearts()
    {
        hearts = new Image[]
        {
            heart1,
            heart2,
            heart3,
            heart4,
            heart5,
            heart6
        };
    }

    private void ResizeBackground(int maxHealth)
    {
        if (!resizeBackground) return;
        if (brownBackground == null) return;

        Vector2 size = brownBackground.sizeDelta;
        size.x = baseBackgroundWidth + Mathf.Max(0, maxHealth - 3) * widthPerExtraHeart;
        brownBackground.sizeDelta = size;
    }
}
