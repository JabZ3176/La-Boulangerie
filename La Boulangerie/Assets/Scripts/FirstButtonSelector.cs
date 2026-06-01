using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuStartSelection : MonoBehaviour
{
    #region REFERENCES
    public GameObject firstSelectedButton;
    #endregion

    #region START
    private void Start()
    {
        SelectFirstAvailableButton();
    }
    #endregion

    #region SELECTION
    private void SelectFirstAvailableButton()
    {
        if (EventSystem.current == null) return;

        if (firstSelectedButton != null)
        {
            Button firstButton = firstSelectedButton.GetComponent<Button>();
            if (firstButton == null || firstButton.interactable)
            {
                EventSystem.current.SetSelectedGameObject(firstSelectedButton);
                return;
            }
        }

        Button[] buttons = Object.FindObjectsByType<Button>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (Button button in buttons)
        {
            if (button != null && button.interactable)
            {
                EventSystem.current.SetSelectedGameObject(button.gameObject);
                return;
            }
        }
    }
    #endregion
}
