using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonVisuals : MonoBehaviour,
    ISelectHandler, IDeselectHandler,
    IPointerEnterHandler
{
    #region REFERENCES
    public GameObject leftArrow;
    public GameObject rightArrow;
    public TextMeshProUGUI buttonText;
    #endregion

    #region START
    void Start()
    {
        UpdateVisuals();
    }
    #endregion

    #region UPDATE
    void Update()
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
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
    #endregion

    #region VISUALS
    void UpdateVisuals()
    {
        bool isSelected = EventSystem.current.currentSelectedGameObject == gameObject;

        leftArrow.SetActive(isSelected);
        rightArrow.SetActive(isSelected);

        buttonText.fontStyle = isSelected ? FontStyles.Italic : FontStyles.Normal;
    }
    #endregion
}
