using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MenuButtonVisuals : MonoBehaviour,
    ISelectHandler, IDeselectHandler,
    IPointerEnterHandler
{
    public GameObject leftArrow;
    public GameObject rightArrow;
    public TextMeshProUGUI buttonText;

    void Start()
    {
        UpdateVisuals();
    }

    void Update()
    {
        UpdateVisuals();
    }

    // Keyboard/controller selection
    public void OnSelect(BaseEventData eventData)
    {
        UpdateVisuals();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        UpdateVisuals();
    }

    // Mouse hover → FORCE select this button
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    void UpdateVisuals()
    {
        bool isSelected = EventSystem.current.currentSelectedGameObject == gameObject;

        leftArrow.SetActive(isSelected);
        rightArrow.SetActive(isSelected);

        buttonText.fontStyle = isSelected ? FontStyles.Italic : FontStyles.Normal;
    }
}