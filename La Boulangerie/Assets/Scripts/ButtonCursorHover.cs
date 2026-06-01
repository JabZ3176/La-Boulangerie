using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class MenuButtonVisuals : MonoBehaviour,
    ISelectHandler,
    IDeselectHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    #region REFERENCES
    [Header("References")]
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Button button;
    #endregion

    #region VISUAL SETTINGS
    [Header("Selected Visuals")]
    [SerializeField] private Color selectedArrowColor = new Color(1f, 0.78f, 0f, 1f);
    [SerializeField] private bool hideArrowsWhenDisabled = true;
    [SerializeField] private bool preventDisabledHoverSelection = true;
    #endregion

    #region PRIVATE VARIABLES
    private Graphic[] leftArrowGraphics;
    private Graphic[] rightArrowGraphics;
    private FontStyles normalFontStyle;
    #endregion

    #region UNITY
    private void Reset()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>(true);
    }

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (buttonText != null)
            normalFontStyle = buttonText.fontStyle;

        CacheArrowGraphics();
    }

    private void OnEnable()
    {
        UpdateVisuals();
    }

    private void Start()
    {
        UpdateVisuals();
    }

    private void Update()
    {
        UpdateVisuals();
    }
    #endregion

    #region SELECTION EVENTS
    public void OnSelect(BaseEventData eventData)
    {
        UpdateVisuals();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        UpdateVisuals();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (preventDisabledHoverSelection && !IsButtonUsable())
        {
            UpdateVisuals();
            return;
        }

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UpdateVisuals();
    }
    #endregion

    #region PUBLIC API
    public void Refresh()
    {
        UpdateVisuals();
    }
    #endregion

    #region VISUALS
    private void UpdateVisuals()
    {
        bool usable = IsButtonUsable();
        bool selected = EventSystem.current != null &&
                        EventSystem.current.currentSelectedGameObject == gameObject;

        if (!usable)
        {
            if (hideArrowsWhenDisabled)
                SetArrowsActive(false);

            if (selected && preventDisabledHoverSelection && EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(null);

            SetTextSelected(false);
            return;
        }

        SetArrowsActive(selected);
        SetArrowColor(selectedArrowColor);
        SetTextSelected(selected);
    }

    private bool IsButtonUsable()
    {
        if (button == null) return true;
        return button.interactable && button.IsInteractable();
    }

    private void SetTextSelected(bool selected)
    {
        if (buttonText == null) return;

        buttonText.fontStyle = selected ? FontStyles.Italic : normalFontStyle;
    }

    private void SetArrowsActive(bool active)
    {
        if (leftArrow != null && leftArrow.activeSelf != active)
            leftArrow.SetActive(active);

        if (rightArrow != null && rightArrow.activeSelf != active)
            rightArrow.SetActive(active);
    }

    private void SetArrowColor(Color color)
    {
        ApplyColor(leftArrowGraphics, color);
        ApplyColor(rightArrowGraphics, color);
    }

    private void ApplyColor(Graphic[] graphics, Color color)
    {
        if (graphics == null) return;

        for (int i = 0; i < graphics.Length; i++)
        {
            if (graphics[i] == null) continue;
            graphics[i].color = color;
        }
    }

    private void CacheArrowGraphics()
    {
        leftArrowGraphics = leftArrow != null
            ? leftArrow.GetComponentsInChildren<Graphic>(true)
            : new Graphic[0];

        rightArrowGraphics = rightArrow != null
            ? rightArrow.GetComponentsInChildren<Graphic>(true)
            : new Graphic[0];
    }
    #endregion
}
