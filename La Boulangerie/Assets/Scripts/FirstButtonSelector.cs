using UnityEngine;
using UnityEngine.EventSystems;

public class MenuStartSelection : MonoBehaviour
{
    #region REFERENCES
    public GameObject firstSelectedButton;
    #endregion

    #region START
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }
    #endregion
}
