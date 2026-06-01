using UnityEngine;
using UnityEngine.UI;

public class BaguetteUI : MonoBehaviour
{
    #region REFERENCES
    [Header("Baguette Slot Icons - Assign All 6")]
    public Image slot1;
    public Image slot2;
    public Image slot3;
    public Image slot4;
    public Image slot5;
    public Image slot6;

    [Header("Optional Background")]
    public RectTransform brownBackground;
    public bool resizeBackground = false;
    public float baseBackgroundWidth = 105f;
    public float widthPerExtraSlot = 20f;
    #endregion

    #region SPRITES
    [Header("Sprites")]
    public Sprite baguetteFull;
    public Sprite baguetteEmpty;
    #endregion

    private Image[] slots;

    private void Awake()
    {
        CacheSlots();
        UpdateSlots(0, GetSavedMaxSlots());
    }

    private void OnValidate()
    {
        CacheSlots();
    }

    public void UpdateSlots(int currentAmmo)
    {
        UpdateSlots(currentAmmo, GetSavedMaxSlots());
    }

    public void UpdateSlots(int currentAmmo, int maxSlots)
    {
        CacheSlots();
        if (slots == null || slots.Length == 0) return;

        maxSlots = Mathf.Clamp(maxSlots, 1, slots.Length);
        currentAmmo = Mathf.Clamp(currentAmmo, 0, maxSlots);

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;

            bool shouldShow = i < maxSlots;
            slots[i].gameObject.SetActive(shouldShow);

            if (!shouldShow) continue;

            slots[i].sprite = currentAmmo > i ? baguetteFull : baguetteEmpty;
            slots[i].rectTransform.localScale = Vector3.one;
        }

        ResizeBackground(maxSlots);
    }

    private void CacheSlots()
    {
        slots = new Image[]
        {
            slot1,
            slot2,
            slot3,
            slot4,
            slot5,
            slot6
        };
    }

    private int GetSavedMaxSlots()
    {
        int savedBaguetteLevel = PlayerPrefs.GetInt("Upgrade_Baguette", 0);
        int singletonBaguetteLevel = PlayerUpgrades.Instance != null ? PlayerUpgrades.Instance.baguetteLevel : 0;
        return 3 + Mathf.Clamp(Mathf.Max(savedBaguetteLevel, singletonBaguetteLevel), 0, 3);
    }

    private void ResizeBackground(int maxSlots)
    {
        if (!resizeBackground) return;
        if (brownBackground == null) return;

        Vector2 size = brownBackground.sizeDelta;
        size.x = baseBackgroundWidth + Mathf.Max(0, maxSlots - 3) * widthPerExtraSlot;
        brownBackground.sizeDelta = size;
    }
}
