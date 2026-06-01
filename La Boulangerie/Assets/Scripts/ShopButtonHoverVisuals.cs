using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Hover/selection visuals for shop Buy buttons.
/// Gives shop buttons the same left/right arrow feel as the main menu buttons,
/// while staying safe for disabled buttons and not interfering with ShopUpgradeRowUI buying logic.
/// </summary>
[DisallowMultipleComponent]
public class ShopButtonHoverVisuals : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    ISelectHandler, IDeselectHandler
{
    #region REFERENCES
    [Header("Button")]
    [SerializeField] private Button button;

    [Header("Arrows")]
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;

    [Header("Optional Extra Hover Visual")]
    [Tooltip("Optional. Leave empty if you only want arrows.")]
    [SerializeField] private GameObject hoverVisual;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI buttonText;
    #endregion

    #region STYLE
    [Header("Text Style")]
    [SerializeField] private FontStyles normalStyle = FontStyles.Normal;
    [SerializeField] private FontStyles selectedStyle = FontStyles.Italic;

    [Header("Arrow Colour")]
    [SerializeField] private bool forceArrowColour = true;
    [SerializeField] private Color selectedArrowColour = new Color(1f, 0.84f, 0f, 1f);

    [Header("Selection")]
    [Tooltip("If on, moving the mouse over the button also selects it for keyboard/controller navigation.")]
    [SerializeField] private bool selectOnHover = true;
    #endregion

    #region PRIVATE VARIABLES
    private bool hovered;
    private bool selected;
    private Graphic[] leftArrowGraphics;
    private Graphic[] rightArrowGraphics;
    #endregion

    #region UNITY
    private void Reset()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (buttonText == null)
            buttonText = GetComponentInChildren<TextMeshProUGUI>();

        CacheArrowGraphics();
    }

    private void OnEnable()
    {
        hovered = false;
        selected = false;
        Refresh();
    }

    private void OnDisable()
    {
        SetVisuals(false);
    }
    #endregion

    #region EVENTS
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!CanShowHover())
        {
            Refresh();
            return;
        }

        hovered = true;

        if (selectOnHover && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(gameObject);

        Refresh();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
        Refresh();
    }

    public void OnSelect(BaseEventData eventData)
    {
        selected = true;
        Refresh();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        selected = false;
        Refresh();
    }
    #endregion

    #region PUBLIC
    /// <summary>
    /// Call this after changing button.interactable if you want the arrows to update immediately.
    /// ShopUpgradeRowUI does not need to call this, but it is safe if it does.
    /// </summary>
    public void Refresh()
    {
        bool active = CanShowHover() && (hovered || selected);
        SetVisuals(active);
    }
    #endregion

    #region VISUALS
    private void SetVisuals(bool active)
    {
        if (leftArrow != null)
            leftArrow.SetActive(active);

        if (rightArrow != null)
            rightArrow.SetActive(active);

        if (hoverVisual != null)
            hoverVisual.SetActive(active);

        if (buttonText != null)
            buttonText.fontStyle = active ? selectedStyle : normalStyle;

        if (forceArrowColour && active)
            ApplyArrowColour();
    }

    private void ApplyArrowColour()
    {
        if (leftArrowGraphics == null || rightArrowGraphics == null)
            CacheArrowGraphics();

        if (leftArrowGraphics != null)
        {
            for (int i = 0; i < leftArrowGraphics.Length; i++)
            {
                if (leftArrowGraphics[i] != null)
                    leftArrowGraphics[i].color = selectedArrowColour;
            }
        }

        if (rightArrowGraphics != null)
        {
            for (int i = 0; i < rightArrowGraphics.Length; i++)
            {
                if (rightArrowGraphics[i] != null)
                    rightArrowGraphics[i].color = selectedArrowColour;
            }
        }
    }

    private void CacheArrowGraphics()
    {
        leftArrowGraphics = leftArrow != null ? leftArrow.GetComponentsInChildren<Graphic>(true) : null;
        rightArrowGraphics = rightArrow != null ? rightArrow.GetComponentsInChildren<Graphic>(true) : null;
    }

    private bool CanShowHover()
    {
        if (!isActiveAndEnabled) return false;
        if (button == null) button = GetComponent<Button>();
        return button == null || button.interactable;
    }
    #endregion
}
